using System.Collections.Generic;
using RPG.Creatures.AI.Core;
using RPG.Creatures.AI.Core.AgentBases;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Creatures.AI.Actions {
    public class SearchEnemyTarget : GoapAction {

        [SerializeField] private float _searchAreaRadius = 5F;
        [SerializeField] private GameObject _pointProp;

        private IFighterAgentBase _agentBase;

        private Queue<GameObject> _researchPositions = new();
        
        public SearchEnemyTarget() {
            _prerequisites.Add(new StateObject {Name = "is_suspicious", Value = true});
            _prerequisites.Add(new StateObject {Name = "is_agro", Value = false});
            _prerequisites.Add(new StateObject {Name = "is_enemy_visible", Value = false});
            
            _effects.Add(new StateObject {Name = "check_area", Value = true});
        }

        public override bool PerformAction(GameObject agent) {
            _researchPositions.TryDequeue(out var point);
            if (point == null) {
                foreach (var pos in GetDiscoverPositions(4)) {
                    _researchPositions.Enqueue(pos);
                }
                point = _researchPositions.Dequeue();
            }
            Destroy(Target);
            if (!ReferenceEquals(_agentBase.GetEnemy(), null)) return true;
            Target = point;
            InRange = false;
            return true;
        }
        public override void DoReset() {
            foreach (var pos in _researchPositions) {
                Destroy(pos);
            }
            Destroy(Target);
            _researchPositions.Clear();
            Target = null;
            InRange = false;
        }
        
        public override bool IsDone() {
            var isDone = _agentBase.GetEnemy() != null || _agentBase.GetSuspiciousTime() < Time.time;
            if (isDone) {
                foreach (var pos in _researchPositions) Destroy(pos);
                _researchPositions.Clear();
            }
            
            return isDone;
        }
        
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            _agentBase = agent.GetComponent<IFighterAgentBase>();
            Target = Instantiate(_pointProp, transform.position, Quaternion.identity);
            return true;
        }

        public GameObject[] GetDiscoverPositions(int count) {
            GameObject[] positions = new GameObject[count];
            for (int i = 0; i < count; i++) {
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
