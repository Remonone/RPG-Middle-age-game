using System.Collections.Generic;
using System.Timers;
using UnityEngine.UIElements;

namespace RPG.UI.Elements.Snackbar {
    
    public class SnackbarElement : VisualElement {
        public string Text { get; set; }
        public SnackbarType Type { get; set; }
        public SnackbarPosition Position { get; set; }
        
        private Timer _timer;

        public Timer Timer {
            get => _timer;
            private set => _timer = value;
        }

        public void ShowSnackbar(float time) {
            ClearClassList();
            AddToClassList("snackbar");
            AddToClassList(Position.ToString());
            AddToClassList(Type.ToString());
            _timer.Interval = time;
            _timer.Start();
        }

        public new class UxmlFactory : UxmlFactory<SnackbarElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            private UxmlStringAttributeDescription _text = new() { name = "value", defaultValue = "" };
            private UxmlEnumAttributeDescription<SnackbarType> _type = new() { name = "type", defaultValue = SnackbarType.Info };
            private UxmlEnumAttributeDescription<SnackbarPosition> _position = new() { name = "position", defaultValue = SnackbarPosition.BottomRight };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var snackbar = ve as SnackbarElement;
                snackbar.Clear();

                snackbar.AddToClassList("snackbar");
                
                snackbar.Text = _text.GetValueFromBag(bag, cc);
                snackbar.Add(new Label(snackbar.Text));

                snackbar.Type = _type.GetValueFromBag(bag, cc);
                snackbar.AddToClassList(snackbar.Type.ToString());

                snackbar.Position = _position.GetValueFromBag(bag, cc);
                snackbar.AddToClassList(snackbar.Position.ToString());

                snackbar.Timer = new Timer();
                snackbar.Timer.AutoReset = false;
                
                snackbar.AddToClassList("Hidden");
                
                snackbar.Timer.Elapsed += (_, _) => {
                    snackbar.AddToClassList("Hidden");
                };
                
            }
        }
        
    }

    public enum SnackbarType {
        Success,
        Error,
        Info,
        Debug
    }

    public enum SnackbarPosition {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight
    }
}
