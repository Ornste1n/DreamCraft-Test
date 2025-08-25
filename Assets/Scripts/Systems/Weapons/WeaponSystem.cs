using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Collections;
using Unity.Mathematics;
using Components.Aspects;
using EntityCommandBuffer = Unity.Entities.EntityCommandBuffer;

namespace Systems.Weapons
{
    // Обрабатывает события и выстрела
    // Берет пулю или расширяет пул - также по-хорошему вынести эту логику в другое место
    // Для быстроты здесь
    [BurstCompile]
    public partial struct WeaponSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CharacterTag>();
            state.RequireForUpdate<MouseClickEvent>();
            state.RequireForUpdate<ProjectileBlobRef>();
            state.RequireForUpdate<ProjectilePoolData>();
            state.RequireForUpdate<CharacterWeaponComponent>();
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Entity eventEntity = SystemAPI.GetSingletonEntity<MouseClickEvent>();
            MouseClickEvent clickEvent = SystemAPI.GetComponent<MouseClickEvent>(eventEntity);

            UnitAspect unit = SystemAPI.GetAspect<UnitAspect>(SystemAPI.GetSingletonEntity<CharacterTag>());
            CharacterWeaponComponent weapon = SystemAPI.GetSingleton<CharacterWeaponComponent>();
            ProjectileBlob blob = SystemAPI.GetSingleton<ProjectileBlobRef>().Catalog.Value.Array[weapon.CurrentId];

            Entity poolEntity = SystemAPI.GetSingletonEntity<ProjectilePoolData>();
            ref ProjectilePoolData pool = ref SystemAPI.GetComponentRW<ProjectilePoolData>(poolEntity).ValueRW;

            DynamicBuffer<ProjectileBufferElement> buffer = SystemAPI.GetBuffer<ProjectileBufferElement>(poolEntity);

            var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);
            EntityManager em = state.EntityManager;

            // параметры пула
            int createdThisFrame = 0;
            int growPerFrame = math.max(0, pool.GrowPerFrame);
            int maxPoolSize = (pool.MaxPoolSize > 0) ? pool.MaxPoolSize : int.MaxValue;

            float3 baseDir = math.normalize(clickEvent.WorldPosition - unit.Position);
            float halfSpread = blob.SpreadAngle * 0.5f;
            
            // в зависимости от количества пулек при выстреле 
            for (int i = 0; i < blob.Count; i++)
            {
                Entity projectile = GetNextOrCreateProjectile(ref pool, buffer, poolEntity, ecb, em,
                    ref createdThisFrame, growPerFrame, maxPoolSize);

                if (projectile == Entity.Null) continue;
      
                // просчитываю смещение пуль исходя из SpreadAngle, эффект 'кучности' или наоборот
                float t = (blob.Count == 1) ? 0f : (float)i / (blob.Count - 1);
                float angle = math.lerp(-halfSpread, halfSpread, t);
                float3 dir = math.mul(quaternion.RotateZ(math.radians(angle)), baseDir); // получаю конечную траекторию

                ecb.SetComponent(projectile, LocalTransform.FromPositionRotation(unit.Position, quaternion.identity));
                ecb.SetComponent(projectile, new ProjectileData
                {
                    Damage = blob.Damage,
                    Direction = dir,
                    Speed = blob.Speed,
                    Lifetime = blob.Lifetime,
                    SineAmplitude = blob.SineAmplitude,
                    SineFrequency = blob.SineFrequency,
                });
            }

            ecb.RemoveComponent<MouseClickEvent>(eventEntity); // удаляю компонент ивента, чтобы система засыпала
        }

        // взять пулю из пула или создать новые
        private Entity GetNextOrCreateProjectile
        (
            ref ProjectilePoolData pool,
            [ReadOnly] DynamicBuffer<ProjectileBufferElement> buffer,
            Entity poolEntity,
            EntityCommandBuffer ecb,
            EntityManager em,
            ref int createdThisFrame,
            int growPerFrame,
            int maxPoolSize
            )
        {
            int length = buffer.Length;

            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    int idx = (pool.CurrentIndex + i) % length;
                    Entity candidate = buffer[idx].Projectile;

                    if (em.IsEnabled(candidate)) continue;

                    pool.CurrentIndex = (idx + 1) % math.max(1, length);
                    ecb.SetEnabled(candidate, true);
                    return candidate; // возвращает неактивную пулю из пула
                }
            }

            int canGrow = math.min(growPerFrame, math.max(0, maxPoolSize - pool.PoolSize)); // сколько можно заспавнить

            if (canGrow <= 0) return Entity.Null;

            Entity firstNew = Entity.Null;
            for (int i = 0; i < canGrow; i++) // создаем новые пули
            {
                Entity proj = ecb.Instantiate(pool.ProjectilePrefab);
                ecb.SetEnabled(proj, true);

                ecb.AddComponent(proj, new ProjectileData());
                ecb.AppendToBuffer(poolEntity, new ProjectileBufferElement { Projectile = proj });

                if (firstNew == Entity.Null)
                    firstNew = proj;
            }

            pool.PoolSize += canGrow;
            createdThisFrame += canGrow;
            pool.CurrentIndex = 0;

            return firstNew; // возвращаю первую из только что созданных
        }
    }
}