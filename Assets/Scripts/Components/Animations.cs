using Unity.Entities;
using Unity.Rendering;

namespace Components
{
    [MaterialProperty("_AnimationIndex")]
    public struct UnitAnimationIndexOverride : IComponentData
    {
        public float Value;

        public UnitAnimationIndexOverride(float value) => Value = value;
    }

    public enum AnimationIndexEnum : byte
    {
        Idle = 0,
        Move = 1
    }
}