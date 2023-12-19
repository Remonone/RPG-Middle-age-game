using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats.Relations {
    public class RelationObject : MonoBehaviour {
        [SerializeField] private string _id;

        private Dictionary<Organisation, float> _relations;

        private void Awake() {
            
        }
    }
}
