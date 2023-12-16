using UnityEngine;

namespace RPG.Creatures.AI {
    public class EnemyAI : MonoBehaviour {
        [SerializeField] private string[] _enemyTag; // TODO: Swap to fractions
        [SerializeField] private int _fieldOfView;
        [SerializeField] private float _visionRange;
        [SerializeField] private float _soundDetectionRange;
        
        
        // TODO: Implement Goal Oriented Action Planning
    }
}
