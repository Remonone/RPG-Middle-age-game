using System;
using System.Linq;
using RPG.Core;
using RPG.Lobby;
using RPG.Network.Controllers;
using RPG.Network.Model;
using RPG.Network.Processors;
using RPG.Saving;
using RPG.Stats;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using NetworkPlayer = RPG.Creatures.Player.NetworkPlayer;

namespace RPG.Network.Management {
    [RequireComponent(typeof(NetworkManager))]
    public class ApplicationManager : MonoBehaviour {

        private LobbyProcessor _processor;
        private PlayerData _data;
        private ServerState _state;
        private LobbyPack _pack;
        
        public static ApplicationManager Instance;

        public string IP { get; set; }
        public ushort Port { get; set; }
        
        public string Token { get; set; }
        
        public string SessionID { get; set; }

        public ServerState State => _state;
        
        public PlayerData PlayerData {
            get => _data;
            set {
                if (!string.IsNullOrEmpty(_data.PlayerID.Value)) return;
                _data = value;
            }
        }

        private void Awake() {
            if (Instance != null) {
                Debug.LogWarning("There is several Application Manager instances on the scene");
                return;
            }
            Instance = this;
        }

        public void HostServer(LobbyPack pack) { // Lobby creation
            if (_state != ServerState.NONE) return;
            SetConnectionData(IP, Port);
            SessionID = pack.SessionId;
            _pack = pack;
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnect;
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.OnLoadComplete += SceneManagerOnOnLoadComplete;
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            _state = ServerState.INITIATED;
        }

        private void SceneManagerOnOnLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode) {
            _processor = FindObjectOfType<LobbyProcessor>();
            _processor.Init(_pack);
            _processor.AddPlayerToLobby(PlayerData);
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= SceneManagerOnOnLoadComplete;
        }

        private void SetConnectionData(string ip, ushort port) {
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ip, port);
        }

        private void OnClientConnect(ulong clientId) {
            var client = NetworkManager.Singleton.ConnectedClients[clientId];
            var player = client.PlayerObject;
            player.GetComponent<NetworkPlayer>().SwitchPlayerStateClientRpc(false);
            var playerInfo = player.GetComponent<NetworkPlayer>()._playerInfo.Value;
            if(!ReferenceEquals(_processor, null))
                _processor.AddPlayerToLobby(playerInfo);
            player.GetComponent<SaveableEntity>().Init(playerInfo.PlayerID.Value);
        }


        public void ConnectToServer(string ip, ushort port) { // Lobby Connection
            if (_state != ServerState.NONE) return;
            SetConnectionData(ip, port);
            NetworkManager.Singleton.StartClient();
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnJoinSceneComplete;
            
            _state = ServerState.CONNECTED;
        }

        private void OnJoinSceneComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode) {
            _processor = FindObjectOfType<LobbyProcessor>();
            SessionID = _processor.Room.Value.SessionID.Value;
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnJoinSceneComplete;
        }

        public void SwitchPlayerState(ServerState state, ulong clientId) {
            if (NetworkManager.Singleton.LocalClientId == clientId) {
                _state = state;
            }
        }

        private void Start() {
            Application.wantsToQuit += ApplicationOnWantsToQuit;
            NetworkManager.Singleton.GetComponent<UnityTransport>().OnTransportEvent += OnTransportEvent;
        }

        private void OnTransportEvent(NetworkEvent eventType, ulong clientId, ArraySegment<byte> payload, float receivetime) {
            if (eventType == NetworkEvent.Disconnect) {
                SceneManager.LoadScene("Main");
            }
        }

        public void DisconnectFromServer() {
            ApplicationOnWantsToQuit();
        }

        private bool ApplicationOnWantsToQuit() {
            if (_state == ServerState.NONE) return true;
            var manager = NetworkManager.Singleton;
            if (_state == ServerState.INITIATED) {
                StartCoroutine(LobbyController.UnloadLobby(Token, _processor.Room.Value.RoomID.Value));
            }
            if (manager.IsHost) {
                var hostId = manager.LocalClientId;
                var ids = manager.ConnectedClients.Values
                    .Where(client => client.ClientId != hostId)
                    .Select(client => client.ClientId)
                    .ToArray();
                foreach (var client in ids) {
                    if(_state == ServerState.STARTED) SavePlayerToDatabase(client);
                    manager.DisconnectClient(client);
                }

                if (_state == ServerState.STARTED) {
                    UpdateSessionInfo(hostId);
                    SavePlayerToDatabase(hostId);
                }
                manager.Shutdown();
                SceneManager.LoadScene("Main");
            } else {
                manager.LocalClient.PlayerObject.GetComponent<NetworkPlayer>().DisconnectPlayerFromServerRpc();
            }
            _state = ServerState.NONE;
            return true;
        }

        private void UpdateSessionInfo(ulong hostId) {
            string scene = SceneManager.GetActiveScene().name;
            int level = NetworkManager.Singleton.ConnectedClients[hostId].PlayerObject.GetComponent<BaseStats>().Level;
            UpdateSession session = new(scene, level);
            StartCoroutine(SessionController.UpdateSession(session, Token, SessionID));
        }

        public void SavePlayerToDatabase(ulong clientId) {
            var entity = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject.GetComponent<SaveableEntity>(); 
            FindObjectOfType<SavingWrapper>().System.Save(entity, Token, SessionID);
        }

        public void StartGame() {
            if (_state != ServerState.INITIATED) return;
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnStartGameFinished;
            NetworkManager.Singleton.SceneManager.LoadScene(_processor.Room.Value.RoomMap.Value, LoadSceneMode.Single);
            
            StartCoroutine(LobbyController.UnloadLobby(Token, _processor.Room.Value.RoomID.Value));
        }

        private void OnStartGameFinished(ulong clientid, string scenename, LoadSceneMode loadscenemode) {
            var players = NetworkManager.Singleton.ConnectedClients.Values;
            foreach (var player in players) {
                InitPlayer(player, SessionID);
            }
        }

        private void InitPlayer(NetworkClient player, string sessionID) {
            var system = FindObjectOfType<SavingWrapper>().System;
            var playerObj = player.PlayerObject;
            var entity = playerObj.GetComponent<SaveableEntity>();
            var networkPlayer = playerObj.GetComponent<NetworkPlayer>();
            var playerData = networkPlayer._playerInfo.Value;
            playerObj.gameObject.SetActive(true);
            StartCoroutine(system.Load(gameObject, playerData.PlayerID.Value, sessionID, data => {
                ClientRpcParams param = new ClientRpcParams {
                    Send = {
                        TargetClientIds = new[] { player.ClientId }
                    }
                };
                entity.RestoreFromJToken(data);
                networkPlayer.InitPlayerClientRpc(data.ToString(), param);
            }));
        }
    }

    public enum ServerState {
        NONE,
        INITIATED,
        CONNECTED,
        STARTED
    }
}
