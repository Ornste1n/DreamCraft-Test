using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Components.Aspects;

namespace Systems.Units
{
    [BurstCompile]
    public partial struct UnitsMoveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<UnitsMoveDirection>();
        }

        public void OnUpdate(ref SystemState state)
        {
            foreach (UnitAspect unitAspect in SystemAPI.Query<UnitAspect>())
            {
                float2 moveSpeed = unitAspect.MoveDirection * unitAspect.MoveSpeed;
                unitAspect.Linear = new float3(moveSpeed, 0f);

                if (math.abs(moveSpeed.x) > 0.15f)
                {
                    float facingDir = unitAspect.InvertRotation // поворачиваем unit-а исходя из направления и флага
                        ? -math.sign(moveSpeed.x) : math.sign(moveSpeed.x);

                    unitAspect.UpdateSpriteScale(facingDir);
                }

                AnimationIndexEnum animationType = math.lengthsq(moveSpeed) > float.Epsilon
                    ? AnimationIndexEnum.Move
                    : AnimationIndexEnum.Idle;

                unitAspect.AnimationIndex = (float)animationType; // устанавливаю индекс анимации в материал
            }
        }
    }
}