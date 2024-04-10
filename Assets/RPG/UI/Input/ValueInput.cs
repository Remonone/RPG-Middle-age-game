using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace RPG.UI.Input {
    public class ValueInput : VisualElement {

        public Action<string> OnErrorMessageSet;
        
        public string label { get; set; }
        public string value { get; set; }

        private string _error;
        public string error {
            get => _error;
            set {
                _error = value;
                OnErrorMessageSet?.Invoke(value);
            }
        }

        public new class UxmlFactory : UxmlFactory<ValueInput, UxmlTraits> { }
 
        public new class UxmlTraits : VisualElement.UxmlTraits {
            UxmlStringAttributeDescription _label = new (){ name = "label", defaultValue = "Input field" };
            UxmlStringAttributeDescription _value = new (){ name = "value" };
            UxmlStringAttributeDescription _error = new (){ name = "error" };
            
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
     
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var valueInput = ve as ValueInput;
     
                valueInput.Clear();

                valueInput.AddToClassList("value_input");
     
                valueInput.label = _label.GetValueFromBag(bag, cc);
                valueInput.Add(new Label(valueInput.label));

                valueInput.value = _value.GetValueFromBag(bag, cc);
                valueInput.Add(new TextField{value = valueInput.value});
                
                valueInput.error = _error.GetValueFromBag(bag, cc);
                valueInput.Add(new Label {
                    text = valueInput.error,
                    name = "error"
                });
                valueInput.OnErrorMessageSet += str => {
                        valueInput.EnableInClassList("input_error", !string.IsNullOrEmpty(str));
                };
            }

        }
    }
}
