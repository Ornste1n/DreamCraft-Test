using Components;
using UnityEngine;
using Unity.Entities;
using Authoring.Units;

namespace Authoring.Character
{
    [RequireComponent(typeof(UnitAuthoring))]
    public class CharacterAuthoring : MonoBehaviour
    {
        public class CharacterAuthoringBaker : Baker<CharacterAuthoring>
        {
            public override void Bake(CharacterAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CharacterTag());
            }
        }
    }
}
