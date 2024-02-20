using System;
using RPG.Core.Predicate.Interfaces;
using RPG.Utils;
using UnityEngine;

namespace RPG.Core.Predicate {
    public class PredicateMonoBehaviour : MonoBehaviour {

        [ReadOnly] [SerializeField] private string _entityID;

        public string EntityID => _entityID;

        private void Awake() {
            if (string.IsNullOrWhiteSpace(_entityID)) {
                _entityID = "E_" + Guid.NewGuid().ToString().Replace('-', '_');
            }
        }

        private void OnEnable() {
            PredicateWorker.RegisterPredicate(_entityID, this);
        }

        private void OnDestroy() {
            PredicateWorker.DestroyPredicate(_entityID);
        }

        public bool TryGetHandler(string handlerName, out IPredicateHandler handler) {
            handler = GetComponent(handlerName) as IPredicateHandler;
            return !ReferenceEquals(handler, null);
        }
        
    }
}
