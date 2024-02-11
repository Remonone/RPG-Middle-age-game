using System;
using System.Collections.Generic;
using RPG.Combat;
using RPG.Combat.DamageDefinition;
using RPG.Creatures.AI.Core;
using RPG.Creatures.AI.Core.AgentBases;
using RPG.Inventories;
using RPG.Stats;
using UnityEngine;

namespace RPG.Creatures.AI.Roles {
    [RequireComponent(typeof(Health))]
    public class Guardian : BaseAgentBehaviour, IFighterAgentBase {
        [SerializeField] private float _agroDuration = 5f;
        [SerializeField] private float _suspiciousDuration = 15f;
        [SerializeField] private float _detectionRadius = 10f;
        
        private float _agroTime = -1;
        private float _suspiciousTime = -1;
        private Health _target;
        
        // Cached info
        private Health _health;
        private Equipment _equipment;

        private Vector3 _targetLastPosition;
        private Vector3 _targetDirection;

        private void Awake() {
            _health = GetComponent<Health>();
            _equipment = GetComponent<Equipment>();
            _id = Guid.NewGuid(); // TODO: Change to save state
        }

        private void OnEnable() {
            _vision.EnemySpotted += OnEnemySpotted;
            _vision.EnemyMissing += OnEnemyMissing;
            _health.OnHit += OnHit;
        }
        private void OnHit(DamageReport report) {
            if (!report.Attacker.TryGetComponent<Health>(out var target)) return;
            if ((target.transform.position - transform.position).magnitude > _detectionRadius) {
                _suspiciousTime = Time.time + _suspiciousDuration;
            } else {
                _target = target;
            }
        }

        private void OnDisable() {
            _vision.EnemySpotted -= OnEnemySpotted;
            _vision.EnemyMissing -= OnEnemyMissing;
        }

        private void OnEnemySpotted(Health health) {
            if (_target != null) return;
            _target = health;
        }

        public override void OnPlanAborted(GoapAction aborter) { }

        private void OnEnemyMissing(Health health) {
            if (_target != health) return;
            var targetTransform = _target.transform;
            _targetLastPosition = targetTransform.position;
            _targetDirection = targetTransform.forward;
            _target = null;
        }
        
        public override List<StateObject> GetCurrentState() {
            List<StateObject> states = new List<StateObject>();
            var shouldAlert = false;

            if (_target != null) {
                if (_agroTime < Time.time) {
                    shouldAlert = true;
                }

                if (_target.IsAlive) {
                    _agroTime = Time.time + _agroDuration;
                }
                else {
                    _agroTime = Time.time;
                }
            }
            if (_agroTime > Time.time) 
                _suspiciousTime = Time.time + _suspiciousDuration;
            
            states.Add(new StateObject { Name = "is_suspicious", Value = _suspiciousTime > Time.time });
            states.Add(new StateObject { Name = "is_agro", Value = _agroTime > Time.time });
            states.Add(new StateObject { Name = "is_enemy_visible", Value = _vision.IsEnemiesInVision });
            states.Add(new StateObject { Name = "is_alive", Value = _health.IsAlive });
            states.Add(new StateObject { Name = "is_armed", Value = _equipment.GetEquipmentItem(EquipmentSlot.WEAPON) != null });
            states.Add(new StateObject { Name = "attack_range", Value = _stats.GetStatValue(Stat.ATTACK_RANGE) });
            states.Add(new StateObject { Name = "should_alert", Value = shouldAlert });
            
            return states;
        }
        public override List<StateObject> CreateGoal() {
            List<StateObject> goal = new();
            if (_agroTime > Time.time) {
                goal.Add(new StateObject {Name = "liquidate_target", Value = true});
            } else if (_suspiciousTime > Time.time) {
                goal.Add(new StateObject {Name = "check_area", Value = true});
            } else {
                goal.Add(new StateObject {Name = "investigate", Value = true});
            }
            
            return goal;
        }

        public override bool MoveAgent(GoapAction action) {
            action.InRange = base.MoveAgent(action) || (_vision.IsEnemiesInVision && _agroTime < Time.time);
            return action.InRange;
        }

        public Vector3 GetTargetLastPosition() => _targetLastPosition;
        public Vector3 GetTargetLastDirection() => _targetDirection;
        public float GetAgroTime() => _agroTime;
        public float GetSuspiciousTime() => _suspiciousTime;
        public GameObject GetEnemy() => _target ? _target.gameObject : null;
    }
    
}
