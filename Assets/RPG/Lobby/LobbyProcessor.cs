using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using RPG.Core;
using RPG.Creatures.Player;
using RPG.Network.Controllers;
using RPG.Network.Management;
using RPG.UI.Lobby;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;

using static RPG.Utils.Constants.DataConstants;

namespace RPG.Lobby {
    public class LobbyProcessor : NetworkBehaviour {
        private LobbyPack _pack;
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
            _pack = pack;
            RoomData data = new RoomData {
                RoomHost = pack.HostName,
                RoomLevel = pack.Level,
                RoomMap = pack.MapName,
                RoomName = pack.RoomName,
            };
            Room.Value = data;
            _token = FindObjectOfType<ApplicationManager>().Token;
        }

        private void OnDisable() {
            if (!IsServer) return;
            var _ = LobbyController.UnloadLobby(_token, _pack.RoomID);
        }

        [ServerRpc(RequireOwnership = false)]
        public void ConnectToLobbyServerRpc(PlayerData data, ServerRpcParams serverRpcParams = default) {
            if (_players.Contains(data)) return;
            _players.Add(data);
            NetworkManager.Singleton.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject.gameObject.SetActive(false);
            DistributeMessageToClientRpc($"Player {data.Username} has join the lobby.");
        }

        [ServerRpc(RequireOwnership = false)]
        public void DisconnectFromLobbyServerRpc(PlayerData data, ServerRpcParams serverRpcParams = default) {
            if (!_players.Contains(data)) return;
            foreach(var player in _players) {
                if (player.PlayerID != data.PlayerID) continue;
                _players.Remove(player);
                NetworkManager.Singleton.DisconnectClient(serverRpcParams.Receive.SenderClientId);
                DistributeMessageToClientRpc($"Player {player.Username} left the lobby.");
                break;
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void CloseLobbyServerRpc(ServerRpcParams serverRpcParams = default) {
            var ids = NetworkManager.ConnectedClients.Values.Select(client => client.ClientId).ToArray();
            var sender = serverRpcParams.Receive.SenderClientId;
            foreach (var client in ids) {
                if (client == sender) continue;
                NetworkManager.DisconnectClient(client, "Lobby has been stopped...");
            }

            StartCoroutine(LobbyController.UnloadLobby(FindObjectOfType<ApplicationManager>().Token, _pack.RoomID));
            NetworkManager.DisconnectClient(sender);
            SceneManager.LoadScene("Main");
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
        
        [ServerRpc]
        public void InitPlayersServerRpc() {
            foreach (var client in _players) {
                var obj = NetworkManager.ConnectedClients.Values.First(obj =>
                    obj.PlayerObject.GetComponent<PlayerController>().Data.PlayerID == client.PlayerID);
                StartCoroutine(FindObjectOfType<SavingWrapper>().System.Load(obj.PlayerObject.gameObject, client.PlayerID.Value, _pack.SessionId, OnLoad));
            }
        }
        private void OnLoad(JObject obj) {
            
        }


        private void OnData(JToken obj) {
            var content = obj["content"];
            if (content == null) return;
            var sceneIndex = (int)content[SCENE_INDEX];
            var sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            if (Room.Value.RoomMap != sceneName) return;
            
        }
    }
}
