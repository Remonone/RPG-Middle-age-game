using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class Mover : MonoBehaviour, IAction {
        
        [SerializeField] private Animator _animator;
        [SerializeField] private float _threshold = 2F;
        
        private readonly int _hZSpeed = Animator.StringToHash("Velocity Z");
        private readonly int _hMoving = Animator.StringToHash("Moving");

        private BaseStats _baseStats;
        private TaskScheduler _scheduler;
        private NavMeshAgent _agent;
        private Health _health;
        
        public Vector3 CurrentDestinationPoint => _agent.destination;
        
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
            _scheduler.SwitchAction(this);
            MoveToPoint(point);
        }

        public void MoveToPoint(Vector3 point) {
            
            _agent.isStopped = false;
            _agent.destination = point;
            _animator.SetFloat(_hZSpeed, 1);
            _animator.SetBool(_hMoving, true);
        }


        void FootR() {
            
        }
        void FootL() {
            
        }
        public void Cancel() {
            _agent.isStopped = true;
            _animator.SetFloat(_hZSpeed, 0);
        }
    }
}
