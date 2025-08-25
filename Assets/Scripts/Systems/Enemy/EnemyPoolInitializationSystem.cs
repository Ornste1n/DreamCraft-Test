using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Physics;

namespace Systems.Enemy
{
    // Инициализация нескольих пулов врагов (каждой тип врага - отдельный пул)
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct EnemyPoolInitializationSystem : ISystem
    {
        private EntityQuery _query;
        
        public void OnCreate(ref SystemState state)
        {
            _query = state.GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[] { typeof(EnemyPoolSettings), ComponentType.ReadWrite<EnemyPoolElement>() },
                None = new ComponentType[] { typeof(EnemyPoolInitialized) }
            });
            
            state.RequireForUpdate<EnemyPoolSettings>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);

            using (NativeArray<Entity> poolEntities = _query.ToEntityArray(Allocator.Temp))
            {
                foreach (Entity poolEntity in poolEntities)
                {
                    EnemyPoolSettings settings = state.EntityManager.GetComponentData<EnemyPoolSettings>(poolEntity);

                    PhysicsMass mass = state.EntityManager.GetComponentData<PhysicsMass>(settings.Prefab);
                    mass.InverseInertia = float3.zero;
                    
                    for (int i = 0; i < settings.PoolSize; i++)
                    {
                        Entity instance = ecb.Instantiate(settings.Prefab);
                        
                        ecb.SetEnabled(instance, false);
                        ecb.SetComponent(instance, mass);
                        ecb.AppendToBuffer(poolEntity, new EnemyPoolElement { Enemy = instance } );
                    }

                    ecb.AddComponent<EnemyPoolInitialized>(poolEntity);
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            state.Enabled = false;
        }
    }
}