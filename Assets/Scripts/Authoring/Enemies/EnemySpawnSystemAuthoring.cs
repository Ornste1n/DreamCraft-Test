using UnityEngine;
using Unity.Entities;

namespace Authoring.Enemies
{
    public class EnemySpawnSystemAuthoring : MonoBehaviour
    {
        [SerializeField] private float SpawnDelay = 2f;
        
        [Header("Location Bounds")]
        [SerializeField] private float Left;
        [SerializeField] private float Right;
        [SerializeField] private float Top;
        [SerializeField] private float Bottom;
        
        public class EnemySpawnSystemAuthoringBaker : Baker<EnemySpawnSystemAuthoring>
        {
            public override void Bake(EnemySpawnSystemAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EnemySpawnComponent
                {
                    Timer = authoring.SpawnDelay,
                    SpawnDelay = authoring.SpawnDelay,
                    LeftBound = authoring.Left,
                    RightBound = authoring.Right,
                    TopBound = authoring.Top,
                    BottomBound = authoring.Bottom,
                });
            }
        }
    }
    
    public struct EnemySpawnComponent : IComponentData
    {
        public float SpawnDelay;
        public float Timer;
        
        public float LeftBound;
        public float RightBound;
        public float TopBound;
        public float BottomBound;
    }
}