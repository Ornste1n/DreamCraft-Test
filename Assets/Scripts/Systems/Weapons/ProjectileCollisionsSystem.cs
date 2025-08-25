using Components;
using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Collections;
using Unity.Physics.Systems;

namespace Systems.Weapons
{
    [BurstCompile]
    [UpdateInGroup(typeof(PhysicsSystemGroup))]
    [UpdateBefore(typeof(AfterPhysicsSystemGroup))]
    public partial struct ProjectileCollisionsSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.TempJob);
            
            ProjectileAttackJob projectileJob = new()
            {
                ProjectileData = SystemAPI.GetComponentLookup<ProjectileData>(true),
                EnemyLookup = SystemAPI.GetComponentLookup<EnemyTag>(true),
                DynamicBuffer = SystemAPI.GetBufferLookup<DamageThisFrame>(),
                ECB = ecb.AsParallelWriter()
            };

            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = projectileJob.Schedule(simulation, state.Dependency);
            
            state.Dependency.Complete();
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }

    // Job-обработка тригер столкновений врагов и пуль
    public struct ProjectileAttackJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentLookup<ProjectileData> ProjectileData;
        [ReadOnly] public ComponentLookup<EnemyTag> EnemyLookup;
        
        public BufferLookup<DamageThisFrame> DynamicBuffer;
        public EntityCommandBuffer.ParallelWriter ECB;
        
        public void Execute(TriggerEvent trigger)
        {
            Entity projectileEn = default;
            Entity enemyEn = default;
            
            if (ProjectileData.HasComponent(trigger.EntityA) && EnemyLookup.HasComponent(trigger.EntityB))
            {
                projectileEn = trigger.EntityA;
                enemyEn = trigger.EntityB;
            }
            else if (ProjectileData.HasComponent(trigger.EntityB) && EnemyLookup.HasComponent(trigger.EntityA))
            {
                projectileEn = trigger.EntityB;
                enemyEn = trigger.EntityA;
            }
            else
            {
                return;
            }

            float damage = ProjectileData[projectileEn].Damage;
            DynamicBuffer<DamageThisFrame> enemyBuffer = DynamicBuffer[enemyEn];
            enemyBuffer.Add(new DamageThisFrame(damage));
            
            ECB.SetEnabled(0, projectileEn, false); // выключаем пулю
        }
    }
}