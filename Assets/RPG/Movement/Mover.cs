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
        [SerializeField] private float _threshold = 2F;
        
        private readonly int Speed = Animator.StringToHash("Movespeed");

        private BaseStats _baseStats;
        private TaskScheduler _scheduler;
        private NavMeshAgent _agent;
        private Health _health;

        public override void OnNetworkSpawn() {
            _agent = GetComponent<NavMeshAgent>();
            _baseStats = GetComponent<BaseStats>();
            _scheduler = GetComponent<TaskScheduler>();
            _health = GetComponent<Health>();
        }

        protected void Start() {
            _agent.speed = _baseStats.GetStatValue(Stat.MOVEMENT_SPEED);
            _agent.angularSpeed = 1000F;
        }
        
        private void Update() {
            if (!IsOwner) return;
            _agent.enabled = _health.IsAlive;
            if ((_agent.destination - transform.position).magnitude < _threshold) {
                Cancel();
            }
        }

        public void RequestToMove(Vector3 destination) {
            if (!IsOwner) return;
            _scheduler.SwitchAction(this);
            _agent.isStopped = false;
            _agent.destination = destination;
            MoveToPointServerRpc(destination);
        }
        
        public void RequestToTransfer(Vector3 destination) {
            if (!IsOwner) return;
            _agent.isStopped = false;
            _agent.destination = destination;
            MoveToPointServerRpc(destination);
        }
        

        [ServerRpc]
        private void MoveToPointServerRpc(Vector3 destination) {
            MoveToPoint(destination);
        }
        
        public void MoveToPoint(Vector3 destination) {
            if (!IsServer) return;
            _agent.isStopped = false;
            _agent.destination = destination;
            _animator.SetFloat(Speed, 1f);
        }

        public void Cancel() {
            if (IsServer) {
                ResetComponent();
                return;
            }

            if (!IsOwner) return;
            _agent.isStopped = true;
            _animator.SetFloat(Speed, 0f);
            MoverResetComponentServerRpc();
        }
        [ServerRpc]
        private void MoverResetComponentServerRpc() {
            ResetComponent();
        }
        private void ResetComponent() {
            if (!IsServer) return;
            _agent.isStopped = true;
            _animator.SetFloat(Speed, 0f);
        }

        public JToken CaptureAsJToken() {
            Debug.Log(transform.position.ToToken());
            return JToken.FromObject(transform.position.ToToken());
        }
        public void RestoreFromJToken(JToken state) {
            _agent.enabled = false;
            transform.position = state.ToObject<Vector3>();
            _agent.enabled = true;
            _scheduler.CancelAction();
        }
    }
}
