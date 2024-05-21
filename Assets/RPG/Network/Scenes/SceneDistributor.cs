using System;
using Unity.Netcode;
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
        
    }
}