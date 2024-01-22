
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

        
        // TODO: Change the target to GameObject(can be as target, and as destination point
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
            print("Checking states...");
            states.Add(new StateObject { Name = "is_suspicious", Value = _agroTime > Time.time });
            states.Add(new StateObject { Name = "is_enemy_visible", Value = isEnemyExisting});
            states.Add(new StateObject { Name = "target_position", Value = _target ? _target.transform.position : null });
            return states;
        }
        public override List<StateObject> CreateGoal() {
            List<StateObject> goal = new();
            
            goal.Add(new StateObject {Name = "investigate", Value = true});
            return goal;
        }
    }
}
