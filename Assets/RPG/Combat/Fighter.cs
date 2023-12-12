using System;
using RPG.Combat.DamageDefinition;
using RPG.Core;
using RPG.Core.Predicate;
using RPG.Inventories;
using RPG.Inventories.Items;
using RPG.Movement;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: REDUCE DEPENDENCY LIST

namespace RPG.Combat {
    public class Fighter : PredicateMonoBehaviour, IAction{

        [SerializeField] private Cooldown _cooldown;

        private BaseStats _stats;
        private Mover _mover;
        private TaskScheduler _scheduler;

        private Health _target;
        private Equipment _equipment;
        private Animator _animator;

        public event Action<DamageReport> OnAttack;

        private readonly int _hAttack = Animator.StringToHash("Action"); 
        private readonly int _hMoving = Animator.StringToHash("Moving");
        private readonly int _hTriggerAction = Animator.StringToHash("TriggerNumber");
        private readonly int _hTrigger = Animator.StringToHash("Trigger");
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

        protected override void OnAwake() {
            _mover = GetComponent<Mover>();
            _stats = GetComponent<BaseStats>();
            _scheduler = GetComponent<TaskScheduler>();
            _animator = GetComponent<Animator>();
            _equipment = GetComponent<Equipment>();
        }
        
        public override void Predicate(string command, object[] arguments, out object result) {
            result = command switch {
                "AttackTarget" => PerformHit(arguments),
                _ => null
            };
        }
        private object PerformHit(object[] arguments) {
            var objToHit = PredicateWorker.GetPredicateMonoBehaviour((string)arguments[0]);
            if (objToHit is not Health target) return null;
            var report =
                DamageUtils.CreateReport(target, (float)Convert.ToDouble(arguments[1]), (DamageType)Enum.Parse(typeof(DamageType), Convert.ToString(arguments[2])), gameObject);
            Debug.Log(report.Attacker + (report.Damage + "") + report.Type);
            target.HitEntity(report);
            return true;
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
            var randAttack = Random.Range(1, 4);
            _animator.SetInteger(_hAttack, randAttack);
            _animator.SetTrigger(_hTrigger);
        }

        void Hit() {
            if (_target == null) return;
            EquipmentItem weapon = _equipment.GetEquipmentItem(EquipmentSlots.WEAPON);
            DamageType type = weapon != null ? weapon.Type : DamageType.PHYSICAL;
            var report = DamageUtils.CreateReport(_target, _stats.GetStatValue(Stat.BASE_ATTACK), type, gameObject); 
            OnAttack?.Invoke(report); // whenever cause attack to target, may invoke this event to give ability to handle some buffs or additional changes
            _target.HitEntity(report);
        }

    }
}
