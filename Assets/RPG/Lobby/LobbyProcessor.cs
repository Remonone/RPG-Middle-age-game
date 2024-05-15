using System;
using RPG.Network.Controllers;
using RPG.Network.Management.Managers;
using UnityEngine;

namespace RPG.Lobby {
    public class LobbyProcessor : MonoBehaviour {
        private LobbyPack _loadedLobby;

        public Action<LobbyPack> OnDataLoaded;

        public void Init(LobbyPack pack) {
            _loadedLobby = pack;
            OnDataLoaded?.Invoke(pack);
        }

        private void OnDestroy() {
            var token = FindObjectOfType<BaseManager>().Token;
            StartCoroutine(LobbyController.UnloadLobby(token, _loadedLobby.RoomID));
        }
    }
}
