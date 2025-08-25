using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Authoring.Enemies;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics;
using Entity = Unity.Entities.Entity;

namespace Systems.Enemy
{
    // Система спавна врага
    // расширение пула тоже сделал здесь для быстроты, по-хорошему надо вынести
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct EnemySpawnSystem : ISystem
    {
        private EntityQuery _poolQuery;
        
        public void OnCreate(ref SystemState state)
        {
            _poolQuery = state.GetEntityQuery(
                ComponentType.ReadOnly<EnemyPoolSettings>(),
                ComponentType.ReadWrite<EnemyPoolElement>()
            );
            
            state.RequireForUpdate<EnemySpawnComponent>();
            state.RequireForUpdate<EnemyPoolInitialized>();
            state.RequireForUpdate<CameraTransformComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            EntityManager em = state.EntityManager;
            EntityCommandBuffer ecb = new(Allocator.Temp);

            ref EnemySpawnComponent spawnData = ref SystemAPI.GetSingletonRW<EnemySpawnComponent>().ValueRW;
            spawnData.Timer -= deltaTime;

            if (spawnData.Timer > 0f) // spawn delay
            {
                ecb.Dispose();
                return;
            }

            spawnData.Timer = spawnData.SpawnDelay;

            (bool canSpawn, float3 pos) = GetSpawnPosition(spawnData); // получаю позицию вне камеры и в пределах карты
            if (!canSpawn) // если например пределы карты малы и умещаются в камере, то не спавню на глазах у игрока
            {
                ecb.Dispose();
                return;
            }
            
            if (_poolQuery.IsEmptyIgnoreFilter)
            {
                ecb.Dispose();
                return;
            }

            using NativeArray<Entity> pools = _poolQuery.ToEntityArray(Allocator.Temp);

            int typeIndex = UnityEngine.Random.Range(0, pools.Length); // выбираю рандомный пул
            Entity chosenPool = pools[typeIndex];

            DynamicBuffer<EnemyPoolElement> buffer = em.GetBuffer<EnemyPoolElement>(chosenPool);
            EnemyPoolSettings settings = em.GetComponentData<EnemyPoolSettings>(chosenPool);

            int oldCount = buffer.Length;
            int freeIndex = -1;

            for (int i = 0; i < oldCount; i++)
            {
                if(!SystemAPI.HasComponent<Disabled>(buffer[i].Enemy)) continue;
                
                freeIndex = i;
                break;
            }

            if (freeIndex != -1)
            {
                Entity enemy = buffer[freeIndex].Enemy;

                LocalTransform lt = em.GetComponentData<LocalTransform>(enemy);
                lt.Position = pos;
                ecb.SetComponent(enemy, lt);

                if (!em.HasComponent<ActiveEnemyTag>(enemy))
                    ecb.AddComponent<ActiveEnemyTag>(enemy);

                ecb.SetEnabled(enemy, true);

                ref HealthComponent healthComponent = ref SystemAPI.GetComponentRW<HealthComponent>(enemy).ValueRW;
                healthComponent.Health = healthComponent.MaxHealth;
                
                buffer[freeIndex] = new EnemyPoolElement { Enemy = enemy };
            }
            else // если не нашел, то расширяю пул
            {
                if (settings.PoolSize < settings.MaxPoolSize)
                {
                    int toAdd = math.min(settings.PoolSize * 2, settings.MaxPoolSize) - settings.PoolSize;

                    PhysicsMass mass = em.GetComponentData<PhysicsMass>(settings.Prefab);
                    mass.InverseInertia = float3.zero;
                    
                    for (int j = 0; j < toAdd; j++)
                    {
                        Entity instance = ecb.Instantiate(settings.Prefab);

                        if (j == 0)
                        {
                            ecb.SetEnabled(instance, true);
                            ecb.AddComponent<ActiveEnemyTag>(instance);

                            LocalTransform prefabLT = em.GetComponentData<LocalTransform>(settings.Prefab);
                            prefabLT.Position = pos;
                            ecb.SetComponent(instance, prefabLT);
                            ecb.AppendToBuffer(chosenPool, new EnemyPoolElement { Enemy = instance });
                        }
                        else
                        {
                            ecb.SetEnabled(instance, false);
                            ecb.RemoveComponent<ActiveEnemyTag>(instance);
                            ecb.AppendToBuffer(chosenPool, new EnemyPoolElement { Enemy = instance });
                        }
                        
                        ecb.SetComponent(instance, mass);
                    }

                    settings.PoolSize += toAdd;
                    ecb.SetComponent(chosenPool, settings);
                }
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }

        // Расчет позиции для спавна; используются четкие координаты границы карты (для быстроты)
        private (bool CanSpawn, float3 Position) GetSpawnPosition(EnemySpawnComponent spawnData)
        {
            CameraTransformComponent cam = SystemAPI.GetSingleton<CameraTransformComponent>();

            // границы камеры
            float leftCam = cam.Position.x - cam.HalfWidth;
            float rightCam = cam.Position.x + cam.HalfWidth;
            float bottomCam = cam.Position.y - cam.HalfHeight;
            float topCam = cam.Position.y + cam.HalfHeight;

            float leftSpawnMax = math.min(leftCam - 0.1f, spawnData.RightBound);
            float rightSpawnMin = math.max(rightCam + 0.1f, spawnData.LeftBound);

            bool canSpawnLeft = leftSpawnMax > spawnData.LeftBound;
            bool canSpawnRight = rightSpawnMin < spawnData.RightBound;

            if (!canSpawnLeft && !canSpawnRight)
                return (false, float3.zero);

            float x = canSpawnLeft switch // тяжеловатая конструкция
            {
                // если можно спавнить и слева и справа, то рандомно выбирают сторону и пределы
                true when canSpawnRight => UnityEngine.Random.value < 0.5f
                    ? UnityEngine.Random.Range(spawnData.LeftBound, leftSpawnMax)
                    : UnityEngine.Random.Range(rightSpawnMin, spawnData.RightBound), 
                
                // иначе только в пределах слева
                true => UnityEngine.Random.Range(spawnData.LeftBound, leftSpawnMax),
                
                // в пределах справа
                _ => UnityEngine.Random.Range(rightSpawnMin, spawnData.RightBound)
            };

            float bottomSpawnMax = math.min(bottomCam - 0.1f, spawnData.TopBound);
            float topSpawnMin = math.max(topCam + 0.1f, spawnData.BottomBound);

            bool canSpawnBottom = bottomSpawnMax > spawnData.BottomBound;
            bool canSpawnTop = topSpawnMin < spawnData.TopBound;

            if (!canSpawnBottom && !canSpawnTop)
                return (false, float3.zero);

            // по аналогии с x
            float y = canSpawnBottom switch
            {
                true when canSpawnTop => UnityEngine.Random.value < 0.5f
                    ? UnityEngine.Random.Range(spawnData.BottomBound, bottomSpawnMax)
                    : UnityEngine.Random.Range(topSpawnMin, spawnData.TopBound),
                true => UnityEngine.Random.Range(spawnData.BottomBound, bottomSpawnMax),
                _ => UnityEngine.Random.Range(topSpawnMin, spawnData.TopBound)
            };

            return (true, new float3(x, y, 0f));
        }
    }
}
