using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RPG.UI.Elements.LobbyElement {
    public class LobbyElement : VisualElement {
        
        private Action<Label, string> _onDataChanged;
        
        public string RoomName { get; set; }
        public int PlayersAmount { get; set; }
        public bool IsSecured { get; set; }
        public string RoomMap { get; set; }
        public string RoomHost { get; set; }
        public int RoomLevel { get; set; }
        public ulong RoomID { get; set; }
        public bool IsSelected { get; set; }
        
        public new class UxmlFactory : UxmlFactory<LobbyElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            private UxmlStringAttributeDescription _roomName = new() { name = "room_name" };
            private UxmlStringAttributeDescription _roomMap = new() { name = "room_map" };
            private UxmlStringAttributeDescription _roomHost = new() { name = "room_host" };
            private UxmlLongAttributeDescription _roomID = new() { name = "room_id" };
            private UxmlBoolAttributeDescription _isSecured = new() { name = "is_secured" };
            private UxmlIntAttributeDescription _roomPlayers = new() { name = "room_players", defaultValue = 1};
            private UxmlIntAttributeDescription _roomLevel = new() { name = "room_level" };
            private UxmlBoolAttributeDescription _isSelected = new() { name = "is_selected", defaultValue = false};

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

                lobbyElement.IsSelected = _isSelected.GetValueFromBag(bag, cc);
                lobbyElement.AddToClassList(lobbyElement.IsSelected ? "selected" : "");

                lobbyElement.RoomID = (ulong)_roomID.GetValueFromBag(bag, cc);
            }
        }
    }
}
