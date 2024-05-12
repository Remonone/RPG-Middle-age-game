using System;
using RPG.Lobby;
using RPG.UI.Elements.LobbyElement;
using RPG.UI.Elements.LobbyElement.LobbyConnect;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Lobby {
    public class LobbyHandler : MonoBehaviour {
        [SerializeField] private LobbyDataContainer _dataPackage;
        [SerializeField] private UIDocument _lobbyList;
        [SerializeField] private GameObject _mainMenu;
        
        private LobbyPack _selectedLobby;
        private VisualElement _root;

        private Button _connectButton;
        private Button _refreshButton;
        private Button _backButton;
        private ListView _listView;

        private bool _isRefreshing;

        private void Awake() {
            _root = _lobbyList.rootVisualElement;

            _connectButton = _root.Q<Button>("Connect");
            _refreshButton = _root.Q<Button>("Refresh");
            _backButton = _root.Q<Button>("Back");
            _listView = _root.Q<ListView>("List");
        }

        private void OnEnable() {
            _dataPackage.OnUpdate += UpdateList;
            _isRefreshing = true;
            _dataPackage.UpdateList();
            _connectButton.clicked += OnConnectPerformed;
            _refreshButton.clicked += OnRefreshPerformed;
            _backButton.clicked += OnBackPerformed;
        }

        private void OnDisable() {
            _dataPackage.OnUpdate -= UpdateList;
            _connectButton.clicked -= OnConnectPerformed;
            _refreshButton.clicked -= OnRefreshPerformed;
            _backButton.clicked -= OnBackPerformed;
        }

        private void OnBackPerformed() {
            _mainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
        private void OnRefreshPerformed() {
            if (_isRefreshing) return;
            _isRefreshing = true;
            _dataPackage.UpdateList();
        }
        private void OnConnectPerformed() {
            if (ReferenceEquals(_selectedLobby, null)) return;
            if (_selectedLobby.IsSecured) {
                PerformSecuredLobby(_selectedLobby);
                return;
            }

            _dataPackage.ConnectToLobby(_selectedLobby.RoomID);
        }
        private void PerformSecuredLobby(LobbyPack selectedLobby) {
            var connectElement = new LobbyConnect {
                RoomID = selectedLobby.RoomID
            };
            connectElement.OnConnect += OnConnectToLobby;
            _root.Add(connectElement);
        }
        private void OnConnectToLobby(ulong id, string password) {
            _dataPackage.ConnectToLobby(id, password);
        }

        private void UpdateList() {
            foreach (var lobby in _dataPackage.Lobbies) {
                var lobbyEl = new LobbyElement {
                    IsSecured = lobby.IsSecured,
                    RoomName = lobby.RoomName,
                    RoomMap = lobby.MapName,
                    PlayersAmount = lobby.PlayerCount,
                    RoomHost = lobby.HostName
                };
                _listView.Add(lobbyEl);
            }
            _listView.Rebuild();
            _isRefreshing = false;
        }
    }
}
