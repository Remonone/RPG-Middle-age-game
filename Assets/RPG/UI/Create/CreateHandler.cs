using System;
using Newtonsoft.Json.Linq;
using RPG.Lobby;
using RPG.Network.Controllers;
using RPG.Network.Management.Managers;
using RPG.Network.Model;
using RPG.UI.Elements.Input;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Create {
    public class CreateHandler : MonoBehaviour {
        [SerializeField] private UIDocument _document;
        [SerializeField] private GameObject _mainMenu;
        
        private VisualElement _root;
        private VisualElement _room;
        private ListView _selection;
        private Button _selectWorld;
        private Button _createWorld;
        private Button _backButton;
        private Button _startButton;
        private ValueInput _roomName;
        private ValueInput _password;
        private Toggle _securedButton;
        private Label _mapInfo;
        private Label _levelInfo;

        private SessionData _selectedWorld;
        private bool _isSelected;

        public void Awake() {
            _root = _document.rootVisualElement;
            _room = _root.Q<VisualElement>("Room");
            _selection = _root.Q<ListView>();
            _selectWorld = _root.Q<Button>("Select");
            _createWorld = _root.Q<Button>("Create");
            _backButton = _root.Q<Button>("Back");
            _startButton = _root.Q<Button>("Start");
            _roomName = _root.Q<ValueInput>("RoomName");
            _securedButton = _root.Q<Toggle>("IsSecured");
            _password = _root.Q<ValueInput>("Password");
            _mapInfo = _root.Q<Label>("Map");
            _levelInfo = _root.Q<Label>("Level");
        }

        private void OnEnable() {
            _backButton.clicked += OnBackButton;
            _selectWorld.clicked += OnWorldSelect;
            _startButton.clicked += OnWorldStart;
        }
        private void OnWorldStart() {
            if (ReferenceEquals(_selectedWorld, null) || !_isSelected) return;
            var token = FindObjectOfType<BaseManager>().Token;
            LobbyCreateData data = new LobbyCreateData(_selectedWorld.Id, token, _password.Value, _roomName.Value);
            StartCoroutine(LobbyController.CreateLobby(data, OnSuccess, OnFail));
        }
        private void OnSuccess(LobbyPack obj) {
            // TODO: Transfer to lobby
        }
        private void OnFail(JToken obj) {
            // TODO: Print error
        }
        private void OnWorldSelect() {
            if (ReferenceEquals(_selectedWorld, null)) return;
            _room.RemoveFromClassList("disabled");
            _isSelected = true;
            SetRoomData();
        }
        
        private void SetRoomData() {
            _mapInfo.text = _selectedWorld.SessionMap;
            _levelInfo.text = $"{_selectedWorld.Level}";
        }
        
        private void OnBackButton() {
            gameObject.SetActive(false);
            _mainMenu.SetActive(true);
            ResetSelection();
        }
        private void ResetSelection() {
            _selectedWorld = null;
            _room.EnableInClassList("disabled", true);
            _isSelected = false;
        }
    }
}
