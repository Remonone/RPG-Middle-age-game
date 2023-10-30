using System;
using Newtonsoft.Json.Linq;
using RPG.Combat.DamageDefinition;
using RPG.Control;
using RPG.Core;
using RPG.Core.Predicate;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using RPG.UI.Cursors;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat {
    public class Fighter : MonoBehaviour, IAction{

        [SerializeField] private Cooldown _cooldown;

        private string _sessionID;

        private BaseStats _stats;
        private Mover _mover;
        private TaskScheduler _scheduler;

        private Health _target;
        private Animator _animator;

        private readonly int _hAttack = Animator.StringToHash("Action"); 
        private readonly int _hMoving = Animator.StringToHash("Moving");
        private readonly int _hTriggerAction = Animator.StringToHash("TriggerNumber");
        private readonly int _hTrigger = Animator.StringToHash("Trigger");

        private string AmplifyPredicate = "#{0}:AmplifyStat:BASE_ATTACK;5;0.";
        
        // PUBLIC

        public bool CanAttack(Health target) => target is { IsAlive: true };
        
        public void Cancel() {
            _target = null;
        }

        public void Attack(SelectableEnemy target) {
            if (!target._isTargetable) return;
            var health = target.GetComponent<Health>();
            if (health == null || !health.IsAlive) return;
            _scheduler.SwitchAction(this);
            _target = health;
        }
        
        // PRIVATE

        private void Awake() {
            _mover = GetComponent<Mover>();
            _stats = GetComponent<BaseStats>();
            _scheduler = GetComponent<TaskScheduler>();
            _animator = GetComponent<Animator>();
            _sessionID = Guid.NewGuid().ToString();
        }
        
        private void Update() {
            if (_target == null || !_target.IsAlive) return;
            
            var distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
            if (distanceToTarget <= _stats.GetStatValue(Stat.ATTACK_RANGE) && _cooldown.IsAvailable) {
                AttackTarget();
                _cooldown.Reset();
                return;
            }
            _mover.MoveToPoint(_target.transform.position);
        }
        private void AttackTarget() {
            _animator.SetBool(_hMoving, false);
            _animator.SetInteger(_hTriggerAction, 4);
            _animator.SetInteger(_hAttack, 4); // TODO: Create a list of random attack list;
            _animator.SetTrigger(_hTrigger);
        }

        void Hit() {
            PredicateWorker.ParsePredicate(string.Format(AmplifyPredicate, GetComponent<BaseStats>().ComponentID), _sessionID);
            // TODO: Change DamageType by player equipment;
            var report = DamageUtils.CreateReport(_target, _stats.GetStatValue(Stat.BASE_ATTACK), DamageType.PHYSICAL, gameObject); 
            // TODO: By player equipment or buffs cast additional changes to target;
            Debug.Log(report);
            _target.HitEntity(report);
        }

    }
}
