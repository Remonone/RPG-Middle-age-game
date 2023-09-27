using Movement;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Control {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionMap _map;
        
        private Mover _mover;

        private void Awake() {
            _mover = GetComponent<Mover>();
        }

        private void OnEnable() {
            _map.Enable();
        }

        private void OnDisable() {
            _map.Disable();
        }

        private void Update() {
            if (_map["Action"].WasPressedThisFrame()) MoveTowardPoint();
        }
        
        
        private void MoveTowardPoint() {
            Ray direction = _camera.ScreenPointToRay(_map["Position"].ReadValue<Vector2>());
            Physics.Raycast(direction, out var hit, 100F);
            if (hit.collider != null) {
                _mover.MoveToPoint(hit.point);
            }
        }
    }
}
