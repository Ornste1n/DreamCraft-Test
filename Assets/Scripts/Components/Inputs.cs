using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct UnitsMoveDirection : IComponentData
    {
        public float2 Value;
    }

    public struct MouseClickEvent : IComponentData
    {
        public float3 WorldPosition;
    }

    public struct ScrollMouseComponent : IComponentData
    {
        public float Value;
    }
}