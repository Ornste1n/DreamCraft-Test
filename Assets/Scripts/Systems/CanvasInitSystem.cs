using UI;
using Components;
using UnityEngine;
using Unity.Entities;
using ScriptableObjects;

namespace Systems
{
    /// Система создает Canvas и дополнительные элементы, учитывая устройство
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial class CanvasInitSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<CanvasRawRefData>();
#if UNITY_ANDROID || UNITY_IOS
            RequireForUpdate<MobileRawRefData>();
#endif
        }

        protected override void OnUpdate()
        {
            Entity canvasSingleton = SystemAPI.GetSingletonEntity<CanvasRawRefData>();
            CanvasRawRefData canvasRefData = SystemAPI.GetComponent<CanvasRawRefData>(canvasSingleton);
            
            Canvas canvasRef = Resources.Load<Canvas>(canvasRefData.Path.ToString());
            Canvas canvas = Object.Instantiate(canvasRef).GetComponent<Canvas>();
#if UNITY_ANDROID || UNITY_IOS
            Entity mobileSingleton = SystemAPI.GetSingletonEntity<MobileRawRefData>();
            MobileRawRefData mobileRefData = SystemAPI.GetComponent<MobileRawRefData>(mobileSingleton);
            
            FixedJoystick joystickRef = Resources.Load<FixedJoystick>(mobileRefData.JoystickPath.ToString());
            FixedJoystick joystick = Object.Instantiate(joystickRef).GetComponent<FixedJoystick>();
            
            SwitchWeaponUI weaponUIRef = Resources.Load<SwitchWeaponUI>(mobileRefData.SwitchWeaponPath.ToString());
            SwitchWeaponUI weaponUI = Object.Instantiate(weaponUIRef).GetComponent<SwitchWeaponUI>();
            
            EntityManager.AddComponentData(mobileSingleton, new MobileRefData()
            {
                JoystickRef = joystick,
                SwitchWeaponRef = weaponUI
            });
            
            joystick.gameObject.transform.SetParent(canvas.transform);
            weaponUI.gameObject.transform.SetParent(canvas.transform, false);
#endif
            
            Enabled = false;
        }
    }
}