using System.Collections.Generic;
using RPG.Lobby;
using RPG.UI.Elements.LobbyElement;
using RPG.UI.Elements.LobbyElement.LobbyConnect;
using RPG.Utils.Constants;
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
        private VisualElement _listContainer;

        private List<LobbyPack> _lobbies = new();

        private ListView _view;

        private bool _isRefreshing;
        
        LobbyElement CreateItem() => new();

        private void Awake() {
            _root = _lobbyList.rootVisualElement;

            _connectButton = _root.Q<Button>("Connect");
            _refreshButton = _root.Q<Button>("Refresh");
            _backButton = _root.Q<Button>("Back");
            _listContainer = _root.Q<VisualElement>("Container");
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
            if (_view.selectedIndex < 0) return;
            _selectedLobby = _lobbies[_view.selectedIndex];
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
        private void OnConnectToLobby(string id, string password) {
            _dataPackage.ConnectToLobby(id, password);
        }

        private void UpdateList() {
            _lobbies.Clear();
            foreach (var lobby in _dataPackage.Lobbies) {
                
                _lobbies.Add(lobby);
            }

            BuildListView();
            _isRefreshing = false;
        }
        private void BuildListView() {
            if(!ReferenceEquals(_view, null)) _view.RemoveFromHierarchy();
            _view = new ListView(_lobbies, PropertyConstants.ITEM_HEIGHT, CreateItem, BindItem);
            _listContainer.Add(_view);
        }
        private void BindItem(VisualElement element, int index) {
            var el = element as LobbyElement;
            var lobby = _lobbies[index];
            el.IsSecured = lobby.IsSecured;
            el.RoomName = lobby.RoomName;
            el.RoomMap = lobby.MapName;
            el.PlayersAmount = lobby.PlayerCount;
            el.RoomHost = lobby.HostName;
            el.RoomLevel = lobby.Level;
        }
    }
}
