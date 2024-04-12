using System.Collections.Generic;
using System.Timers;
using UnityEngine.UIElements;

namespace RPG.UI.Elements.Snackbar {
    
    public class SnackbarElement : VisualElement {
        public string Text { get; set; }
        public SnackbarType Type { get; set; }
        public SnackbarPosition Position { get; set; }
        public float TimeToClose { get; set; }
        
        public new class UxmlFactory : UxmlFactory<SnackbarElement, UxmlTraits> {}

        public new class UxmlTraits : VisualElement.UxmlTraits {
            private UxmlStringAttributeDescription _text = new() { name = "value", defaultValue = "" };
            private UxmlEnumAttributeDescription<SnackbarType> _type = new() { name = "type", defaultValue = SnackbarType.Info };
            private UxmlEnumAttributeDescription<SnackbarPosition> _position = new() { name = "position", defaultValue = SnackbarPosition.BottomRight };
            private UxmlFloatAttributeDescription _timeToClose = new() { name = "time_to_close", defaultValue = 10000F };
            
            private Timer _timer;
            
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

                snackbar.TimeToClose = _timeToClose.GetValueFromBag(bag, cc);
                _timer = new Timer();
                _timer.Interval = snackbar.TimeToClose;
                _timer.Elapsed += (_, _) => {
                    snackbar.RemoveFromHierarchy();
                };

                _timer.AutoReset = false;
                _timer.Enabled = true;
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
