using Movement;
using Movement.Patrols;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Control {
    public class EnemyMovement : MonoBehaviour {

        [SerializeField] private Patrol _patrol; 
        [SerializeField] private float _treshold = .5f;
        
        private Mover _mover;

        private void Awake() {
            _mover = GetComponent<Mover>();
        }

        private void Start() {
            _mover.MoveToPoint(_patrol.NextPoint());
        }

        private void Update() {
            if ((transform.position - _mover.CurrentDestinationPoint).magnitude < _treshold) {
                _mover.MoveToPoint(_patrol.NextPoint());
            }
        }


    }
}
