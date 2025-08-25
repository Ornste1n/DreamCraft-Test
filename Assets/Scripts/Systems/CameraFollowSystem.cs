using Components;
using Unity.Burst;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Components.Aspects;
using ComponentSystemGroups;

namespace Systems
{
    [UpdateInGroup(typeof(CameraInitializationSystemsGroup))]
    public partial struct CameraInitSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraTransformComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Camera camera = Camera.main!;
            ref CameraTransformComponent transform = ref SystemAPI.GetSingletonRW<CameraTransformComponent>().ValueRW;
            transform.HalfHeight = camera.orthographicSize;
            transform.HalfWidth = camera.orthographicSize * camera.aspect;
            
            state.Enabled = false;
        }
    }
    
    [BurstCompile]
    [UpdateInGroup(typeof(CameraSimulationSystemsGroup))]
    public partial struct CameraFollowSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CharacterTag>();
            state.RequireForUpdate<CameraMoveConfig>();
        }

        /// Записываем позицию игрока в CameraTransformComponent
        public void OnUpdate(ref SystemState state)
        {
            Entity characterEntity = SystemAPI.GetSingletonEntity<CharacterTag>(); 
            UnitAspect aspect = SystemAPI.GetAspect<UnitAspect>(characterEntity);

            ref CameraTransformComponent transform = ref SystemAPI.GetSingletonRW<CameraTransformComponent>().ValueRW;
            transform.Position = aspect.Position;
        }
    }

    /// Класс синхронизирует gameObject камеру с сущностью игрока засчет CameraTransformComponent
    [UpdateInGroup(typeof(CameraPresentationSystemsGroup))]
    public partial struct SyncCameraObjectSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<CameraMoveConfig>();
            state.RequireForUpdate<CameraTransformComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            Camera camera = Camera.main!;
            CameraTransformComponent camComp = SystemAPI.GetSingleton<CameraTransformComponent>();
            Transform transform = camera.transform;

            float3 cur = camComp.Position;
            transform.position = new Vector3(cur.x, cur.y, transform.position.z);
        }
    }
}