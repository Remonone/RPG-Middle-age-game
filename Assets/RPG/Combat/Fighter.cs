using System;
using RPG.Combat.DamageDefinition;
using RPG.Core;
using RPG.Core.Predicate;
using RPG.Core.Predicate.Interfaces;
using RPG.Inventories;
using RPG.Movement;
using RPG.Stats;
using RPG.Utils;
using Unity.Netcode;
using UnityEngine;

namespace RPG.Combat {
    public class Fighter : NetworkBehaviour, IAction, IPredicateHandler{

        [SerializeField] private Cooldown _cooldown;
        [SerializeField] protected bool _shouldResetOnAttack;

        private BaseStats _stats;
        private Mover _mover;
        private TaskScheduler _scheduler;

        private readonly NetworkVariable<NetworkObjectReference> _target = new();
        private Equipment _equipment;
        private Animator _animator;

        public event Action<DamageReport> OnAttack;

        private readonly int _isAttacking = Animator.StringToHash("IsAttacking");

        private Vector3? TargetPosition {
            get {
                if (NetworkManager.Singleton.SpawnManager == null) return null;
                if (!_target.Value.TryGet(out var obj)) return null;
                return obj.transform.position;
            }
        }

        public override void OnNetworkSpawn() {
            _mover = GetComponent<Mover>();
            _stats = GetComponent<BaseStats>();
            _scheduler = GetComponent<TaskScheduler>();
            _animator = GetComponent<Animator>();
            _equipment = GetComponent<Equipment>();
        }

        private void Update() {
            if (!TargetPosition.HasValue) return;
            if (!IsOwner) return;
            if (!IsTargetInRange()) {
                _mover.RequestToTransfer(TargetPosition.Value);
            } else {
                 AttackTarget();
            }
            
        }
        private bool IsTargetInRange() {
            return TargetPosition.HasValue && (TargetPosition.Value - transform.position).magnitude < _stats.GetStatValue(Stat.ATTACK_RANGE);
        }

        private void AttackTarget() {
            if (!_cooldown.IsAvailable || !_target.Value.TryGet(out _)) return;
            _mover.Cancel();
            _animator.SetTrigger(_isAttacking);
        }

        void Hit() {
            if (!IsTargetInRange()) return;
            if (IsServer) {
                ApplyHit();
            }
        }
        
        private void ApplyHit() {
            // LATER: create range weapon
            var weapon = _equipment.GetEquipmentItem(EquipmentSlot.WEAPON);
            var damageType = ReferenceEquals(weapon, null) ? DamageType.PHYSICAL : weapon.Type;
            _target.Value.TryGet(out var obj);
            var targetHealth = obj.GetComponent<Health>();
            var report = DamageUtils.CreateReport(targetHealth, _stats.GetStatValue(Stat.BASE_ATTACK), damageType, GetComponent<NetworkObject>());
            OnAttack?.Invoke(report);
            targetHealth.HitEntity(report);
        }

        public void Attack(GameObject target) {
            if (!IsOwner) return;
            var netObj = target.GetComponent<NetworkObject>();
            if (target.GetComponent<Health>() is not { IsAlive: true }) return;
            _scheduler.SwitchAction(this);
            SetTargetServerRpc(netObj);
        }
        
        [ServerRpc]
        private void SetTargetServerRpc(NetworkObjectReference netObj) {
            netObj.TryGet(out var component);
            SetTarget(component.gameObject);
        }

        public void SetTarget(GameObject target) {
            if (!IsServer) return;
            var obj = target.GetComponent<NetworkObject>();
            if (target.GetComponent<Health>() is not { IsAlive: true }) return;
            _target.Value = obj;
        }

        public void Cancel() {
            if (IsServer) {
                ResetComponent();
                return;
            }
            if (IsOwner) {
                FighterResetComponentServerRpc();
            }
        }
        
        [ServerRpc]
        private void FighterResetComponentServerRpc() {
            ResetComponent();
        }
        private void ResetComponent() {
            _target.Value = new NetworkObjectReference();
        }

        
        public object Predicate(string command, object[] arguments) {
            return command switch {
                "AttackTarget" => PerformHit(arguments),
                _ => null
            };
        }
        //
        private object PerformHit(object[] arguments) {
            var objToHit = PredicateWorker.GetPredicateMonoBehaviour((string)arguments[0]);
            if (objToHit.GetComponent<Health>() is not { } target) return null;
            var netObject = objToHit.GetComponent<NetworkObject>();
            var report = DamageUtils.CreateReport(target, (float)Convert.ToDouble(arguments[1]), (DamageType)Enum.Parse(typeof(DamageType), Convert.ToString(arguments[2])), netObject);
            target.HitEntity(report);
            return true;
        }

    }
}
