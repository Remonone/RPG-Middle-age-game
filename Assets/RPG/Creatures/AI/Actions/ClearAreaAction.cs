﻿using System.Collections.Generic;
using RPG.Creatures.AI.Core;
using RPG.Creatures.AI.Core.AgentBases;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    public class ClearAreaAction : GoapAction {
        [SerializeField] private List<GameObject> _patrolPoints;
        
        private readonly Queue<GameObject> _points = new ();

        private IFighterAgentBase _fighterAgent;

        public ClearAreaAction() {
            _prerequisites.Add(new StateObject {Name = "is_suspicious", Value = false});
            _prerequisites.Add(new StateObject {Name = "is_enemy_visible", Value = false});
            
            _effects.Add(new StateObject {Name = "investigate", Value = true});
        }

        public override bool PerformAction(GameObject agent) {
            _points.TryDequeue(out var point);
            if (point == null) return false;
            if (!ReferenceEquals(_fighterAgent.GetEnemy(), null)) return false;
            Target = point;
            InRange = false;
            return true;
        }
        
        public override void DoReset() {
            InRange = false;
            Target = null;
            _fighterAgent = null;
            _points.Clear();
        }
        
        public override bool IsDone() {
            return _points.Count < 1;
        }
        
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            _fighterAgent = agent.GetComponent<IFighterAgentBase>();
            if (_fighterAgent == null) return false;
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
