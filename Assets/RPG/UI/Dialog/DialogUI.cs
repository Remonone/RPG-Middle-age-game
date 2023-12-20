using RPG.Dialogs;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Dialog {
    public class DialogUI : MonoBehaviour {
        
        private VisualElement _root;
        private PlayerConversant _playerConversant;
        private Label _currentConversant;
        private Label _dialogText;
        private Box _playerResponses;

        private void Awake() {
            _playerConversant = GameObject.FindWithTag("Player").GetComponent<PlayerConversant>();
            _root = GetComponent<UIDocument>().rootVisualElement;
        }

        private void Start() {
            _currentConversant = _root.Q<Label>("conversant");
            _dialogText = _root.Q<Label>("dialog");
            _playerResponses = _root.Q<Box>("choices");
            _playerConversant.OnUpdate += UpdateUI;
            UpdateUI();
        }
        
        private void UpdateUI() {
            gameObject.SetActive(_playerConversant.IsActive);
            if (!_playerConversant.IsActive) return;
            _currentConversant.text = _playerConversant.CurrentConversantName;
            if (_playerConversant.IsChoosing) {
                BuildChooseList();
            }
            else {
                _dialogText.text = _playerConversant.GetCurrentText();
                if (!_playerConversant.HasNext()) return;
                var button = new Button(() => { _playerConversant.Next();}) {
                    text = "Next"
                };
                _playerResponses.Add(button);
            }
        }
        private void BuildChooseList() {
            _playerResponses.Clear();
            foreach (var choice in _playerConversant.GetChoices()) {
                var button = new Button(() => { _playerConversant.SelectChoice(choice);}) {
                    text = choice.Text
                };
                _playerResponses.Add(button);
            }
        }
    }
}
