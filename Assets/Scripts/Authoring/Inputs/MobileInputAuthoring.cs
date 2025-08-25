using UI;
using Components;
using UnityEngine;
using Unity.Entities;

namespace Authoring.Inputs
{
    
    public class MobileInputAuthoring : MonoBehaviour
    {
        [SerializeField] private string JoystickPath;
        [SerializeField] private string SwitchWeaponUIPath;
        
#if UNITY_ANDROID || UNITY_IOS
        public class MobileInputAuthoringBaker : Baker<MobileInputAuthoring>
        {
            public override void Bake(MobileInputAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new MobileRawRefData()
                {
                    JoystickPath = authoring.JoystickPath,
                    SwitchWeaponPath = authoring.SwitchWeaponUIPath
                });
            }
        }
#endif
    }
}
