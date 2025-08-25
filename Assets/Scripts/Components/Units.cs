using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct UnitMoveComponent : IComponentData
    {
        public readonly float MoveSpeed;
        public readonly bool InvertRotation;

        public UnitMoveComponent(float moveSpeed, bool invertRotation)
        {
            MoveSpeed = moveSpeed;
            InvertRotation = invertRotation;
        }
    }
    
    public struct DamageThisFrame : IBufferElementData
    {
        public float Value;

        public DamageThisFrame(float value) => Value = value;
    }
    
    public struct HealthComponent : IComponentData
    {
        public float Health;
        public readonly float MaxHealth;

        public HealthComponent(float health)
        {
            Health = health;
            MaxHealth = health;
        }
    }
    
    // компонент для ренедеринга хп баров
    public struct HealthBarComponent : IComponentData 
    {
        public float2 Size;
        public float2 Offset;
        public float3 MinSize;
        public float ScaleFactor;
        public UnityObjectRef<Material> HealthMaterial;
    }
    
    public struct DestroyUnitFlag : IComponentData { }
}