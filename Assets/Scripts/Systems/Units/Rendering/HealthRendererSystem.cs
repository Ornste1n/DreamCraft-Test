using Utilities;
using Components;
using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Components.Aspects;

namespace Systems.Units.Rendering
{
    // Написал шейдер и систему для рендеринга хп бара
    // Задача была оптимизировать количество вызова отрисовок
    // Собирает все необходимые данные (матрицу преобразований, хп)
    // Можно было сделать по-другому, GPU-only, но быстрее сделал это
    
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial class UnitsHealthRendererSystem : SystemBase
    {
        #region Material Properties
        
        private Mesh _mesh;
        private Material _healthBarMaterial;
        private MaterialPropertyBlock _materialProperty;
        
        private const int MaxInstancesPerBatch = 1023;
        private static readonly int s_fill = Shader.PropertyToID("_Fill");
        private static readonly int s_meshSize = Shader.PropertyToID("_MeshSize");
        private static readonly int s_fillColor = Shader.PropertyToID("_FillColor");
        
        #endregion

        #region Render Properties
        
        private float2 _offset;
        private float3 _minSize;
        private float _scaleFactor;

        private float _ortSize;
        private Vector3 _eulerRotation;

        private NativeArray<float> _fills;
        private NativeArray<Matrix4x4> _matrices;
        
        #endregion
        
        protected override void OnCreate()
        {
            _materialProperty = new MaterialPropertyBlock();
            _fills = new NativeArray<float>(MaxInstancesPerBatch, Allocator.Persistent);
            _matrices = new NativeArray<Matrix4x4>(MaxInstancesPerBatch, Allocator.Persistent);
            
            RequireForUpdate<HealthBarComponent>();
            RequireForUpdate<CameraTransformComponent>();
        }

        protected override void OnStartRunning()
        {
            HealthBarComponent barComponent = SystemAPI.GetSingleton<HealthBarComponent>();

            Camera camera = Camera.main!;
            _eulerRotation = camera.transform.eulerAngles;
            _ortSize = camera.orthographicSize;
            
            _offset = barComponent.Offset;
            _minSize = barComponent.MinSize;
            _scaleFactor = barComponent.ScaleFactor;
            _healthBarMaterial = barComponent.HealthMaterial.Value;
            _mesh = MeshUtility.CreateQuadMesh(barComponent.Size.x, barComponent.Size.y);
            
            _healthBarMaterial.SetVector(s_meshSize, new Vector2(barComponent.Size.x, barComponent.Size.y));
        }

        protected override void OnUpdate()
        {
            float2 offset = _offset;
            float3 minSize = _minSize;
            float scaleFactor = _scaleFactor;

            Quaternion barRotation = Quaternion.Euler(_eulerRotation);
            float3 cameraPosition = SystemAPI.GetSingleton<CameraTransformComponent>().Position;

            float barScaleFactor = math.tan(_ortSize * Mathf.Deg2Rad * 0.5f);

            int count = 0;
            NativeArray<float> fills = _fills;
            NativeArray<Matrix4x4> matrices = _matrices;
            
            // прохожусь по всем активным юнитам на сцене
            foreach (UnitAspect unit in SystemAPI.Query<UnitAspect>())
            {
                if (SystemAPI.HasComponent<Disabled>(unit.Entity)) continue;
                
                if(unit.Health <= 0) continue;

                Vector3 barPosition = unit.Position;
                barPosition.x += offset.x;
                barPosition.y += offset.y;

                float distance = math.distance(cameraPosition, barPosition);
                float3 scale = math.max(distance * Vector3.one * barScaleFactor * scaleFactor, minSize);
                
                // заношу в массивы данные
                fills[count] = unit.Health / unit.MaxHealth;
                matrices[count++] = Matrix4x4.TRS(barPosition, barRotation, scale);

                if (count != MaxInstancesPerBatch) continue;
                
                DrawHealthBatch(matrices, fills, count);
                count = 0;
            }

            if(count > 0)
                DrawHealthBatch(matrices, fills, count);
        }

        // Отрисовываю за один вызов count мешей с разными Matrix4x4 и float (хп)
        private void DrawHealthBatch(NativeArray<Matrix4x4> matrices, NativeArray<float> fills, int count)
        {
            _materialProperty.Clear();
            _materialProperty.SetFloatArray(s_fill, fills.GetSubArray(0, count).ToArray());

            RenderParams render = new RenderParams(_healthBarMaterial)
            {
                matProps = _materialProperty,
            };
            
            Graphics.RenderMeshInstanced(render, _mesh, 0, matrices, count);
        }

        protected override void OnDestroy()
        {
            _fills.Dispose();
            _matrices.Dispose();
        }
    }
}