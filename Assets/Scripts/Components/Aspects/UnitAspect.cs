using Unity.Physics;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

namespace Components.Aspects
{
    /// <summary>
    /// Аспект для группировки всех юнитов
    /// </summary>
    public readonly partial struct UnitAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<HealthComponent> m_Health;
        
        private readonly RefRW<PhysicsVelocity> m_Physics;
        private readonly RefRW<LocalTransform> m_Transform;

        private readonly RefRW<UnitsMoveDirection> m_MoveDirection;
        private readonly RefRO<UnitMoveComponent> m_UnitMove;
        
        private readonly RefRW<UnitAnimationIndexOverride> m_AnimationIndex;

        private readonly DynamicBuffer<DamageThisFrame> m_Buffer;

        #region Properties
        public float Health
        {
            get => m_Health.ValueRO.Health;
            set => m_Health.ValueRW.Health = value;
        }

        public float MaxHealth => m_Health.ValueRO.MaxHealth;
        
        public DynamicBuffer<DamageThisFrame> DamageBuffer => m_Buffer;
        
        public float MoveSpeed => m_UnitMove.ValueRO.MoveSpeed;
        public bool InvertRotation => m_UnitMove.ValueRO.InvertRotation;
        
        public float3 Position => m_Transform.ValueRO.Position;
        
        public float3 Linear
        {
            get => m_Physics.ValueRO.Linear;
            set => m_Physics.ValueRW.Linear = value;
        }

        public float2 MoveDirection
        {
            get => m_MoveDirection.ValueRO.Value;
            set => m_MoveDirection.ValueRW.Value = value;
        }
        
        public float AnimationIndex { set => m_AnimationIndex.ValueRW.Value = value; }
        #endregion
        
        public void UpdateSpriteScale(float facing)
        {
            m_Transform.ValueRW.Rotation = quaternion.EulerXYZ(0, (facing < 0 ? math.PI : 0), 0);
        }
    }
}