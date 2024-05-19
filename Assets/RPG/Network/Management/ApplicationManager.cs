using System;
using RPG.Lobby;
using RPG.UI.Lobby;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RPG.Network.Management {
    [RequireComponent(typeof(NetworkManager))]
    public class ApplicationManager : MonoBehaviour {
        private bool _isConnected;
        private PlayerData _data;

        public string IP { get; set; }
        public ushort Port { get; set; }
        
        public string Token { get; set; }
        
        public PlayerData PlayerData {
            get => _data;
            set {
                if (!string.IsNullOrEmpty(_data.PlayerID.Value)) return;
                _data = value;
            }
        }

        private LobbyPack _lobby;

        public void HostServer(LobbyPack pack) {
            if (_isConnected) return;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(IP, Port);
            NetworkManager.Singleton.StartHost();
            NetworkManager.Singleton.SceneManager.OnLoadComplete += OnSceneLoaded;
            _lobby = pack;
            NetworkManager.Singleton.SceneManager.LoadScene("Lobby", LoadSceneMode.Single);
            
        }
        private void OnSceneLoaded(ulong clientid, string scenename, LoadSceneMode loadscenemode) {
            FindObjectOfType<LobbyProcessor>().Init(_lobby);
            var playerObj = NetworkManager.Singleton.LocalClient.PlayerObject;
            playerObj.gameObject.SetActive(false);
            _isConnected = true;
            FindObjectOfType<LobbyInstanceHandler>().Initialize();
            NetworkManager.Singleton.SceneManager.OnLoadComplete -= OnSceneLoaded;
        }


        public void ConnectToServer(string ip, ushort port) {
            if (_isConnected) return;
            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetConnectionData(ip, port);
            NetworkManager.Singleton.StartClient();
            transport.OnTransportEvent += OnTransport;
        }
        private void OnTransport(NetworkEvent eventtype, ulong clientid, ArraySegment<byte> payload, float receivetime) {
            if (eventtype == NetworkEvent.Disconnect) {
                SceneManager.LoadScene("Main");
                _isConnected = false;
            }
        }

    }
}
