using System.Collections.Generic;
using RPG.Creatures.AI.Core;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    public class ClearAreaAction : GoapAction {
        [SerializeField] private List<GameObject> _patrolPoints;
        [SerializeField] private AiVision _vision;
        
        private readonly Queue<GameObject> _points = new ();

        public ClearAreaAction() {
            _prerequisites.Add(new StateObject {Name = "is_suspicious", Value = false});
            _prerequisites.Add(new StateObject {Name = "is_enemy_visible", Value = false});
            
            _effects.Add(new StateObject {Name = "investigate", Value = true});
        }

        public override bool PerformAction(GameObject agent) {
            _points.TryDequeue(out var point);
            if (point == null) return false;
            if (_vision.IsEnemiesInVision) return false;
            Target = point;
            InRange = false;
            return true;
        }
        
        public override void DoReset() {
            InRange = false;
            Target = null;
            _points.Clear();
        }
        
        public override bool IsDone() {
            return _points.Count < 1;
        }
        
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            foreach (var point in _patrolPoints) {
                _points.Enqueue(point);
            }
            _points.TryDequeue(out var patrolPoint);
            Target = patrolPoint;
            return Target != null;
        }
        
        public override bool RequiresInRange() {
            return true;
        }
    }
}
