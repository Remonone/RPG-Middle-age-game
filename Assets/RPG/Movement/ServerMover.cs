using RPG.Combat;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement {
    public class ServerMover : Mover {
        
        public override void OnNetworkSpawn() {
            if (!IsServer || !IsOwner) return;
            Agent = GetComponent<NavMeshAgent>();
            BaseStats = GetComponent<BaseStats>();
            Scheduler = GetComponent<TaskScheduler>();
            Health = GetComponent<Health>();
        }

        public override void StartMovingToPoint(Vector3 point) {
            if (!IsServer) return;
            if (!Health.IsAlive) return;
            Scheduler.SwitchAction(this);
            TranslateToPoint(point);
        }

        public override void TranslateToPoint(Vector3 destination) {
            if (!IsServer) return;
            if (!Health.IsAlive) return;
            Agent.isStopped = false;
            Agent.destination = destination;
        }

        protected override void Start() {
            if (!IsServer) return;
            Agent.speed = BaseStats.GetStatValue(Stat.MOVEMENT_SPEED);
            Agent.angularSpeed = 1000F;
        }
    }
}
