using UnityEngine;

namespace RPG.Lobby {
    public class LobbyHandler : MonoBehaviour {
        [SerializeField] private LobbyDataContainer _container;
        

        private void OnEnable() {
            _container.OnUpdate += UpdateLobbies;
            _container.UpdateList();
        }
        

        private void OnDisable() {
            _container.OnUpdate -= UpdateLobbies;
        }
        
        private void UpdateLobbies() {
            
        }
        
    }
}
