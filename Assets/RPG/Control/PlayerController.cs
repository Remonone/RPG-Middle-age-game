using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Combat;
using RPG.Core.Predicate;
using RPG.Movement;
using RPG.UI.Cursors;
using RPG.UI.Templates;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace RPG.Control {
    public class PlayerController : PredicateMonoBehaviour {
        [SerializeField] private Camera _camera;
        [SerializeField] private InputActionMap _map;
        [SerializeField] private CursorPreview[] _cursors;
        private Mover _mover;
        private Fighter _fighter;

        [Serializable]
        public class Test { 
            public string test1;
            public int test2;
            public float test3;
            public Vector3 test4;
            public List<string> list;
        }
        [SerializeField] [Foldout] private Test test;

        
        // PUBLIC

        public InputActionMap Map => _map;        
        
        public override void Predicate(string command, object[] arguments, out object result) {
            result = command switch {
                "FindNearByLayer" => FindNearByLayer(arguments),
                "FindNearByTag" => FindNearByTag(arguments),
                _ => null
            };
        }
        
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
        
        // PREDICATES
        
        private object FindNearByTag(object[] args) {
            ValidateArgs(args, typeof(string), typeof(string), typeof(int));
            string array = "[";
            foreach (var obj in Physics.OverlapSphere(transform.position, (float)Convert.ToDouble(args[2]))) {
                if (!obj.gameObject.CompareTag(Convert.ToString(args[0]))) continue;
                var component = obj.GetComponent(Convert.ToString(args[1]));
                if (component.GetType() == typeof(PredicateMonoBehaviour))
                    array += ((PredicateMonoBehaviour)component).ComponentID + ",";
            }
            if (array.Length != 1) array = array.Substring(0, array.Length - 1);
            array += "]";
            
            return array;
        }
        private object FindNearByLayer(object[] args) {
            ValidateArgs(args, typeof(string), typeof(string), typeof(int));
            string array = "[";
            foreach (var obj in Physics.OverlapSphere(transform.position, 
                         (float)Convert.ToDouble(args[2]), LayerMask.NameToLayer(Convert.ToString(args[0])))) {
                
                var component = obj.GetComponent(Convert.ToString(args[1]));
                if (component.GetType() == typeof(PredicateMonoBehaviour))
                    array += ((PredicateMonoBehaviour)component).ComponentID + ",";
            }
            if (array.Length != 1) array = array.Substring(0, array.Length - 1);
            array += "]";
            
            return array;
        }
    }

    [Serializable]
    internal sealed class CursorPreview {
        public CursorType Type;
        public Texture2D Image;
        public Vector2 Hotspot;
    }
}
