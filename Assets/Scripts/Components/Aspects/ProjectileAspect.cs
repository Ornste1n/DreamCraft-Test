using Unity.Physics;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Components.Aspects
{
    /// <summary>
    /// Аспект для группировки всех пуль
    /// </summary>
    public readonly partial struct ProjectileAspect : IAspect
    {
        public readonly Entity Entity;

        private readonly RefRW<ProjectileData> m_MoveComponent;
        
        private readonly RefRW<PhysicsVelocity> m_Physics;
        private readonly RefRW<LocalTransform> m_Transform;

        #region Properties
        public ProjectileData Data 
        {
            get => m_MoveComponent.ValueRO;
            set => m_MoveComponent.ValueRW = value;
        }

        public float3 Position => m_Transform.ValueRO.Position;

        public float3 Linear
        {
            get => m_Physics.ValueRO.Linear;
            set => m_Physics.ValueRW.Linear = value;
        }
        #endregion
    }
}