using RPG.Lobby;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Lobby {
    public class LobbyInstanceHandler : NetworkBehaviour {
        [SerializeField] private UIDocument _document;
        [SerializeField] private LobbyProcessor _processor;

        public NetworkVariable<string> RoomName = new();

        private void OnEnable() {
            if (!IsHost) return;
            _processor.OnDataLoaded += LoadLobby;
        }
        private void LoadLobby(LobbyPack obj) {
            RoomName.Value = obj.RoomName;
        }
    }
}
