using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring.Inputs
{
    public class InputsAuthoring : MonoBehaviour
    {
        public class InputsAuthoringBaker : Baker<InputsAuthoring>
        {
            public override void Bake(InputsAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new UnitsMoveDirection());
            }
        }
    }
}