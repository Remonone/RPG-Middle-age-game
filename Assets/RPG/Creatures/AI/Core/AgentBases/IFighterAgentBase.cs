using UnityEngine;

namespace RPG.Creatures.AI.Core.AgentBases {
    public interface IFighterAgentBase {
        public Vector3 GetTargetLastPosition();
        public Vector3 GetTargetLastDirection();
        public float GetAgroTime();
        public float GetSuspiciousTime();
        public GameObject GetEnemy();
    }
}
