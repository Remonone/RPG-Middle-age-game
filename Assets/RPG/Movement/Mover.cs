using Newtonsoft.Json.Linq;
using RPG.Combat;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class Mover : NetworkBehaviour, IAction, ISaveable {
        
        [SerializeField] protected Animator _animator;
        
        private readonly int _speed = Animator.StringToHash("Movespeed");

        private BaseStats _baseStats;
        private TaskScheduler _scheduler;
        private NavMeshAgent _agent;
        private Health _health;
        private float _creatureSpeed;

        public override void OnNetworkSpawn() {
            _agent = GetComponent<NavMeshAgent>();
            _baseStats = GetComponent<BaseStats>();
            _scheduler = GetComponent<TaskScheduler>();
            _health = GetComponent<Health>();
            _baseStats.OnStatUpdated += OnStatUpdated;
        }

        private void OnStatUpdated() {
            _agent.speed = _baseStats.GetStatValue(Stat.MOVEMENT_SPEED);
        }
        
        protected void Start() {
            _agent.speed = _baseStats.GetStatValue(Stat.MOVEMENT_SPEED);
            _agent.angularSpeed = 1000F;
        }
        
        private void Update() {
            if (!IsOwner) return;
            _agent.enabled = _health.IsAlive;
            UpdateAnimator();
        }

        public void RequestToMove(Vector3 destination) {
            if (!IsOwner) return;
            _scheduler.SwitchAction(this);
            MoveAgent(destination);
            MoveToPointServerRpc(destination);
        }
        
        public void RequestToTransfer(Vector3 destination) {
            if (!IsOwner) return;
            MoveAgent(destination);
            MoveToPointServerRpc(destination);
        }
        

        [ServerRpc]
        private void MoveToPointServerRpc(Vector3 destination) {
            MoveAgent(destination);
        }
        
        public void MoveToPoint(Vector3 destination) {
            if (!IsServer) return;
            MoveAgent(destination);
        }

        public void Cancel() {
            if (IsServer) {
                ResetComponent();
                return;
            }

            if (!IsOwner) return;
            _agent.isStopped = true;
            MoverResetComponentServerRpc();
        }
        
        [ServerRpc]
        private void MoverResetComponentServerRpc() {
            ResetComponent();
        }
        
        private void ResetComponent() {
            if (!IsServer) return;
            _agent.isStopped = true;
        }

        public JToken CaptureAsJToken() {
            return JToken.FromObject(transform.position.ToToken());
        }

        private void UpdateAnimator() {
            var velocity = _agent.velocity;
            var local = transform.InverseTransformDirection(velocity);
            var speed = local.z;
            _animator.SetFloat(_speed, speed / _agent.speed);
        }

        private void MoveAgent(Vector3 dest) {
            _agent.isStopped = false;
            _agent.destination = dest;
        }
        
        public void RestoreFromJToken(JToken state) {
            _agent.enabled = false;
            transform.position = state.ToObject<Vector3>();
            _agent.enabled = true;
            _scheduler.CancelAction();
        }
    }
}
