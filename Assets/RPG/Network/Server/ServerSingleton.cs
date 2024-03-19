using System.Threading.Tasks;
using UnityEngine;

namespace RPG.Network.Server {
    public class ServerSingleton : MonoBehaviour {

        private static ServerSingleton _instance;

        private ServerGameManager _manager;
        
        public static ServerSingleton Instance {
            get {
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<ServerSingleton>();

                if (_instance == null) {
                    Debug.LogError("No ServerSingleton initialized!");
                    return null;
                }

                return _instance;
            }
        }

        private void Start() {
            DontDestroyOnLoad(gameObject);
        }

        public async Task InitServer() {
            _manager = new ServerGameManager();
        }
    }
}
