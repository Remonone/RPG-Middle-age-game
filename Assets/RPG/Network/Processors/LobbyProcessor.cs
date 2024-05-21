using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RPG.Creatures.Player;
using RPG.Lobby;
using RPG.Network.Controllers;
using RPG.Network.Management;
using RPG.Network.Scenes;
using RPG.UI.Lobby;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace RPG.Network.Processors {
    public class LobbyProcessor : NetworkBehaviour {
        private LobbyPack _pack;
        private List<string> _messages = new();
        [FormerlySerializedAs("_manager")] [SerializeField] private GameProcessor _processor;

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
                SessionID = pack.SessionId
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
            var player = NetworkManager.Singleton.ConnectedClients[serverRpcParams.Receive.SenderClientId].PlayerObject;
            player.GetComponent<PlayerController>().DisableClientRpc();
            player.gameObject.SetActive(false);
            DistributeMessageToClientRpc($"Player {data.Username} has join the lobby.");
        }

        [ServerRpc(RequireOwnership = false)]
        public void DisconnectFromLobbyServerRpc(PlayerData data, ServerRpcParams serverRpcParams = default) {
            if (!_players.Contains(data)) return;
            foreach(var player in _players) {
                if (player.PlayerID != data.PlayerID) continue;
                _players.Remove(player);
                
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
                NetworkManager.DisconnectClient(client);
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
        
        [ServerRpc(RequireOwnership = false)]
        public void InitPlayersServerRpc() {
            foreach (var player in NetworkManager.ConnectedClients.Values) {
                player.PlayerObject.Spawn();
            }
            // foreach (var client in _players) {
            //     var obj = NetworkManager.ConnectedClients.Values.First(obj =>
            //         obj.PlayerObject.GetComponent<PlayerController>().Data.PlayerID == client.PlayerID);
            //     StartCoroutine(FindObjectOfType<SavingWrapper>().System.Load(obj.PlayerObject.gameObject, client.PlayerID.Value, _pack.SessionId,
            //         data => {
            //             var content = data["content"];
            //             if (content == null) return;
            //             
            //             var sceneIndex = (int)content[SCENE_INDEX];
            //             var sceneName = SceneManager.GetSceneByBuildIndex(sceneIndex).name;
            //             if (Room.Value.RoomMap != sceneName) data["content"]["RPG.Movement.Mover"].Remove();
            //             InitPlayer(obj.PlayerObject, data);
            //         }));
            // }
        }

        // private void InitPlayer(NetworkObject player, JObject data) {
        //     player.GetComponent<SaveableEntity>().RestoreFromJToken(data);
        //     player.gameObject.SetActive(true);
        //     player.GetComponent<PlayerController>().();
        // }
        
        [ServerRpc(RequireOwnership = false)]
        public void StartGameServerRpc() {
            var sceneName = Room.Value.RoomMap;
            Instantiate(_processor);
            NetworkManager.SceneManager.OnLoadComplete += OnSceneLoadComplete;
            FindObjectOfType<SceneDistributor>().TransferToDifferentSceneServerRpc(sceneName.Value);
        }

        public void OnSceneLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode) {
            FindObjectOfType<GameProcessor>().InitPlayers();
        }
    }
}
