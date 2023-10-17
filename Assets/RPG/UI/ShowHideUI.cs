using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RPG.UI {
    public class ShowHideUI : MonoBehaviour {
        [SerializeField] private InputAction _action;
        [SerializeField] private GameObject _objectToSwitch;

        public void OnEnable() {
            _action.Enable();
        }

        public void OnDisable() {
            _action.Disable();
        }

        private void Update() {
            if (_action.WasPerformedThisFrame()) {
                _objectToSwitch.SetActive(!_objectToSwitch.activeSelf);
            }
        }
    
    }
}
