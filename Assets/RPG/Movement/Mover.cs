using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class Mover : MonoBehaviour {
        private NavMeshAgent _agent;

        [SerializeField] private float _speed; // Will be changed
        [SerializeField] private Animator _animator;
        [SerializeField] private float _threshold = 2F;
        
        private readonly int _hZSpeed = Animator.StringToHash("Velocity Z");
        private readonly int _hMoving = Animator.StringToHash("Moving");

        public Vector3 CurrentDestinationPoint => _agent.destination;
        
        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
            _agent.speed = _speed;
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
