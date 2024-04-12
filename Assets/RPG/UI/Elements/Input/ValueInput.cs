using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RPG.UI.Elements.Input {
    public class ValueInput : VisualElement {

        public Action<string> OnErrorMessageSet;
        
        public string Label { get; set; }
        public string Value { get; set; }

        private string _error;
        public string Error {
            get => _error;
            set {
                _error = value;
                OnErrorMessageSet?.Invoke(value);
            }
        }
        
        public bool IsPassword { get; set; }

        public new class UxmlFactory : UxmlFactory<ValueInput, UxmlTraits> { }
 
        public new class UxmlTraits : VisualElement.UxmlTraits {
            UxmlStringAttributeDescription _label = new (){ name = "label", defaultValue = "Input field" };
            UxmlStringAttributeDescription _value = new (){ name = "value" };
            UxmlStringAttributeDescription _error = new (){ name = "error" };
            UxmlBoolAttributeDescription _isPassword = new() { name = "password", defaultValue = false };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get { yield break; }
            }
     
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var valueInput = ve as ValueInput;
     
                valueInput.Clear();

                valueInput.AddToClassList("value_input");
     
                valueInput.Label = _label.GetValueFromBag(bag, cc);
                valueInput.Add(new Label(valueInput.Label));

                valueInput.Value = _value.GetValueFromBag(bag, cc);
                valueInput.Add(new TextField{value = valueInput.Value});
                
                valueInput.Error = _error.GetValueFromBag(bag, cc);
                valueInput.Add(new Label {
                    text = valueInput.Error,
                    name = "error"
                });

                valueInput.IsPassword = _isPassword.GetValueFromBag(bag, cc);
                valueInput.Q<TextField>().isPasswordField = valueInput.IsPassword;
                valueInput.OnErrorMessageSet += str => {
                        valueInput.EnableInClassList("input_error", !string.IsNullOrEmpty(str));
                };
            }

        }
    }
}
