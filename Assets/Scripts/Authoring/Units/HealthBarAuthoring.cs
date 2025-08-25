using Components;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Authoring.Units
{
    public class HealthBarAuthoring : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private Material Material;
        [SerializeField] private float Height;
        [SerializeField] private float Width;
        [Space]
        [SerializeField] private float ScaleFactor;
        [SerializeField] private float3 MinSize;

        [Header("Position")]
        [SerializeField] private float YOffset;
        [SerializeField] private float XOffset;

        public class HealthBarAuthoringBaker : Baker<HealthBarAuthoring>
        {
            public override void Bake(HealthBarAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new HealthBarComponent()
                {
                    ScaleFactor = authoring.ScaleFactor,
                    HealthMaterial = authoring.Material,
                    Size = new float2(authoring.Width,authoring.Height),
                    Offset = new float2(authoring.XOffset, authoring.YOffset),
                    MinSize = authoring.MinSize
                });
            }
        }
    }
}