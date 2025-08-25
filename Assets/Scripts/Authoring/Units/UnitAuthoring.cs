using Components;
using UnityEngine;
using Unity.Entities;

namespace Authoring.Units
{
    public class UnitAuthoring : MonoBehaviour
    {
        [SerializeField] private float Health;

        [Header("Movement")]
        [SerializeField] private float MoveSpeed;
        [SerializeField] private bool InvertRotation;
        
        public class UnitAnimationBaker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                
                AddComponent(entity, new HealthComponent(authoring.Health));
                
                AddComponent(entity, new UnitAnimationIndexOverride(1));
                
                AddComponent(entity, new UnitsMoveDirection());
                AddComponent(entity, new UnitMoveComponent(authoring.MoveSpeed, authoring.InvertRotation));

                AddBuffer<DamageThisFrame>(entity);
            }
        }
    }
}