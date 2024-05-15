using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using RPG.Lobby;
using RPG.Network.Controllers;
using RPG.Network.Management;
using RPG.Network.Model;
using RPG.UI.Elements;
using RPG.UI.Elements.Input;
using RPG.Utils.Constants;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Create {
    public class CreateHandler : MonoBehaviour {
        [SerializeField] private UIDocument _document;
        [SerializeField] private GameObject _mainMenu;
        
        private VisualElement _root;
        private VisualElement _room;
        private VisualElement _worldCreate;
        private VisualElement _selectionContainer;
        private Button _selectWorld;
        private Button _createWorld;
        private Button _backButton;
        private Button _startButton;
        private Button _confirmWorld;
        private ValueInput _roomName;
        private ValueInput _password;
        private ValueInput _worldName;
        private Toggle _securedButton;
        private Label _mapInfo;
        private Label _levelInfo;

        private ListView _view;

        private SessionData _selectedWorld;

        private List<SessionData> _sessionList = new();
        WorldRow CreateItem() => new();

        private void OnEnable() {
            InitRoot();
            CreateSelectionList();
            _backButton.clicked += OnBackButton;
            _selectWorld.clicked += OnWorldSelect;
            _startButton.clicked += OnWorldStart;
            _createWorld.clicked += OnWorldInit;
            _confirmWorld.clicked += OnWorldCreate;
            FetchWorlds();
            _securedButton.RegisterValueChangedCallback(OnToggleChanged);
        }
        
        private void InitRoot() {
            _root = _document.rootVisualElement;
            _room = _root.Q<VisualElement>("Room");
            _selectionContainer = _root.Q<VisualElement>("Container");
            _selectWorld = _root.Q<Button>("Select");
            _createWorld = _root.Q<Button>("Create");
            _backButton = _root.Q<Button>("Cancel");
            _startButton = _root.Q<Button>("Start");
            _roomName = _root.Q<ValueInput>("RoomName");
            _securedButton = _root.Q<Toggle>("IsSecured");
            _password = _root.Q<ValueInput>("Password");
            _mapInfo = _root.Q<Label>("Map");
            _levelInfo = _root.Q<Label>("Level");
            _worldCreate = _root.Q<VisualElement>("WorldName");
            _worldName = _root.Q<ValueInput>("Name");
            _confirmWorld = _root.Q<Button>("Confirm");
        }
        
        private void CreateSelectionList() {
            _view = CreateView();
            _selectionContainer.Add(_view);
        }
        private ListView CreateView() {
            var view = new ListView(_sessionList, PropertyConstants.ITEM_HEIGHT, CreateItem, BindItemToList);
            view.selectionType = SelectionType.Single;
            return view;
        }

        private void BindItemToList(VisualElement element, int index) {
            var worldRow = element as WorldRow;
            var session = _sessionList[index];
            worldRow.Map = session.SessionMap;
            worldRow.Name = session.SessionName;
            worldRow.Level = session.Level;
        }

        private void OnDisable() {
            _backButton.clicked -= OnBackButton;
            _selectWorld.clicked -= OnWorldSelect;
            _startButton.clicked -= OnWorldStart;
            _createWorld.clicked -= OnWorldInit;
            _confirmWorld.clicked -= OnWorldCreate;
            _securedButton.UnregisterValueChangedCallback(OnToggleChanged);
        }
        private void FetchWorlds() {
            var token = FindObjectOfType<ApplicationManager>().Token;
            StartCoroutine(LobbyController.FetchWorlds(token, OnWorlds, OnCreationFailed));
        }
        private void OnWorlds(List<SessionData> sessions) {
            _sessionList = sessions;
            _view.RemoveFromHierarchy();
            CreateSelectionList();
        }
        private void OnWorldInit() {
            _worldCreate.EnableInClassList("disabled", false);
        }
        private void OnWorldCreate() {
            if (_worldName.Value == "") return;
            var token = FindObjectOfType<ApplicationManager>().Token;
            StartCoroutine(LobbyController.CreateGame(token, _worldName.Value, OnCreated, OnCreationFailed));
        }
        private void OnCreated(SessionData data) {
            _selectedWorld = data;
            _sessionList.Add(data);
            _view.RemoveFromHierarchy();
            CreateSelectionList();
            OpenWorldConfig();
        }
        private void OpenWorldConfig() {
            _worldCreate.EnableInClassList("disabled", true);
            _room.EnableInClassList("disabled", false);
        }
        private void OnCreationFailed(string obj) {
            Debug.Log(obj);
        }
        private void OnToggleChanged(ChangeEvent<bool> evt) {
            _password.EnableInClassList("disabled", !evt.newValue);
        }
        
        private void OnWorldStart() {
            if (ReferenceEquals(_selectedWorld, null)) return;
            var token = FindObjectOfType<ApplicationManager>().Token;
            LobbyCreateData data = new LobbyCreateData(_selectedWorld.Id, token, _password.Value, _roomName.Value);
            StartCoroutine(LobbyController.CreateLobby(data, OnSuccess, OnFail));
        }
        private void OnSuccess(LobbyPack obj) {
            FindObjectOfType<ApplicationManager>().HostServer(obj);
        }
        private void OnFail(JToken obj) {
            Debug.Log((string)obj["error_message"]);
        }
        private void OnWorldSelect() {
            if (_view.selectedIndex < 0) return;
            _room.RemoveFromClassList("disabled");
            _selectedWorld = _sessionList[_view.selectedIndex];
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
            _password.EnableInClassList("disabled", true);
        }
    }
}
