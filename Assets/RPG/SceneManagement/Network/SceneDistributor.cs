using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement.Network {
    public class SceneDistributor : NetworkBehaviour {

        private readonly Dictionary<int, List<ulong>> _sceneDistribution = new();

        private void Start() {
            DontDestroyOnLoad(this);
        }

        private void Awake() {
            NetworkManager.OnClientDisconnectCallback += OnPlayerLeaveScene;
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
            if (_sceneDistribution.ContainsKey(sceneId)) {
                LoadSceneClientRpc(sceneId, position, param);
                _sceneDistribution[sceneId].Add(senderId);
                return;
            }

            string sceneName = SceneManager.GetSceneByBuildIndex(sceneId).name;
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            _sceneDistribution.Add(sceneId, new List<ulong> {senderId});
            LoadSceneClientRpc(sceneId, position, param);
        }
        
        [ClientRpc]
        private void LoadSceneClientRpc(int sceneId, Vector3 position, ClientRpcParams clientRpcParams = default) {
            SceneManager.LoadSceneAsync(sceneId).completed += (obj) => {
                var client = NetworkManager.ConnectedClients.First(id => id.Key == NetworkManager.LocalClientId).Value;
                client.PlayerObject.transform.position = position;
            };
            
        }

        public void TransferToScene(int sceneId, Vector3 position, ulong senderId) {
            ClientRpcParams param = new ClientRpcParams {
                Send = {
                    TargetClientIds = new[] { senderId }
                }
            };
            if (_sceneDistribution.ContainsKey(sceneId)) {
                OnPlayerLeaveScene(senderId);
                TransferToSceneClientRpc(sceneId, position, param);
                _sceneDistribution[sceneId].Add(senderId);
                return;
            }

            string sceneName = SceneManager.GetSceneByBuildIndex(sceneId).name;
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
            _sceneDistribution.Add(sceneId, new List<ulong> {senderId});
            TransferToSceneClientRpc(sceneId, position, param);
        }

        private int GetSceneIdByClientId(ulong id) {
            foreach (var scene in _sceneDistribution.Where(pair => pair.Value.Contains(id))) {
                return scene.Key;
            }
            return -1;
        }

        [ClientRpc]
        private void TransferToSceneClientRpc(int sceneId, Vector3 position, ClientRpcParams clientRpcParams = default) {
            SceneManager.LoadSceneAsync(sceneId).completed += (obj) => {
                var client = NetworkManager.ConnectedClients.First(id => id.Key == NetworkManager.LocalClientId).Value;
                client.PlayerObject.transform.position = position;
            };
        }

    }
}
