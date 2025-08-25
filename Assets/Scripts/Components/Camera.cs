using Unity.Collections;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

namespace Components
{

    public struct CameraMoveConfig : IComponentData
    {
        public readonly float FollowSpeed;

        public CameraMoveConfig(float followSpeed)
        {
            FollowSpeed = followSpeed;
        }
    }

    public struct CameraTransformComponent : IComponentData
    {
        public float3 Position;
        
        public float HalfWidth;
        public float HalfHeight;
    }
}