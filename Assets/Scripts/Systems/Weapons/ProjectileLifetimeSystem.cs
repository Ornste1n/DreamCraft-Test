using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Components.Aspects;

namespace Systems.Weapons
{
    // Инициализация пула пуль
    [BurstCompile]
    public partial struct ProjectilePoolInitSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<ProjectilePoolData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);

            Entity poolEntity = SystemAPI.GetSingletonEntity<ProjectilePoolData>();
            ProjectilePoolData poolData = SystemAPI.GetComponent<ProjectilePoolData>(poolEntity);

            ecb.AddBuffer<ProjectileBufferElement>(poolEntity);

            for (int i = 0; i < poolData.PoolSize; i++)
            {
                Entity proj = ecb.Instantiate(poolData.ProjectilePrefab);
                ecb.SetEnabled(proj, false);
                ecb.AddComponent(proj, new ProjectileData());
                ecb.AppendToBuffer(poolEntity, new ProjectileBufferElement { Projectile = proj });
            }

            ecb.Playback(state.EntityManager);
            ecb.Dispose();

            state.Enabled = false;
        }
    }

    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ProjectileMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<BeginSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            float deltaTime = SystemAPI.Time.DeltaTime;

            var ecbSystem = SystemAPI.GetSingleton<BeginSimulationEntityCommandBufferSystem.Singleton>();
            EntityCommandBuffer ecb = ecbSystem.CreateCommandBuffer(state.WorldUnmanaged);

            var job = new ProjectileMoveJob
            {
                DeltaTime = deltaTime,
                Ecb = ecb.AsParallelWriter()
            };

            job.ScheduleParallel();
        }
    }

    // Job для просчета движения пуль
    [BurstCompile]
    public partial struct ProjectileMoveJob : IJobEntity
    {
        public float DeltaTime;
        public EntityCommandBuffer.ParallelWriter Ecb;

        private void Execute(ProjectileAspect aspect, [EntityIndexInQuery] int entityInQueryIndex)
        {
            ProjectileData data = aspect.Data;

            data.TimeAlive += DeltaTime;
            if (data.TimeAlive >= data.Lifetime) // выключаем если слишком долго живет
            {
                Ecb.SetEnabled(entityInQueryIndex, aspect.Entity, false);
                return;
            }

            float3 bulletVelocity = data.Direction * data.Speed;

            if (data.SineAmplitude > 0f)
            {
                // просчет син тракетории
                float prevSine = math.sin((data.TimeAlive - DeltaTime) * data.SineFrequency) * data.SineAmplitude;
                float currSine = math.sin(data.TimeAlive * data.SineFrequency) * data.SineAmplitude;

                float3 perp = new(-data.Direction.y, data.Direction.x, 0f);
                float3 sideVelocity = perp * (currSine - prevSine) / DeltaTime;

                bulletVelocity += sideVelocity;
            }

            aspect.Linear = bulletVelocity;
            aspect.Data = data;
        }
    }
}