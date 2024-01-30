using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Creatures.AI.Core;
using RPG.Inventories;
using RPG.Stats;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Creatures.AI.Roles {
    [RequireComponent(typeof(Health))]
    public class Guardian : BaseAgentBehaviour, IOrganisationWrapper {
        
        [SerializeField] private Organisation _organisation;
        [SerializeField] private float _agroDuration;

        private Guid _id;
        private float _agroTime = -1;
        private Health _target;
        
        // Cached info
        private Health _health;
        private Equipment _equipment;
        private BaseStats _stats;

        public GameObject Target => _target.gameObject;

        private void Awake() {
            _health = GetComponent<Health>();
            _equipment = GetComponent<Equipment>();
            _stats = GetComponent<BaseStats>();
            _id = Guid.NewGuid(); // TODO: Change to save state
        }
        public override List<StateObject> GetCurrentState() {
            List<StateObject> states = new List<StateObject>();
            var isEnemyExisting = false;
            
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
                
            
            states.Add(new StateObject { Name = "is_suspicious", Value = _agroTime > Time.time });
            states.Add(new StateObject { Name = "is_enemy_visible", Value = isEnemyExisting});
            states.Add(new StateObject { Name = "target_position", Value = _target ? _target.transform.position : null });
            states.Add(new StateObject {Name = "is_alive", Value = _health.IsAlive});
            states.Add(new StateObject {Name = "is_armed", Value = _equipment.GetEquipmentItem(EquipmentSlot.WEAPON) != null});
            states.Add(new StateObject {Name = "attack_range", Value = _stats.GetStatValue(Stat.ATTACK_RANGE)});
            return states;
        }
        public override List<StateObject> CreateGoal() {
            List<StateObject> goal = new();
            if (_target != null) {
                goal.Add(new StateObject {Name = "liquidate_target", Value = true});
            } else {
                goal.Add(new StateObject {Name = "investigate", Value = true});
            }
            
            return goal;
        }

        public Organisation GetOrganisation() {
            return _organisation;
        }
        public Guid GetGuid() {
            return _id;
        }
    }
}
