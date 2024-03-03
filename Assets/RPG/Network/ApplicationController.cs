using System.Threading.Tasks;
using RPG.Network.Client;
using RPG.Network.Server;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Network {
    public class ApplicationController : MonoBehaviour {

        [SerializeField] private ClientSingleton _clientPrefab;
        [SerializeField] private ServerSingleton _serverPrefab;
    
        private async void Start() {
            DontDestroyOnLoad(gameObject);

            await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }
        private async Task LaunchInMode(bool isDedicatedServer) {
            if (isDedicatedServer) {
                SceneManager.LoadScene("Village");
                ServerSingleton serverSingleton = Instantiate(_serverPrefab);
                await serverSingleton.InitServer();
            }
            else {
                var clientSingleton = Instantiate(_clientPrefab);
                await clientSingleton.CreateClient();
            }
        }
    }
}
