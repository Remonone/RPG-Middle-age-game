using RPG.Combat.Modifiers.BaseTypes;
using RPG.Core.Predicate;
using UnityEngine;

namespace RPG.Combat.Modifiers {
    public abstract class EventModifier : Modification {
        [SerializeField] private string _eventName;
        [SerializeField] private bool _isLocalEvent;
        
        protected PredicateMonoBehaviour Performer;

        
        public override void RegisterModification(GameObject performer) {
            Performer = (PredicateMonoBehaviour)performer.GetComponent(_performerComponent);
            if (_isLocalEvent) {
                
            }
        }
        public override void UnregisterModification() {
        }
    }
}
