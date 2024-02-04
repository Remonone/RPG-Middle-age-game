using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Creatures.AI.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Creatures.AI.Actions {
    public class SearchEnemyTarget : GoapAction {

        [SerializeField] private float _searchAreaRadius = 5F;
        [SerializeField] private GameObject _pointProp;

        private Queue<GameObject> _researchPositions = new();
        
        public SearchEnemyTarget() {
            _prerequisites.Add(new StateObject {Name = "is_suspicious", Value = true});
            _prerequisites.Add(new StateObject {Name = "is_agro", Value = false});
            _prerequisites.Add(new StateObject {Name = "is_enemy_visible", Value = false});
            
            _effects.Add(new StateObject {Name = "check_area", Value = true});
        }

        public override bool PerformAction(GameObject agent) {
            return false;
        }
        public override void DoReset() {
            _researchPositions = null;
            Target = null;
            InRange = false;
        }
        
        public override bool IsDone() {
            return false; // TODO: If player was found;
        }
        
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            foreach (var pos in GetDiscoverPositions()) {
                _researchPositions.Enqueue(pos);
            }

            Target = _researchPositions.Dequeue();
            return true;
        }

        public GameObject[] GetDiscoverPositions() {
            GameObject[] positions = new GameObject[4];
            for (int i = 0; i < 4; i++) {
                Vector3 randomDirection = Random.insideUnitSphere * _searchAreaRadius + transform.position;
                NavMesh.SamplePosition(randomDirection, out var hit, _searchAreaRadius, 1);
                positions[i] = Instantiate(_pointProp, hit.position, Quaternion.identity);
            }

            return positions;
        }
        
        public override bool RequiresInRange() {
            return true;
        }
    }
}
