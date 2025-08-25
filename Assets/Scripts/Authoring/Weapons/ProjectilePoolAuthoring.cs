using Components;
using UnityEngine;
using Unity.Entities;

namespace Authoring.Weapons
{
    public class ProjectilePoolAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject ProjectilePrefab;
        [SerializeField] private int PoolSize;
        [SerializeField] private int MaxPoolSize;
        [SerializeField] private int GrowPerFrame;
        
        public class ProjectilePoolAuthoringBaker : Baker<ProjectilePoolAuthoring>
        {
            public override void Bake(ProjectilePoolAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new ProjectilePoolData()
                {
                    CurrentIndex = 0,
                    PoolSize = authoring.PoolSize,
                    MaxPoolSize = authoring.MaxPoolSize,
                    GrowPerFrame = authoring.GrowPerFrame,
                    ProjectilePrefab = GetEntity(authoring.ProjectilePrefab, TransformUsageFlags.Dynamic)
                });
            }
        }
    }
}