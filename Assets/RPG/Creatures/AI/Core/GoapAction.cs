using System;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Creatures.AI.Core {
    public abstract class GoapAction : ScriptableObject {

        [SerializeField] private List<KeyPair> _prerequisites;
        [SerializeField] private List<KeyPair> _effects;

        public bool InRange = false;
        
        public float Cost = 1f;
        public GameObject Target;

        public List<KeyPair> Prerequisites => _prerequisites;
        public List<KeyPair> Effects => _effects;

        public abstract bool PerformAction(GameObject agent);

        public abstract void DoReset();

        public abstract bool IsDone();

        public abstract bool CheckProceduralPrerequisites(GameObject agent);

        public abstract bool RequiresInRange();
        

        [Serializable]
        public class KeyPair {
            public string Name;
            public object Value;

            public KeyPair Clone() {
                return new KeyPair { Name = this.Name, Value = this.Value };
            }
        }

    }
}
