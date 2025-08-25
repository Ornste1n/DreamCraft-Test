using UnityEngine;
using Unity.Entities;
using Unity.Rendering;
namespace Components
{
    public struct WeaponMaterialsElement : IBufferElementData
    {
        public UnityObjectRef<Material> Material; // для смены view оружия
    }
    
    public struct WeaponTag : IComponentData { }
}