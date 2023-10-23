using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace RPG.Core.Predicate {
    public abstract class PredicateMonoBehaviour : MonoBehaviour {

        [ReadOnly] [SerializeField] private string _componentID;

        private void Awake() {
            if (string.IsNullOrWhiteSpace(_componentID)) {
                _componentID = System.Guid.NewGuid().ToString();
            }
            OnAwake();
        }

        protected abstract void OnAwake();

        public abstract void Predicate((string command, object[] arguments)[] predicates, out List<object> results);
    }
}
