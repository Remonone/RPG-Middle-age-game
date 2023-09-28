using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class Mover : MonoBehaviour {
        private NavMeshAgent _agent;

        
        [SerializeField] private Animator _animator;
        [SerializeField] private float _threshold = 2F;
        
        private readonly int _hZSpeed = Animator.StringToHash("Velocity Z");
        private readonly int _hMoving = Animator.StringToHash("Moving");

        public Vector3 CurrentDestinationPoint => _agent.destination;

        private BaseStats _baseStats;
        
        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
            _baseStats = GetComponent<BaseStats>();
        }

        private void Start() {
            _agent.speed = _baseStats.GetStatValue(Stat.MOVEMENT_SPEED);
            _agent.angularSpeed = 1000F;
        }
        
        private void Update() {
            if ((_agent.destination - transform.position).magnitude < _threshold) {
                CancelMove();
            }
        }

        public void MoveToPoint(Vector3 point) {
            
            _agent.isStopped = false;
            _agent.destination = point;
            _animator.SetFloat(_hZSpeed, 1);
            _animator.SetBool(_hMoving, true);
        }

        public void CancelMove() {
            _agent.isStopped = true;
            _animator.SetFloat(_hZSpeed, 0);
        }

        void FootR() {
            
        }
        void FootL() {
            
        }        
    }
}
