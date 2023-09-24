using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

namespace Movement {
    public class PlayerMovement : MonoBehaviour {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionMap _map;
        
        private NavMeshAgent _agent;
        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void OnEnable() {
            _map.Enable();
        }

        private void OnDisable() {
            _map.Disable();
        }

        private void Update() {
            if (_map["Action"].WasPressedThisFrame()) {
                Ray direction = _camera.ScreenPointToRay(_map["Position"].ReadValue<Vector2>());
                Physics.Raycast(direction, out var hit, 100F);
                print("Raycasted");
                if (hit.collider != null) {
                    print("point");
                    var point = hit.point;
                    _agent.destination = point;
                }
            }
        }
    }
}
