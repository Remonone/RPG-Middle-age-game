using Newtonsoft.Json.Linq;
using RPG.Combat;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using RPG.Utils;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class Mover : MonoBehaviour, IAction, ISaveable {
        
        [SerializeField] private Animator _animator;
        [SerializeField] private float _threshold = 2F;
        
        private readonly int _speed = Animator.StringToHash("Movespeed");

        private BaseStats _baseStats;
        private TaskScheduler _scheduler;
        private NavMeshAgent _agent;
        private Health _health;

        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
            _baseStats = GetComponent<BaseStats>();
            _scheduler = GetComponent<TaskScheduler>();
            _health = GetComponent<Health>();
        }

        private void Start() {
            _agent.speed = _baseStats.GetStatValue(Stat.MOVEMENT_SPEED);
            _agent.angularSpeed = 1000F;
        }
        
        private void Update() {
            _agent.enabled = _health.IsAlive;
            if ((_agent.destination - transform.position).magnitude < _threshold) {
                Cancel();
            }
        }

        public void StartMovingToPoint(Vector3 point) {
            if (!_health.IsAlive) return;
            _scheduler.SwitchAction(this);
            MoveToPoint(point);
        }

        public void MoveToPoint(Vector3 point) {
            if (!_health.IsAlive) return;
            _agent.isStopped = false;
            _agent.destination = point;
            _animator.SetFloat(_speed, 1);
        }


        public void Cancel() {
            _agent.isStopped = true;
            _animator.SetFloat(_speed, 0);
        }
        public JToken CaptureAsJToken() {
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
