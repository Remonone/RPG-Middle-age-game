using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Elements {
    public class WorldRow : VisualElement {


        private string _map;
        private int _level;
        private string _name;
        
        public string Map {
            get => _map;
            set {
                _map = value;
                _onDataChanged.Invoke(_mapLabel, value);
            }
        }
        public int Level {
            get => _level;
            set {
                _level = value;
                _onDataChanged.Invoke(_levelLabel, $"{value}");
            }
        }

        public string Name {
            get => _name;
            set {
                _name = value;
                _onDataChanged.Invoke(_nameLabel, value);
            }
        }

        private Action<Label, string> _onDataChanged;

        private readonly Label _nameLabel = new();
        private readonly Label _mapLabel = new();
        private readonly Label _levelLabel = new();

        public WorldRow() {
            var nameLabel = new Label();
            
            Add(_nameLabel);
            Add(_mapLabel);
            Add(_levelLabel);
            _onDataChanged += ChangeData;
        }
        ~WorldRow() {
            _onDataChanged -= ChangeData;
        }
        
        private void ChangeData(Label arg1, string arg2) {
            arg1.text = arg2;
        }

        public new class UxmlFactory : UxmlFactory<WorldRow, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits {
            UxmlIntAttributeDescription _level = new() { name = "level" };
            UxmlStringAttributeDescription _map = new() { name = "map" };
            UxmlStringAttributeDescription _name = new() { name = "world_name" };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription {
                get { yield break; }
            }

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc) {
                base.Init(ve, bag, cc);
                var row = ve as WorldRow;
                Debug.Log("init");
                row.Clear();

                row.Name = _name.GetValueFromBag(bag, cc);
                row.Add(new Label(row.Name));

                row.Map = _map.GetValueFromBag(bag, cc);
                row.Add(new Label(row.Map));

                row.Level = _level.GetValueFromBag(bag, cc);
                row.Add(new Label($"{row.Level}"));
            }
        }
    }
}
