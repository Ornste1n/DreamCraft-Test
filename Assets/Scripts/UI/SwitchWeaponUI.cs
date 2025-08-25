using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    // Кнопки для переключения на мобильниках
    public class SwitchWeaponUI : MonoBehaviour
    {
        [SerializeField] private Button _upButton;
        [SerializeField] private Button _downButton;

        public Action<float> OnButtonClicked;
        
        private void Awake()
        {
            _upButton.onClick.AddListener(HandleUpButton);
            _downButton.onClick.AddListener(HandleDownButton);
        }

        private void HandleUpButton() => OnButtonClicked?.Invoke(1);
        private void HandleDownButton() => OnButtonClicked?.Invoke(-1);

        private void OnDestroy()
        {
            _upButton.onClick.RemoveListener(HandleUpButton);
            _downButton.onClick.RemoveListener(HandleDownButton);
        }
    }
}
