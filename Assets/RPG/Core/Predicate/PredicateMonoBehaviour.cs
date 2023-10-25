using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace RPG.Core.Predicate {
    public abstract class PredicateMonoBehaviour : MonoBehaviour {

        [ReadOnly] [SerializeField] private string _componentID;

        public string ComponentID => _componentID;

        private void Awake() {
            if (string.IsNullOrWhiteSpace(_componentID)) {
                _componentID = "C_" + Guid.NewGuid().ToString().Replace('-', '_');
            }
            OnAwake();
        }

        private void OnEnable() {
            PredicateWorker.RegisterPredicate(_componentID, this);
            OnEnableEvent();
        }

        private void OnDestroy() {
            PredicateWorker.DestroyPredicate(_componentID);
            OnDestroyEvent();
        }
        
        // TODO: Might be changable...
        protected virtual void OnAwake() {}
        protected virtual void OnEnableEvent() {}
        protected virtual void OnDestroyEvent() {}
        public abstract void Predicate(string command, object[] arguments, out object result);
    }
}
