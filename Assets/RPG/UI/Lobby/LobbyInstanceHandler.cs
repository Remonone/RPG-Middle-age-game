using RPG.Lobby;
using RPG.Network.Management;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RPG.UI.Lobby {
    public class LobbyInstanceHandler : NetworkBehaviour {
        [SerializeField] private UIDocument _document;
        [SerializeField] private LobbyProcessor _processor;

        private VisualElement _root;
        
        // DATA

        private Button _quitButton;
        private Label _roomName;
        private Label _roomMap;
        private Label _roomLevel;
        private Label _roomHost;
        
        // CHAT
        private TextField _chatInput;
        private Button _chatSend;
        private VisualElement _chatContainer;
        private ListView _chatHistory;
        
        // CONTROLLER
        private VisualElement _controller;
        private Button _startGame;
        

        public void Initialize() {
            InitInputs();
            _processor.Room.OnValueChanged += OnDataChanged;
            SetRoomData(_processor.Room.Value);
            _processor.OnMessage += OnChatUpdate;
        }
        private void OnDataChanged(RoomData previousvalue, RoomData newvalue) {
            SetRoomData(newvalue);
        }
        private void SetRoomData(RoomData processorRoom) {
            _roomName.text = processorRoom.RoomName.Value;
            _roomHost.text = processorRoom.RoomHost.Value;
            _roomMap.text = processorRoom.RoomMap.Value;
            _roomLevel.text = $"{processorRoom.RoomLevel}";
        }

        private void OnChatUpdate() {
            _chatHistory.Rebuild();
        }
        private void InitInputs() {
            _root = _document.rootVisualElement;
            
            // CHAT
            _chatInput = _root.Q<TextField>("ChatInput");
            _chatSend = _root.Q<Button>("SendMessage");
            _chatContainer = _root.Q<VisualElement>("ChatContainer");
            Label CreateMessage() => new();
            _chatHistory = new ListView(_processor.Messages, 24, CreateMessage, BindItem);
            _chatHistory.selectionType = SelectionType.None;
            _chatContainer.Add(_chatHistory);
            _chatSend.clicked += OnSendMessage;
            _controller = _root.Q<VisualElement>("Controls");
            _startGame = _root.Q<Button>("Start");
            if (!NetworkManager.IsHost) {
                _controller.EnableInClassList("disabled", true);
            }
            else {
                _startGame.clicked += OnGameStart;
            }
            
            //DATA
            _roomName = _root.Q<Label>("Name");
            _roomMap = _root.Q<Label>("Map");
            _roomLevel = _root.Q<Label>("Level");
            _roomHost = _root.Q<Label>("Host");
            _quitButton = _root.Q<Button>("Quit");
            if (NetworkManager.IsHost) {
                _quitButton.clicked += CloseLobby;
            }
            else {
                _quitButton.clicked += OnQuit;
            }
        }
        private void OnGameStart() {
            var sceneName = _processor.Room.Value.RoomMap;
            NetworkManager.SceneManager.OnLoadComplete += OnSceneLoadComplete;
            NetworkManager.SceneManager.LoadScene(sceneName.Value, LoadSceneMode.Single);
            
        }
        private void OnSceneLoadComplete(ulong clientid, string scenename, LoadSceneMode loadscenemode) {
            _processor.InitPlayersServerRpc();
        }
        private void CloseLobby() {
            _processor.CloseLobbyServerRpc();
        }
        private void OnQuit() {
            
            var data = FindObjectOfType<ApplicationManager>().PlayerData;
            _processor.DisconnectFromLobbyServerRpc(data);
        }

        private void BindItem(VisualElement el, int index) {
            var label = el as Label;
            label.text = (string)_processor.Messages[index];
        }
        private void OnSendMessage() {
            if (_chatInput.value == "") return;
            FixedString512Bytes str = _chatInput.value;
            FixedString64Bytes username = FindObjectOfType<ApplicationManager>().PlayerData.Username;
            _processor.SendMessageServerRpc(username, str);
            _chatInput.value = "";
        }
    }

    public struct RoomData: INetworkSerializable {
        public FixedString64Bytes RoomName;
        public FixedString64Bytes RoomMap;
        public int RoomLevel;
        public FixedString64Bytes RoomHost;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
            serializer.SerializeValue(ref RoomHost);
            serializer.SerializeValue(ref RoomLevel);
            serializer.SerializeValue(ref RoomMap);
            serializer.SerializeValue(ref RoomName);
        }
    }
    
   
}
