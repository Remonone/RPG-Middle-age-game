using System;
using System.Collections.Generic;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Creatures.AI {
    [Serializable]
    public class AiVision {
        [SerializeField] private int _fieldOfView;
        [SerializeField] private float _visionRange;
        [SerializeField] private MeshCollider _collider;

        public void OnAwake() {
            
        }

        public Dictionary<Organisation, GameObject[]> GetTargetsInVision() {
            return new Dictionary<Organisation, GameObject[]>();
        }
    }
}
