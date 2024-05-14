using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RPG.UI.Elements.LobbyElement.LobbyConnect {
    public class LobbyConnect : VisualElement{
        public string RoomID { get; set; }
        public string Password { get; set; }
        public Action<string, string> OnConnect;
        
        public new class UxmlFactory : UxmlFactory<LobbyConnect, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits {
            private UxmlStringAttributeDescription _roomId = new() { name = "room_id" };
            private UxmlStringAttributeDescription _roomPassword = new() { name = "room_password" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get {yield break;}
            }
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var connectElement = ve as LobbyConnect;
                connectElement.Clear();
                
                connectElement.AddToClassList("lobby_connect");
                connectElement.RoomID = _roomId.GetValueFromBag(bag, cc);
                connectElement.Password = _roomPassword.GetValueFromBag(bag, cc);
                var input = new TextField { value = connectElement.Password };
                connectElement.Add(input);
                input.RegisterValueChangedCallback(evt => {
                    if (evt.target is Label) return;

                    connectElement.Password = evt.newValue;
                });
                var el = new VisualElement();
                el.AddToClassList("lobby_connect_control");
                var connectButton = new Button {
                    text = "Connect"
                };
                connectButton.clicked += () => {
                    connectElement.OnConnect?.Invoke(connectElement.RoomID, connectElement.Password);
                };
                el.Add(connectButton);
                var cancelButton = new Button {
                    text = "Cancel"
                };
                cancelButton.clicked += () => {
                    cancelButton.RemoveFromHierarchy();
                };
                el.Add(cancelButton);
                connectElement.Add(el);
            }

        }
    }
}
