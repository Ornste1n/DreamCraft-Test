using System;
using System.Collections.Generic;
using UI;
using Components;
using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Components.Aspects;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using MouseClickEvent = Components.MouseClickEvent;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

namespace Systems.Input
{
    /// Система обработки ввода в зависимости от устройства
    public partial class PlayerInputSystem : SystemBase
    {
        private Entity _entityInput;
        private InputControls _inputControls;
        private Plane _plane = new(Vector3.forward, Vector3.zero);

#if UNITY_IOS || UNITY_ANDROID
        private Joystick _joystick;
        private SwitchWeaponUI _switchWeaponUI;
#endif
        
        protected override void OnCreate()
        {
            _inputControls = new InputControls();
            _inputControls.Enable();
            
            _entityInput = EntityManager.CreateEntity();
            _inputControls.Player.Fire.performed += HandleMouseClick;
            EntityManager.AddComponent<ScrollMouseComponent>(_entityInput);
            
            RequireForUpdate<CharacterTag>();
            
#if UNITY_IOS || UNITY_ANDROID
            RequireForUpdate<MobileRefData>();
#elif UNITY_STANDALONE
            _inputControls.Player.SwitchWeapon.performed += HandleMouseScroll;
#endif
        }

#if UNITY_IOS || UNITY_ANDROID
        protected override void OnStartRunning()
        {
            MobileRefData mobile = SystemAPI.GetSingleton<MobileRefData>();
            _joystick = mobile.JoystickRef.Value;
            _switchWeaponUI = mobile.SwitchWeaponRef.Value;
            
            _switchWeaponUI.OnButtonClicked += HandleSwitchWeaponUI;
        }

        private void HandleSwitchWeaponUI(float value)
        {
            ref ScrollMouseComponent scroll = ref SystemAPI.GetSingletonRW<ScrollMouseComponent>().ValueRW;
            scroll.Value = value;
        }
#elif UNITY_STANDALONE
        private void HandleMouseScroll(InputAction.CallbackContext context)
        {
            float scrollValue = context.ReadValue<float>();
            if (math.abs(scrollValue) < 0.1f) return;

            ref ScrollMouseComponent scroll = ref SystemAPI.GetSingletonRW<ScrollMouseComponent>().ValueRW;
            scroll.Value = scrollValue;
        }
#endif

        protected override void OnUpdate()
        {
            float2 moveInputValue = float2.zero;
#if UNITY_STANDALONE
            moveInputValue = _inputControls.Player.Move.ReadValue<Vector2>();
#elif UNITY_IOS || UNITY_ANDROID
            moveInputValue = _joystick.Direction;
#endif
            Entity characterEntity = SystemAPI.GetSingletonEntity<CharacterTag>();
            UnitAspect unitAspect = SystemAPI.GetAspect<UnitAspect>(characterEntity);
            
            unitAspect.MoveDirection = moveInputValue;
        }

        private void HandleMouseClick(InputAction.CallbackContext ctx)
        {
            Vector2 position = _inputControls.Player.PointerPosition.ReadValue<Vector2>();
            EntityManager.AddComponentData(_entityInput, new MouseClickEvent()
            {
                WorldPosition = GetWorldPosition(Camera.main, position)
            });
        }

        private float3 GetWorldPosition(Camera camera, Vector3 position)
        {
            Ray ray = camera.ScreenPointToRay(position);

            if (_plane.Raycast(ray, out float enter))
                return ray.GetPoint(enter);

            return float3.zero;
        }

        protected override void OnDestroy()
        {
#if UNITY_STANDALONE
            _inputControls.Player.SwitchWeapon.performed -= HandleMouseScroll;
#elif UNITY_IOS || UNITY_ANDROID
            _switchWeaponUI.OnButtonClicked -= HandleSwitchWeaponUI;
#endif
            _inputControls.Player.Fire.performed -= HandleMouseClick;
            _inputControls.Dispose();
        }
    }
}
