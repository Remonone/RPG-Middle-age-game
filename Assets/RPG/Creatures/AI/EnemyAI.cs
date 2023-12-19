using System;
using UnityEngine;

namespace RPG.Creatures.AI {
    public class EnemyAI : MonoBehaviour {
        [SerializeField] private string[] _enemyTag;
        
        [SerializeField] private float _soundDetectionRange;
        [SerializeField] private AiVision _vision;

        private void Awake() {
            _vision.OnAwake();
        }

        // TODO: Implement Goal Oriented Action Planning
    }
}
