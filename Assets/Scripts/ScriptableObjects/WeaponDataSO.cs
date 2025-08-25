using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(menuName = "ECS/Weapon Data")]
    public class WeaponDataSo : ScriptableObject // конфигурация оружия
    {
        [field: SerializeField] public Material WeaponMaterial;
        [field: SerializeField] public float Damage { get; private set; }
        [field: SerializeField] public float Speed { get; private set; }
        [field: SerializeField] public float Lifetime { get; private set; }
        [field: SerializeField] public int Count { get; private set; }
        [field: SerializeField] public float SpreadAngle { get; private set; }
        [field: SerializeField] public float SineAmplitude { get; private set; }
        [field: SerializeField] public float SineFrequency { get; private set; }
    }
}