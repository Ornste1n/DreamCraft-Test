using Components;
using Unity.Burst;
using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

namespace Systems.Weapons
{
    // Обрабатывает ввод смены оружия
    // Обновляет вид оружия у героя
    [BurstCompile]
    public partial struct WeaponUpdateSystem : ISystem
    {
        private float _lastValue;
        
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<WeaponTag>();
            state.RequireForUpdate<ScrollMouseComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            ref ScrollMouseComponent scroll = ref SystemAPI.GetSingletonRW<ScrollMouseComponent>().ValueRW;
            
            if(scroll.Value == _lastValue || scroll.Value == 0) return;
            
            var weaponMaterials = SystemAPI.GetSingletonBuffer<WeaponMaterialsElement>(); // буфер материалов
            ref var weaponComponent = ref SystemAPI.GetSingletonRW<CharacterWeaponComponent>().ValueRW;
            Entity characterWeapon = SystemAPI.GetSingletonEntity<WeaponTag>();

            int weaponCount = weaponMaterials.Length;
            int delta = scroll.Value > 0 ? 1 : -1;
            
            weaponComponent.CurrentId = (weaponComponent.CurrentId + delta + weaponCount) % weaponCount; // индекс нужного оружия в буфере
            
            var meshInfo = state.EntityManager.GetComponentData<MaterialMeshInfo>(characterWeapon);
            var rma = state.EntityManager.GetSharedComponentManaged<RenderMeshArray>(characterWeapon);

            rma.MaterialReferences[MaterialMeshInfo.StaticIndexToArrayIndex(meshInfo.Material)] 
                = weaponMaterials[weaponComponent.CurrentId].Material; // устанавливаю новый материал
            
            //обновляю цвет, чтобы система отредерила изменения, по-другому никак не получалось
            URPMaterialPropertyBaseColor newColor = new() { Value = new float4(1,1,1,1f) }; 

            if (!state.EntityManager.HasComponent<URPMaterialPropertyBaseColor>(characterWeapon))
                state.EntityManager.AddComponentData(characterWeapon, newColor);
            else
                state.EntityManager.SetComponentData(characterWeapon, newColor);
            
            scroll.Value = 0f;
            _lastValue = scroll.Value;
        }
    }
}