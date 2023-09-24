using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Movement {
    public class EnemyMovement : MonoBehaviour {

        [SerializeField] private float _rangeX;
        [SerializeField] private float _rangeZ;
        [SerializeField] private float _treshold = .5f;
        
        private NavMeshAgent _agent;

        private void Awake() {
            _agent = GetComponent<NavMeshAgent>();
        }

        private void Start() {
            _agent.destination = GetRandomPoint();
        }

        private void Update() {
            if ((transform.position - _agent.destination).magnitude < _treshold) {
                _agent.destination = GetRandomPoint();
            }
        }

        private Vector3 GetRandomPoint() {
            var pos = transform.position;
            Random random = new Random((int)((pos.x + pos.y + pos.z) * 100));
            return new Vector3((float)random.NextDouble() * _rangeX, 1, (float)random.NextDouble() * _rangeZ);
        }

    }
}
