using System.Collections.Generic;
using System.Linq;
using RPG.Creatures.Player;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement.Network {
    public class SceneDistributor : NetworkBehaviour {

        private Dictionary<int, List<ulong>> _sceneDistribution;

        private void Start() {
            
            NetworkManager.OnServerStarted += OnStartServer;
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) {
                DontDestroyOnLoad(this);
                NetworkManager.StartServer();
            }
        }
        private void OnStartServer() {
            NetworkManager.OnClientDisconnectCallback += OnPlayerLeaveScene;
            _sceneDistribution = new Dictionary<int, List<ulong>>();
        }

        private void OnPlayerLeaveScene(ulong playerId) {
            var sceneId = GetSceneIdByClientId(playerId);
            if (sceneId == -1) return;
            _sceneDistribution[sceneId].Remove(playerId);
            if (_sceneDistribution[sceneId].Count < 1) UnloadScene(sceneId);
        }
        
        private void UnloadScene(int sceneId) {
            Scene scene = SceneManager.GetSceneByBuildIndex(sceneId);
            NetworkManager.SceneManager.UnloadScene(scene);
        }

        public void LoadScene(int sceneId, Vector3 position, ulong senderId) {
            ClientRpcParams param = new ClientRpcParams {
                Send = {
                    TargetClientIds = new[] { senderId }
                }
            };
            var client = NetworkManager.ConnectedClients.First(id => id.Key == senderId).Value;
            var reference = client.PlayerObject.GetComponent<NetworkObjectReference>();
            if (_sceneDistribution.ContainsKey(sceneId)) {
                LoadSceneClientRpc(sceneId, position, reference, param);
                _sceneDistribution[sceneId].Add(senderId);
                return;
            }

            string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneId);
            string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            
            _sceneDistribution.Add(sceneId, new List<ulong> {senderId});
            LoadSceneClientRpc(sceneId, position, reference, param);
        }
        
        [ClientRpc]
        private void LoadSceneClientRpc(int sceneId, Vector3 position, NetworkObjectReference playerObject, ClientRpcParams clientRpcParams = default) {
            playerObject.TryGet(out var obj);
            obj.GetComponent<PlayerController>().LoadScene(sceneId, position);
        }

        public void TransferToScene(int sceneId, Vector3 position, ulong senderId) {
            ClientRpcParams param = new ClientRpcParams {
                Send = {
                    TargetClientIds = new[] { senderId }
                }
            };
            var client = NetworkManager.ConnectedClients.First(id => id.Key == senderId).Value;
            var reference = client.PlayerObject.GetComponent<NetworkObjectReference>();
            if (_sceneDistribution.ContainsKey(sceneId)) {
                OnPlayerLeaveScene(senderId);
                LoadSceneClientRpc(sceneId, position, reference, param);
                _sceneDistribution[sceneId].Add(senderId);
                return;
            }

            string sceneName = SceneManager.GetSceneByBuildIndex(sceneId).name;
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            _sceneDistribution.Add(sceneId, new List<ulong> {senderId});
            LoadSceneClientRpc(sceneId, position, reference, param);
        }

        private int GetSceneIdByClientId(ulong id) {
            foreach (var scene in _sceneDistribution.Where(pair => pair.Value.Contains(id))) {
                return scene.Key;
            }
            return -1;
        }
    }
}
