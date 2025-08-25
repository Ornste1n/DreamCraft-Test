using Components;
using UnityEngine;
using Unity.Entities;

namespace Authoring
{
    public class CanvasAuthoring : MonoBehaviour
    {
        [SerializeField] private string CanvasPath;
        
        public class CanvasAuthoringBaker : Baker<CanvasAuthoring>
        {
            public override void Bake(CanvasAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CanvasRawRefData()
                {
                    Path = authoring.CanvasPath
                });
            }
        }
    }
}