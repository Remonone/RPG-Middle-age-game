using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Lobby;
using RPG.UI.Lobby;
using Unity.Collections;
using Unity.Netcode;

namespace RPG.Network.Processors {
    public class LobbyProcessor : NetworkBehaviour {
        private List<string> _messages = new();

        private string _token;
        
        public NetworkVariable<RoomData> Room = new();
        private NetworkList<PlayerData> _players;

        public IList Messages => _messages;
        public Action OnMessage;
        private string _onLoad;

        private void Awake() {
            _players = new NetworkList<PlayerData>();
        }

        public void Init(LobbyPack pack) {
            RoomData data = new RoomData {
                RoomHost = pack.HostName,
                RoomLevel = pack.Level,
                RoomMap = pack.MapName,
                RoomName = pack.RoomName,
                SessionID = pack.SessionId,
                RoomID = pack.RoomID,
            };
            Room.Value = data;
        }

        public void AddPlayerToLobby(PlayerData data) {
            if (!IsHost || !IsServer) return;
            _players.Add(data);
            DistributeMessageToClientRpc($"Player {data.Username} has join the lobby.");
        }
        public void DisconnectPlayerFromLobby(PlayerData data) {
            if (!IsHost || !IsServer) return;
            if (!_players.Contains(data)) return;
            foreach(var player in _players) {
                if (player.PlayerID != data.PlayerID) continue;
                _players.Remove(player);
                DistributeMessageToClientRpc($"Player {player.Username} left the lobby.");
                break;
            }
        }


        [ServerRpc(RequireOwnership = false)]
        public void SendMessageServerRpc(FixedString64Bytes sender, FixedString512Bytes str) {
            var result = $"{sender.Value}: {str.Value}";
            DistributeMessageToClientRpc(result);
        }
        
        [ClientRpc]
        private void DistributeMessageToClientRpc(FixedString512Bytes result) {
            _messages.Add(result.Value);
            OnMessage?.Invoke();
        }
    }
}
