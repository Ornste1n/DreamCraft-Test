using Components;
using Unity.Burst;
using Unity.Physics;
using Unity.Entities;
using Unity.Collections;

namespace Systems.Enemy
{
    [BurstCompile]
    public partial struct EnemyAttackSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SimulationSingleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EnemyAttackJob attackJob = new()
            {
                CharacterLookup = SystemAPI.GetComponentLookup<CharacterTag>(true),
                AttackData = SystemAPI.GetComponentLookup<EnemyAttackData>(true),
                DynamicBuffer = SystemAPI.GetBufferLookup<DamageThisFrame>()
            };

            SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = attackJob.Schedule(simulation, state.Dependency);
        }
    }
    
    // Job-обработка столкновений врагов и игрока
    public struct EnemyAttackJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentLookup<CharacterTag> CharacterLookup;
        [ReadOnly] public ComponentLookup<EnemyAttackData> AttackData;

        public BufferLookup<DamageThisFrame> DynamicBuffer;
        
        public void Execute(CollisionEvent collision)
        {
            Entity characterEn = default;
            Entity enemyEn = default;

            if (CharacterLookup.HasComponent(collision.EntityA) && AttackData.HasComponent(collision.EntityB))
            {
                characterEn = collision.EntityA;
                enemyEn = collision.EntityB;
            }
            else if (CharacterLookup.HasComponent(collision.EntityB) && AttackData.HasComponent(collision.EntityA))
            {
                characterEn = collision.EntityB;
                enemyEn = collision.EntityA;
            }
            else
            {
                return;
            }

            EnemyAttackData attackData = AttackData[enemyEn];
            DynamicBuffer<DamageThisFrame> playerBuffer = DynamicBuffer[characterEn];
            playerBuffer.Add(new DamageThisFrame(attackData.Damage)); // добавляю damage врага в буфер
        }
    }
}