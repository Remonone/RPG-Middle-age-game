using System.Collections.Generic;
using RPG.Creatures.AI.Core;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    public class ClearAreaAction : GoapAction {
        [SerializeField] private List<GameObject> _patrolPoints;
        
        private Queue<GameObject> _points = new ();
        private AiVision _vision;

        public ClearAreaAction() {
            _prerequisites.Add(new StateObject {Name = "is_suspicious", Value = false});
            _prerequisites.Add(new StateObject {Name = "is_enemy_visible", Value = false});
            
            _effects.Add(new StateObject {Name = "investigate", Value = true});
        }

        private void Start() {
            _vision = GetComponent<AiVision>();
            foreach (var point in _patrolPoints) {
                _points.Enqueue(point);
            }
        }

        public override bool PerformAction(GameObject agent) {
            
            return true;
        }
        public override void DoReset() {
            InRange = false;
            Target = null;
        }
        public override bool IsDone() {
            return (transform.position - Target.transform.position).magnitude < .3f;
        }
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            Target = _points.Dequeue();
            _points.Enqueue(Target);
            return Target != null;
        }
        public override bool RequiresInRange() {
            return true;
        }
    }
}
