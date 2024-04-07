using System;
using RPG.Combat.DamageDefinition;
using RPG.Core;
using RPG.Core.Predicate;
using RPG.Core.Predicate.Interfaces;
using RPG.Inventories;
using RPG.Inventories.Items;
using RPG.Movement;
using RPG.Stats;
using RPG.Utils;
using Unity.Netcode;
using UnityEngine;

namespace RPG.Combat {
    public class Fighter : NetworkBehaviour, IAction, IPredicateHandler{

        [SerializeField] private Cooldown _cooldown;
        [SerializeField] private bool _shouldResetOnAttack;
        [SerializeField] private NetworkObject _networkReference;

        private BaseStats _stats;
        private Mover _mover;
        private TaskScheduler _scheduler;

        private Health _target;
        private Equipment _equipment;
        private Animator _animator;

        public event Action<DamageReport> OnAttack;

        private readonly int _isAttacking = Animator.StringToHash("IsAttacking");
        // PUBLIC

        public bool CanAttack(Health target) => target is { IsAlive: true };
        
        public void Cancel() {
            if (!IsOwner) return;
            _target = null;
            _mover.Cancel();
        }
        
        //[ServerRpc]
        public void Attack(SelectableTarget target) {
            if (!IsOwner) return;
            var reference = target.GetComponent<NetworkObjectReference>();
            AttackServerRpc(reference);
        }

        [ServerRpc]
        private void AttackServerRpc(NetworkObjectReference attackTo, ServerRpcParams serverRpcParams = default) {
            attackTo.TryGet(out var targetNet);
            var target = targetNet.GetComponent<SelectableTarget>();
            if (!target.Targetable) return;
            var health = target.GetComponent<Health>();
            if (health == null || !health.IsAlive) return;
            _scheduler.SwitchAction(this);
            _target = health;

            ClientRpcParams clientRpcParams = new ClientRpcParams {
                Send = {
                    TargetClientIds = new[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            AttackClientRpc(attackTo, clientRpcParams);
        }
        
        [ClientRpc]
        private void AttackClientRpc(NetworkObjectReference attackTo, ClientRpcParams clientRpcParams = default) {
            attackTo.TryGet(out var targetNet);
            var target = targetNet.GetComponent<SelectableTarget>();
            if (!target.Targetable) return;
            var health = target.GetComponent<Health>();
            if (health == null || !health.IsAlive) return;
            _scheduler.SwitchAction(this);
            _target = health;
        }


        public void Attack(Health target) {
            if (target == null || !target.IsAlive) return;
            _scheduler.SwitchAction(this);
            _target = target;
        }
        
        // PRIVATE

        private void Awake() {
            _mover = GetComponent<Mover>();
            _stats = GetComponent<BaseStats>();
            _scheduler = GetComponent<TaskScheduler>();
            _animator = GetComponent<Animator>();
            _equipment = GetComponent<Equipment>();
        }
        
        public object Predicate(string command, object[] arguments) {
            return command switch {
                "AttackTarget" => PerformHit(arguments),
                _ => null
            };
        }
        
        private object PerformHit(object[] arguments) {
            var objToHit = PredicateWorker.GetPredicateMonoBehaviour((string)arguments[0]);
            if (objToHit.GetComponent<Health>() is not { } target) return null;
            var report =
                DamageUtils.CreateReport(target, (float)Convert.ToDouble(arguments[1]), (DamageType)Enum.Parse(typeof(DamageType), Convert.ToString(arguments[2])), _networkReference);
            var netObject = objToHit.GetComponent<NetworkObject>();
            target.HitEntity(report, netObject.NetworkObjectId);
            return true;
        }

        private void Update() {
            if (_target == null || !_target.IsAlive) return;
            
            if (IsTargetInRange() && _cooldown.IsAvailable) {
                AttackTarget();
                _cooldown.Reset();
                return;
            }
            _mover.TranslateToPoint(_target.transform.position);
        }
        public void AttackTarget() {
            _animator.SetTrigger(_isAttacking);
        }

        void Hit() {
            HitServerRpc();
        }
        
        [ServerRpc]
        private void HitServerRpc() {
            if (_target == null) return;
            if (!IsTargetInRange()) return;
            EquipmentItem weapon = _equipment.GetEquipmentItem(EquipmentSlot.WEAPON);
            DamageType type = weapon != null ? weapon.Type : DamageType.PHYSICAL;
            var report = DamageUtils.CreateReport(_target, _stats.GetStatValue(Stat.BASE_ATTACK), type, _networkReference); 
            OnAttack?.Invoke(report); // whenever cause attack to target, may invoke this event to give ability to handle some buffs or additional changes
            var netObject = _target.GetComponent<NetworkObject>();
            _target.HitEntity(report, netObject.NetworkObjectId);
            if (_shouldResetOnAttack) _scheduler.SwitchAction(null);
        }
        

        private bool IsTargetInRange() {
            var distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
            return distanceToTarget <= _stats.GetStatValue(Stat.ATTACK_RANGE);
        }

    }
}
