using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace RPG.Network.Client {
    public class ClientSingleton : MonoBehaviour {

        private static ClientSingleton _instance;

        private ClientGameManager _manager;

        public ClientGameManager Manager => _manager;
        
        public static ClientSingleton Instance {
            get {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<ClientSingleton>();

                if (_instance == null) {
                    Debug.LogError("No ClientSingleton initialized!");
                    return null;
                }

                return _instance;
            }
        }

        IEnumerator Start() {
            DontDestroyOnLoad(gameObject);
            yield return CreateClient();
        }

        public async Task CreateClient() {
            _manager = new ClientGameManager();

            await _manager.InitClient();
        }
    }
}
