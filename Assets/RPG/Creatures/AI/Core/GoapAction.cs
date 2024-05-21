using System;
using System.Collections.Generic;
using RPG.Utils;
using Unity.Netcode;
using UnityEngine;

namespace RPG.Creatures.AI.Core {
    public abstract class GoapAction : NetworkBehaviour {

        [SerializeField] protected List<StateObject> _prerequisites;
        [SerializeField] protected List<StateObject> _effects;
        
        public bool InRange;
        
        public float Cost = 1f;
        [ReadOnly] public GameObject Target;

        
        public GoapAction() {
            _prerequisites = new List<StateObject>();
            _effects = new List<StateObject>();
        }

        public List<StateObject> Prerequisites => _prerequisites;
        public List<StateObject> Effects => _effects;

        public abstract bool PerformAction(GameObject agent);

        public abstract void DoReset();

        public abstract bool IsDone();

        public abstract bool CheckProceduralPrerequisites(GameObject agent);

        public abstract bool RequiresInRange();

    }
    
    [Serializable]
    public class StateObject {
        public string Name;
        public object Value;

        public StateObject Clone() {
            return new StateObject { Name = this.Name, Value = this.Value };
        }

        public bool IsEqual(StateObject dif) {
            return this.Name == dif.Name && Equals(Value, dif.Value);
        }
        public bool IsEqualState(StateObject dif) {
            return Name == dif.Name;
        }
    }
}
