using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Network.Controllers;
using RPG.Network.Management;
using RPG.Network.Model;
using UnityEngine;

namespace RPG.Lobby {
    public class LobbyDataContainer : MonoBehaviour {

        private ApplicationManager _manager;
        
        private List<LobbyPack> _lobbies;

        public IEnumerable<LobbyPack> Lobbies => _lobbies;

        public Action OnUpdate;


        private void Awake() {
            _manager = FindObjectOfType<ApplicationManager>();
        }

        public void UpdateList() {
            StartCoroutine(LobbyController.GetLobbyList(OnLoad, OnFail));
        }
        private void OnFail(string error) {
            _lobbies = new List<LobbyPack>();
            // TODO: popup
            Debug.Log(error);
            OnUpdate?.Invoke();
        }
        private void OnLoad(List<LobbyPack> lobbies) {
            _lobbies = lobbies;
            OnUpdate?.Invoke();
        }

        public void ConnectToLobby(string lobbyId) {
            StartCoroutine(LobbyController.JoinLobby(new LobbyPayload { RoomID = lobbyId }, OnJoin, OnJoinFailed));
        }
        
        public void ConnectToLobby(string lobbyId, string password) {
            StartCoroutine(LobbyController.JoinLobby(new LobbyPayload { RoomID = lobbyId, Password = password}, OnJoin, OnJoinFailed));
        }
        
        private void OnJoin(string obj) {
            var connectionData = JToken.Parse(obj);
            string ip = (string)connectionData["server_ip"];
            ushort port = (ushort)connectionData["server_port"];
            _manager.ConnectToServer(ip, port);
            
        }
        private void OnJoinFailed(string obj) {
            // TODO: popup
            Debug.Log(JToken.Parse(obj)["error_message"]);
        }
    }
}
