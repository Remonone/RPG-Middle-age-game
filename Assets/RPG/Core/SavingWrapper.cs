using RPG.Saving;
using UnityEngine;

namespace RPG.Core {
    [RequireComponent(typeof(SavingSystem))]
    public class SavingWrapper : MonoBehaviour {

        private SavingSystem _system;

        public SavingSystem System => _system;
        private void Start() {
            DontDestroyOnLoad(this);
            _system = GetComponent<SavingSystem>();
        }
    }
}
