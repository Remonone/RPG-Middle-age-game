using System;
using System.Linq;
using RPG.Combat;
using RPG.Movement;
using RPG.UI.Cursors;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RPG.Creatures.Controls {
    public class PlayerController : MonoBehaviour {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionMap _map;
        [SerializeField] private CursorPreview[] _cursors;
        
        private Mover _mover;
        private Fighter _fighter;
        
        // PUBLIC
        public InputActionMap Map => _map;        
        
        
        // PRIVATE

        private void Awake() {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
        }

        private void OnEnable() {
            _map.Enable();
        }
        
        private void OnDisable() {
            _map.Disable();
        }

        private void Update() {
            if (InteractWithUI()) return;
            if (InteractWithComponent()) return;
            if (MoveTowardPoint()) return;
            SetCursor(CursorType.EMPTY);
        }
        private bool InteractWithUI() {
            var isOverUI = EventSystem.current.IsPointerOverGameObject();
            var shouldActive = isOverUI && _map["Action"].WasPressedThisFrame();
            if (shouldActive) SetCursor(CursorType.UI);
            return shouldActive;
        }

        private bool InteractWithComponent() {
            if (!_map["Action"].WasPressedThisFrame()) return false;
            var hits = SortedRaycast();
            if (hits.Length < 1) return false;
            foreach (var hit in hits) {
                var selectable = hit.collider.GetComponent<SelectableEnemy>();
                if (selectable == null) continue;
                _fighter.Attack(selectable);
                return true;
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
            if (!_map["Action"].WasPressedThisFrame()) return false;
            Ray direction = GetMouseRay();
            Physics.Raycast(direction, out var hit, 100F);
            if (hit.collider != null) {
                _mover.StartMovingToPoint(hit.point);
                SetCursor(CursorType.MOVEMENT);
                return true;
            }
            return false;
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
