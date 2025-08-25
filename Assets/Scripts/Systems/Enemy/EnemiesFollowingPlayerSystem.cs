using Authoring;
using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Components.Aspects;

namespace Systems.Enemy
{
    [BurstCompile]
    public partial struct EnemiesFollowingPlayerSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CharacterTag>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Entity characterEntity = SystemAPI.GetSingletonEntity<CharacterTag>();
            UnitAspect unitAspect = SystemAPI.GetAspect<UnitAspect>(characterEntity);

            EnemyMoveToJob job = new()
            {
                TargetPosition = unitAspect.Position.xy
            };

            state.Dependency = job.ScheduleParallel(state.Dependency);
        }
    }
    
    /// Job которая устанавливаем врагам их направление движения за игроком
    [BurstCompile]
    [WithAll(typeof(EnemyTag))]
    public partial struct EnemyMoveToJob : IJobEntity
    {
        public float2 TargetPosition;
        
        private void Execute(ref UnitsMoveDirection unitsMoveInput, in LocalTransform transform)
        {
            float2 direction = TargetPosition - transform.Position.xy;
            unitsMoveInput.Value = math.normalize(direction);
        }
    }
}