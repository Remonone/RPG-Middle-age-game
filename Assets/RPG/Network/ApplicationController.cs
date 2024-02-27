using System.Threading.Tasks;
using RPG.Network.Client;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Network {
    public class ApplicationController : MonoBehaviour {

        [SerializeField] private ClientSingleton _clientPrefab;
    
        private async void Start() {
            DontDestroyOnLoad(gameObject);

            await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
        }
        private async Task LaunchInMode(bool isDedicatedServer) {
            if (isDedicatedServer) {
                SceneManager.LoadScene("Village");
            }
            else {
                var clientSingleton = Instantiate(_clientPrefab);
                await clientSingleton.CreateClient();
            }
        }
    }
}
