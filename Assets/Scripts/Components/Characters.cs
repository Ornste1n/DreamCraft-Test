using Unity.Entities;
using Unity.Rendering;
using Unity.Mathematics;

namespace Components
{
    public struct CharacterTag : IComponentData { }
    
    public struct CharacterWeaponComponent : IComponentData
    {
        public int CurrentId;
    }
    
    public struct InvincibilityFrame : IComponentData
    {
        public float Seconds;

        public InvincibilityFrame(float seconds) => Seconds = seconds;
    }
    
    [MaterialProperty("_BaseColor")] // для анимации урона
    public struct InvincibilityBaseColor : IComponentData
    {
        public float4 Value;
    }
}