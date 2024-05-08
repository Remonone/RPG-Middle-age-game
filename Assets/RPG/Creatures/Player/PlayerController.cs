using System;
using System.Linq;
using RPG.Core.Cursors;
using RPG.Movement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RPG.Creatures.Player {
    
    [RequireComponent(typeof(CameraBehaviour))]
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionMap _map;
        [SerializeField] private CursorPreview[] _cursors;
        
        private Mover _mover;
        private CameraBehaviour _cameraBehaviour;
        
        // PUBLIC
        public InputActionMap Map => _map;        
        
        // PRIVATE

        private void Awake() {
            _mover = GetComponent<Mover>();
            _cameraBehaviour = GetComponent<CameraBehaviour>();
            _cameraBehaviour.Init();
        }

        private void OnEnable() {
            _map.Enable();
        }
        
        private void OnDisable() {
            _map.Disable();
        }

        private void Update() {
            if (InteractWithCamera()) return;
            if (InteractWithUI()) return;
            if (InteractWithComponent()) return;
            if (MoveTowardPoint()) return;
            SetCursor(CursorType.EMPTY);
        }
        
        private bool InteractWithCamera() {
            if (!_map["Camera Rotation"].IsPressed()) return false;
            _cameraBehaviour.RotateCamera(_map["Mouse Delta"].ReadValue<Vector2>());
            return true;
        }
        
        private bool InteractWithUI() {
            var isOverUI = EventSystem.current.IsPointerOverGameObject();
            var shouldActive = isOverUI && _map["Action"].WasPressedThisFrame();
            if (shouldActive) SetCursor(CursorType.UI);
            return shouldActive;
        }

        private bool InteractWithComponent() {
            var hits = SortedRaycast();
            foreach (var hit in hits) {
                var raycastables = hit.transform.GetComponents<ITrajectory>();
                foreach (var raycastable in raycastables) {
                    SetCursor(raycastable.GetCursorType());
                    if (raycastable.HandleRaycast(this)) return true;
                }
            }
            return false;
        }

        private RaycastHit[] SortedRaycast() {
            var hits = Physics.RaycastAll(GetMouseRay());
            var distances = new float[hits.Length];
            for (var i = 0; i < distances.Length; i++) {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }
        
        private Ray GetMouseRay() => _camera.ScreenPointToRay(_map["Position"].ReadValue<Vector2>());
        
        private bool MoveTowardPoint() {
            Ray direction = GetMouseRay();
            Physics.Raycast(direction, out var hit, 100F);
            if (hit.collider == null) {
                SetCursor(CursorType.EMPTY);
                return false;
            }
            SetCursor(CursorType.MOVEMENT);
            if (_map["Action"].WasPressedThisFrame()) _mover.StartMovingToPoint(hit.point);;
            return true;
        }
        
        private void SetCursor(CursorType type) {
            var cursor = _cursors.Single(cursor => cursor.Type == type);
            Cursor.SetCursor(cursor.Image, cursor.Hotspot, CursorMode.Auto);
        }
    }

    [Serializable]
    internal sealed class CursorPreview {
        public CursorType Type;
        public Texture2D Image;
        public Vector2 Hotspot;
    }
}
