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
        [SerializeField] protected bool _shouldResetOnAttack;
        [SerializeField] protected NetworkObject _networkReference;

        protected BaseStats Stats;
        protected Mover Mover;
        protected TaskScheduler Scheduler;

        protected Health Target;
        protected Equipment Equipment;
        protected Animator Animator;

        public event Action<DamageReport> OnAttack;

        private readonly int _isAttacking = Animator.StringToHash("IsAttacking");
        // PUBLIC

        public bool CanAttack(Health target) => target is { IsAlive: true };
        
        public void Cancel() {
            if (!IsOwner) return;
            Target = null;
            Mover.Cancel();
        }
        
        public virtual void Attack(SelectableTarget target) {
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
            Scheduler.SwitchAction(this);
            Target = health;

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
            Scheduler.SwitchAction(this);
            Target = health;
        }


        public void Attack(Health target) {
            if (target == null || !target.IsAlive) return;
            Scheduler.SwitchAction(this);
            Target = target;
        }
        
        // PRIVATE

        private void Awake() {
            Mover = GetComponent<Mover>();
            Stats = GetComponent<BaseStats>();
            Scheduler = GetComponent<TaskScheduler>();
            Animator = GetComponent<Animator>();
            Equipment = GetComponent<Equipment>();
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
            var netObject = objToHit.GetComponent<NetworkObject>();
            var report = DamageUtils.CreateReport(target, (float)Convert.ToDouble(arguments[1]), (DamageType)Enum.Parse(typeof(DamageType), Convert.ToString(arguments[2])), netObject);
            target.HitEntity(report, netObject.NetworkObjectId);
            return true;
        }

        private void Update() {
            if (Target is not { IsAlive: true }) return;
            
            if (IsTargetInRange() && _cooldown.IsAvailable) {
                Animator.SetTrigger(_isAttacking);
                _cooldown.Reset();
                return;
            }
            Mover.TranslateToPoint(Target.transform.position);
        }

        public virtual void Hit() {
            HitServerRpc();
        }

        protected void InvokeOnAttackAction(DamageReport report) {
            OnAttack?.Invoke(report);
        }
        
        [ServerRpc]
        private void HitServerRpc() {
            if (Target == null) return;
            if (!IsTargetInRange()) return;
            EquipmentItem weapon = Equipment.GetEquipmentItem(EquipmentSlot.WEAPON);
            DamageType type = weapon != null ? weapon.Type : DamageType.PHYSICAL;
            var report = DamageUtils.CreateReport(Target, Stats.GetStatValue(Stat.BASE_ATTACK), type, _networkReference); 
            OnAttack?.Invoke(report); // whenever cause attack to target, may invoke this event to give ability to handle some buffs or additional changes
            var netObject = Target.GetComponent<NetworkObject>();
            Target.HitEntity(report, netObject.NetworkObjectId);
            if (_shouldResetOnAttack) Scheduler.SwitchAction(null);
        }
        

        protected bool IsTargetInRange() {
            var distanceToTarget = Vector3.Distance(transform.position, Target.transform.position);
            return distanceToTarget <= Stats.GetStatValue(Stat.ATTACK_RANGE);
        }

    }
}
