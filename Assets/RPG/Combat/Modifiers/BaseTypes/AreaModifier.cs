using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core.Predicate;
using UnityEngine;

namespace RPG.Combat.Modifiers.BaseTypes {
    public abstract class AreaModifier : Modification {

        [SerializeField] private float _range;
        [SerializeField] private LayerMask _layer;
        
        protected PredicateMonoBehaviour Performer;

        
        public override void RegisterModification(GameObject performer) {
            Performer = (PredicateMonoBehaviour)performer.GetComponent(_performerComponent);
            OnRegisterModification(CaptureObjects);
        }


        protected abstract void OnRegisterModification(Func<GameObject, PredicateMonoBehaviour[]> func);
        
        private PredicateMonoBehaviour[] CaptureObjects(GameObject performer) {
            if (ReferenceEquals(Performer, null)) return null;
            var position = performer.transform.position;
            var colliders = GetTargetsInRange(position);
            var objects = colliders.Select(collider => collider.gameObject).ToArray();
            return CollectBehaviours(objects);
            
        }

        private PredicateMonoBehaviour[] CollectBehaviours(GameObject[] targets) {
            List<PredicateMonoBehaviour> predicates = new();
            foreach (var obj in targets) {
                var component = (PredicateMonoBehaviour)obj.GetComponent(_performToComponent);
                if(component == null) continue;
                predicates.Add(component);
            }

            return predicates.ToArray();
        }
        
        public override void UnregisterModification() {
            OnUnregisterModification();
        }
        
        protected abstract void OnUnregisterModification();

        private Collider[] GetTargetsInRange(Vector3 position) {
            return Physics.OverlapSphere(position, _range, _layer);
        }
    }
}
