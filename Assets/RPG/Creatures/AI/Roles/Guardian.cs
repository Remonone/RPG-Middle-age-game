using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Creatures.AI.Core;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Creatures.AI.Roles {
    [RequireComponent(typeof(Health))]
    public class Guardian : BaseAgentBehaviour {

        [SerializeField] private AiVision _vision;
        [SerializeField] private Organisation _organisation;
        [SerializeField] private float _agroDuration;

        private float _agroTime = -1;
        private Health _target;
        private Health _health;

        private void Awake() {
            _health = GetComponent<Health>();
        }

        public override List<StateObject> GetCurrentState() {
            List<StateObject> states = new List<StateObject>();
            var isEnemyExisting = ReferenceEquals(_target, null);
            if (!isEnemyExisting) {
                var targets = _vision.GetTargetsInVision();
                var enemies = new List<GameObject>();
                foreach (var targetBundle in targets) {
                    if (_organisation.GetRelationWithOrganisation(targetBundle.Key) < _organisation.AgroThreshold) {
                        enemies.AddRange(targetBundle.Value);
                    }
                }

                if (enemies.Count > 0) {
                    isEnemyExisting = true;
                    _agroTime = Time.time + _agroDuration;
                    _target = enemies[0].GetComponent<Health>();
                }
                
            }
            
            states.Add(new StateObject { Name = "isEnemyInVision", Value = isEnemyExisting});
            states.Add(new StateObject { Name = "IsSuspicious", Value = _agroTime > Time.time });
            states.Add(new StateObject { Name = "Target", Value = _target });
            return states;
        }
        public override List<StateObject> CreateGoal() {
            List<StateObject> goal = new();
            
            goal.Add(new StateObject {Name = "CheckArea", Value = true});
            return goal;
        }
    }
}
