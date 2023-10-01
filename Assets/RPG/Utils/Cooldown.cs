using UnityEngine;

namespace RPG.Utils {
    public class Cooldown : MonoBehaviour {
        [SerializeField] private float _timeToReset = 1f;

        private float _cooldownTime = -1f;

        public bool IsAvailable => _cooldownTime < Time.time;

        public void Reset() => _cooldownTime = Time.time + _timeToReset;
    }
}
