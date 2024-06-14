using System;
using UnityEngine.UIElements;

namespace RPG.UI.Elements.PlayerInfo {
    public class PlayerInfo : VisualElement {
        private string _playerName;
        private int _playerLevel;
        private bool _isHostView;
        private bool _isHost;

        private Action<Label, string> _onDataChanged;

        public string PlayerName {
            get => _playerName;
            set {
                _playerName = value;
                _nameLabel.text = value;
            }
        }

        public int PlayerLevel {
            get => _playerLevel;
            set {
                _playerLevel = value;
                _levelLabel.text =  $"{value}";
            }
        }

        public bool IsHost {
            get => _isHost;
            set {
                _isHost = value;
                SetHost(value);
            }
        }

        public bool IsHostView {
            get => _isHostView;
            set {
                _isHostView = value;
                SetHostView(value);
            }
        }

        private void SetHostView(bool value) {
            _mainElement.EnableInClassList("is_host_view", value);
        }

        private void SetHost(bool value) {
            _hostElement.EnableInClassList("disabled", !value);
        }

        private readonly Label _nameLabel = new();
        private readonly Label _levelLabel = new();
        private readonly VisualElement _hostElement = new();
        private readonly VisualElement _mainElement = new();
        private readonly Button _kickButton = new();

        public PlayerInfo() {
            _kickButton.text = "X";
            _kickButton.AddToClassList("kick_button");

            _mainElement.Add(_hostElement);
            _mainElement.Add(_nameLabel);
            _mainElement.Add(_levelLabel);
            _mainElement.Add(_kickButton);
            _mainElement.AddToClassList("player_row");
        }
        
        public new class UxmlFactory : UxmlFactory<WorldRow, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits { }
    }
}