using Components;
using UnityEngine;
using Unity.Entities;

namespace Authoring
{
    public class CameraAuthoring : MonoBehaviour
    {
        [SerializeField] private float FollowSpeed;
        
        public class CameraAuthoringBaker : Baker<CameraAuthoring>
        {
            public override void Bake(CameraAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CameraTransformComponent());
                AddComponent(entity, new CameraMoveConfig(authoring.FollowSpeed));
            }
        }
    }
}