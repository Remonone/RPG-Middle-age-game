using UnityEngine;

namespace RPG.Core {
    public class PersistantObject : MonoBehaviour {
        [SerializeField] private GameObject _persistentObject;
        private static bool _hasSpawned = false;
        
        private void Awake() {
            if (_hasSpawned) return;

            SpawnPersistentObjects();
            _hasSpawned = true;
        }
        private void SpawnPersistentObjects() {
            var persistentObject = Instantiate(_persistentObject);
            DontDestroyOnLoad(persistentObject);
        }
    }
}
