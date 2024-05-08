using System;
using RPG.Lobby;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Lobby {
    public class LobbyHandler : MonoBehaviour {
        [SerializeField] private LobbyDataContainer _dataPackage;
        [SerializeField] private UIDocument _lobbyList;
        
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
        }

        private void OnEnable() {
            _dataPackage.OnUpdate += UpdateList;
            _isRefreshing = true;
            _dataPackage.UpdateList();
            _connectButton.clicked += OnConnectPerformed;
            _refreshButton.clicked += OnRefreshPerformed;
            _backButton.clicked += OnBackPerformed;
        }
        private void OnBackPerformed() {
            throw new NotImplementedException();
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
            throw new NotImplementedException();
        }

        private void UpdateList() {
            foreach (var lobby in _dataPackage.Lobbies) {
                
            }

            _isRefreshing = false;
        }
    }
}
