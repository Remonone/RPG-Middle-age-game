using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Creatures.AI.Core;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Creatures.AI.Roles {
    [RequireComponent(typeof(Health))]
    public class Guardian : BaseAgentBehaviour {
        [SerializeField] private float _agroDuration;
        [SerializeField] private float _suspiciousDuration;
        
        private float _agroTime = -1;
        private float _suspiciousTime = -1;
        private Health _target;
        
        // Cached info
        private Health _health;
        private Equipment _equipment;
        private BaseStats _stats;

        private void Awake() {
            _health = GetComponent<Health>();
            _equipment = GetComponent<Equipment>();
            _stats = GetComponent<BaseStats>();
            _id = Guid.NewGuid(); // TODO: Change to save state
        }
        public override List<StateObject> GetCurrentState() {
            List<StateObject> states = new List<StateObject>();
            
            if (_vision.IsEnemiesInVision) {
                _agroTime = Time.time + _agroDuration;
                var current = _vision.EnemiesInVision.Current;
                if (current != null && _target == null)
                    _target = current.GetComponent<Health>();
            }

            if (_agroTime > Time.time) {
                _suspiciousTime = Time.time + _suspiciousDuration;
            }
            
            states.Add(new StateObject { Name = "is_agro", Value = _agroTime > Time.time });
            states.Add(new StateObject { Name = "is_enemy_visible", Value = _vision.IsEnemiesInVision});
            states.Add(new StateObject {Name = "is_alive", Value = _health.IsAlive});
            states.Add(new StateObject {Name = "is_armed", Value = _equipment.GetEquipmentItem(EquipmentSlot.WEAPON) != null});
            states.Add(new StateObject {Name = "attack_range", Value = _stats.GetStatValue(Stat.ATTACK_RANGE)});
            return states;
        }
        public override List<StateObject> CreateGoal() {
            List<StateObject> goal = new();
            if (_target != null) {
                goal.Add(new StateObject {Name = "liquidate_target", Value = true});
            } else if (_agroTime > Time.time) {
                goal.Add(new StateObject {Name = "check_area", Value = true});
            } else {
                goal.Add(new StateObject {Name = "investigate", Value = true});
            }
            
            return goal;
        }

        public override bool MoveAgent(GoapAction action) {
            // If targets on vision or point is reached;
            action.InRange = _vision.IsEnemiesInVision || base.MoveAgent(action);
            return action.InRange;
        }
    }
}
