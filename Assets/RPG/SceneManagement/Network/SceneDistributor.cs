using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.SceneManagement.Network {
    public class SceneDistributor : NetworkBehaviour {

        private Dictionary<int, ulong> _sceneDistribution;

        private void Start() {
            if (!IsServer) return;
            DontDestroyOnLoad(this);
        }

        private void Awake() {
            NetworkManager.OnClientDisconnectCallback += OnPlayerDisconnect;
        }
        private void OnPlayerDisconnect(ulong obj) {
            
        }

        public void LoadPlayerOnScene(int sceneId, Vector3 position) {
            if (!IsClient) return;
            LoadSceneOnServerRpc(sceneId, position);
        }

        [ServerRpc]
        public void LoadSceneOnServerRpc(int sceneId, Vector3 position) {
            if (_sceneDistribution.ContainsKey(sceneId)) {
                LoadSceneClientRpc(sceneId, position);
            }
        }
        
        [ClientRpc]
        private void LoadSceneClientRpc(int sceneId, Vector3 position) {
            SceneManager.LoadScene(sceneId);
        }

    }
}
