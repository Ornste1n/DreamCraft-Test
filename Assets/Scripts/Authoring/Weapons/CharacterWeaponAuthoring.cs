using Components;
using UnityEngine;
using Unity.Entities;

namespace Authoring.Weapons
{
    public class CharacterWeaponAuthoring : MonoBehaviour
    {
        public class CharacterWeaponAuthoringBaker : Baker<CharacterWeaponAuthoring>
        {
            public override void Bake(CharacterWeaponAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new WeaponTag());
                AddComponent(entity, new CharacterWeaponComponent { CurrentId = 0 });
            }
        }
    }
}