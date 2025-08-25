using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Authoring.Character
{
    public class InvincibilityEffectAuthoring : MonoBehaviour
    {
        [SerializeField] private float BlinkSpeed;
        [SerializeField] private Color EffectColor;
        
        public class InvincibilityEffectAuthoringBaker : Baker<InvincibilityEffectAuthoring>
        {
            public override void Bake(InvincibilityEffectAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity,
                    new InvincibilityEffectComponent(authoring.BlinkSpeed, authoring.EffectColor));
            }
        }
    }

    public struct InvincibilityEffectComponent : IComponentData
    {
        public float BlinkSpeed;
        public float4 EffectColor;

        public InvincibilityEffectComponent(float speed, Color color)
        {
            BlinkSpeed = speed;
            EffectColor = new float4(color.r, color.g, color.b, color.a);
        }
    }
}