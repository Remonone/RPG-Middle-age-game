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
        
        protected readonly int Speed = Animator.StringToHash("Movespeed");

        protected BaseStats BaseStats;
        protected TaskScheduler Scheduler;
        protected NavMeshAgent Agent;
        protected Health Health;

        public override void OnNetworkSpawn() {
            Agent = GetComponent<NavMeshAgent>();
            BaseStats = GetComponent<BaseStats>();
            Scheduler = GetComponent<TaskScheduler>();
            Health = GetComponent<Health>();
        }

        protected virtual void Start() {
            Agent.speed = BaseStats.GetStatValue(Stat.MOVEMENT_SPEED);
            Agent.angularSpeed = 1000F;
        }
        
        private void Update() {
            if (!IsOwner) return;
            Agent.enabled = Health.IsAlive;
            if ((Agent.destination - transform.position).magnitude < _threshold) {
                Cancel();
            }
        }

        public virtual void StartMovingToPoint(Vector3 point) {
            if (!Health.IsAlive) return;
            Scheduler.SwitchAction(this);
            TranslateToPoint(point);
        }
        
        public virtual void TranslateToPoint(Vector3 point) {
            if (!Health.IsAlive) return;
            TranslateObjectServerRpc(point);
        }

        [ServerRpc]
        private void TranslateObjectServerRpc(Vector3 destination, ServerRpcParams serverRpcParams = default) {
            Agent.isStopped = false;
            Agent.destination = destination;
            _animator.SetFloat(Speed, 1);
            ClientRpcParams clientRpcParams = new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            TranslateObjectClientRpc(destination, clientRpcParams);
        }

        [ClientRpc]
        private void TranslateObjectClientRpc(Vector3 destination, ClientRpcParams clientRpcParams = default) {
            Agent.isStopped = false;
            Agent.destination = destination;
            _animator.SetFloat(Speed, 1);
        }


        public void Cancel() {
            CancelServerRpc();
        }

        [ServerRpc]
        private void CancelServerRpc(ServerRpcParams serverRpcParams = default) {
            Agent.isStopped = true;
            _animator.SetFloat(Speed, 0);
            ClientRpcParams clientRpcParams = new ClientRpcParams {
                Send = new ClientRpcSendParams {
                    TargetClientIds = new[] { serverRpcParams.Receive.SenderClientId }
                }
            };
            CancelClientRpc(clientRpcParams);
        }

        [ClientRpc]
        private void CancelClientRpc(ClientRpcParams clientRpcParams) {
            Agent.isStopped = true;
            _animator.SetFloat(Speed, 0);
        }
        
        public JToken CaptureAsJToken() {
            return JToken.FromObject(transform.position.ToToken());
        }
        public void RestoreFromJToken(JToken state) {
            Agent.enabled = false;
            transform.position = state.ToObject<Vector3>();
            Agent.enabled = true;
            Scheduler.CancelAction();
        }
    }
}
