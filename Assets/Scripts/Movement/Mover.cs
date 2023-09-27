using UnityEngine;
using UnityEngine.AI;

namespace Movement {
    public class Mover : MonoBehaviour {
        private NavMeshAgent _agent;

        [SerializeField] private float _speed;
        [SerializeField] private Animator _animator;
        
        private readonly int _hZSpeed = Animator.StringToHash("MovementZ");

        public Vector3 CurrentDestinationPoint => _agent.destination;
        
        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
            _agent.speed = _speed;
        }

        public void MoveToPoint(Vector3 point) {
            _agent.isStopped = false;
            _agent.destination = point;
            _animator.SetFloat(_hZSpeed, _agent.speed);
        }

        public void CancelMove() {
            _agent.isStopped = true;
        }
        
    }
}
