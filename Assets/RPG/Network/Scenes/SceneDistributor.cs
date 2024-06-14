using System;
using System.Linq;
using RPG.Movement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

namespace RPG.Network.Scenes {
    public class SceneDistributor: NetworkBehaviour {
        
        private void Start() {
            DontDestroyOnLoad(this);
        }

        public void LoadSceneOnDisconnect(string sceneName) {
            if (NetworkManager.IsConnectedClient) return;
            SceneManager.LoadScene(sceneName);
        }

        [ServerRpc(RequireOwnership = false)]
        public void TransferToDifferentSceneServerRpc(string sceneName,  ServerRpcParams serverRpcParams = default) {
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }

        [ServerRpc(RequireOwnership = false)]
        public void TransferToDifferentSceneWithCoordsServerRpc(string sceneName, Vector3 coords,
            ServerRpcParams serverRpcParams = default) {
            NetworkManager.SceneManager.OnLoadComplete += (clientId, sceneName, sceneMode) => {
                var players = NetworkManager.Singleton.ConnectedClients.Values.Select(client => client.PlayerObject);
                foreach (var player in players) {
                    player.GetComponent<NavMeshAgent>().Warp(coords);
                }
            };
            NetworkManager.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
        
    }
}