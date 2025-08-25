using Components;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace Authoring.Enemies
{
    public class EnemiesBufferAuthoring : MonoBehaviour
    {
        [SerializeField] private GameObject[] EnemyPrefabs;
        [SerializeField] private int PoolSizePerPrefab;
        [SerializeField] private int MaxPoolSize;
        
        public class ForestFactoryBaker : Baker<EnemiesBufferAuthoring>
        {
            public override void Bake(EnemiesBufferAuthoring authoring)
            {
                foreach (GameObject prefab in authoring.EnemyPrefabs)
                {
                    Entity poolEntity = CreateAdditionalEntity(TransformUsageFlags.None);

                    AddComponent(poolEntity, new EnemyPoolSettings
                    {
                        Prefab = GetEntity(prefab, TransformUsageFlags.Dynamic),
                        PoolSize = math.max(1, authoring.PoolSizePerPrefab),
                        MaxPoolSize = authoring.MaxPoolSize
                    });

                    AddBuffer<EnemyPoolElement>(poolEntity);
                }
            }
        }
    }
}