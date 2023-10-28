using System;
using RPG.Utils;
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

        protected bool ValidateArgs(object[] args, params Type[] expected) {
            if (args.Length != expected.Length) return false;
            // var counter = 0;
            // foreach (var arg in args) {
            //     Debug.Log(arg + " " + expected[counter]);
            //     var stringValue = Convert.ToString(arg);
            //     counter++;
            //     if(stringValue.GetTypeCode().ToString() != expected.ToString()) 
            //         Debug.LogError($"Argument {arg} of type {stringValue.GetTypeCode().ToString()} type mismatch. Expected: {expected[counter - 1]}.");
            // } NOT WORKING

            return true;
        }
        
        // TODO: Might be changable...
        protected virtual void OnAwake() {}
        protected virtual void OnEnableEvent() {}
        protected virtual void OnDestroyEvent() {}
        public abstract void Predicate(string command, object[] arguments, out object result);
    }
}
