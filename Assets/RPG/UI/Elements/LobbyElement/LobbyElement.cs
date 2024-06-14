using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RPG.UI.Elements.LobbyElement {
    public class LobbyElement : VisualElement {
        
        private Action<Label, string> _onDataChanged;

        private string _roomName;
        private string _roomMap;
        private string _roomHost;
        private int _playersAmount;
        private int _roomLevel;
        private bool _isSecured;
        
        public string RoomName {
            get => _roomName;
            set {
                _roomName = value;
                _roomNameEl.text = value;
            }
        }

        public int PlayersAmount {
            get => _playersAmount;
            set {
                _playersAmount = value;
                _playersAmountEl.text = $"{value}/4";
            }
        }

        public bool IsSecured {
            get => _isSecured;
            set {
                _isSecured = value;
                _isSecuredEl.EnableInClassList("lobby_secured", value);
            }
        }

        public string RoomMap {
            get => _roomMap;
            set {
                _roomMap = value;
                _roomInfoEl.text = $"{value}({_roomLevel})";
            }
        }

        public string RoomHost {
            get => _roomHost;
            set {
                _roomHost = value;
                _roomHostEl.text = value;
            }
        }

        public int RoomLevel {
            get => _roomLevel;
            set {
                _roomLevel = value;
                _roomInfoEl.text = $"{_roomMap}({value})";
            }
        }

        public ulong RoomID { get; set; }
        
        
        
        private readonly VisualElement _isSecuredEl = new();
        private readonly Label _roomNameEl = new();
        private readonly Label _roomInfoEl = new();
        private readonly Label _roomHostEl = new();
        private readonly Label _playersAmountEl = new();

        public LobbyElement() {
            AddToClassList("lobby_line");
            Add(_isSecuredEl);
            _isSecuredEl.AddToClassList("lobby_secured");
            _isSecuredEl.AddToClassList(IsSecured ? "secured" : "");
            Add(_roomNameEl);
            _roomNameEl.text = RoomName;
            Add(_roomInfoEl);
            _roomInfoEl.text = $"{RoomMap}({RoomLevel})";
            Add(_roomHostEl);
            _roomHostEl.text = RoomHost;
            
            Add(_playersAmountEl);
            _playersAmountEl.text = $"{PlayersAmount}/4";
        }
        
        public new class UxmlFactory : UxmlFactory<LobbyElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            private UxmlStringAttributeDescription _roomName = new() { name = "room_name" };
            private UxmlStringAttributeDescription _roomMap = new() { name = "room_map" };
            private UxmlStringAttributeDescription _roomHost = new() { name = "room_host" };
            private UxmlLongAttributeDescription _roomID = new() { name = "room_id" };
            private UxmlBoolAttributeDescription _isSecured = new() { name = "is_secured" };
            private UxmlIntAttributeDescription _roomPlayers = new() { name = "room_players", defaultValue = 1};
            private UxmlIntAttributeDescription _roomLevel = new() { name = "room_level" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get {yield break;}
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var lobbyElement = ve as LobbyElement;
                lobbyElement.Clear();
                
                lobbyElement.AddToClassList("lobby_line");

                lobbyElement.IsSecured = _isSecured.GetValueFromBag(bag, cc);
                var secured = new VisualElement();
                lobbyElement.Add(secured);
                secured.AddToClassList("lobby_secured");
                secured.AddToClassList(lobbyElement.IsSecured ? "secured" : "");

                lobbyElement.RoomName = _roomName.GetValueFromBag(bag, cc);
                lobbyElement.Add(new Label(lobbyElement.RoomName));

                lobbyElement.RoomMap = _roomMap.GetValueFromBag(bag, cc);
                lobbyElement.RoomLevel = _roomLevel.GetValueFromBag(bag, cc);
                lobbyElement.Add(new Label(lobbyElement.RoomMap + "(" + lobbyElement.RoomLevel + ")"));

                lobbyElement.RoomHost = _roomHost.GetValueFromBag(bag, cc);
                lobbyElement.Add(new Label(lobbyElement.RoomHost));

                lobbyElement.PlayersAmount = _roomPlayers.GetValueFromBag(bag, cc);
                lobbyElement.Add(new Label(lobbyElement.PlayersAmount + " / 4"));

                lobbyElement.RoomID = (ulong)_roomID.GetValueFromBag(bag, cc);
            }
        }
    }
}
