using UI;
using Unity.Entities;
using Unity.Collections;

namespace Components
{
    public struct CanvasRawRefData : IComponentData
    {
        public FixedString128Bytes Path;
    }
    
    public struct MobileRawRefData : IComponentData
    {
        public FixedString128Bytes JoystickPath;
        public FixedString128Bytes SwitchWeaponPath;
    }
    
    public struct MobileRefData : IComponentData
    {
        public UnityObjectRef<Joystick> JoystickRef;
        public UnityObjectRef<SwitchWeaponUI> SwitchWeaponRef;
    }
}