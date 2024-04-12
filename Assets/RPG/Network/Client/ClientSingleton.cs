using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace RPG.Network.Client {
    public class ClientSingleton : MonoBehaviour {

        private static ClientSingleton _instance;
        public static bool IsInitiated { get; private set; } 

        private ClientGameManager _manager;

        public ClientGameManager Manager => _manager;
        
        public static ClientSingleton Instance {
            get {
                if (ReferenceEquals(_instance, null)) return _instance;

                _instance = FindObjectOfType<ClientSingleton>();

                if (ReferenceEquals(_instance, null)) {
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
            IsInitiated = true;
        }
    }
}
