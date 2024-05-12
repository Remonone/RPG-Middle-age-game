using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI.Menu {
    public class MenuHandler : MonoBehaviour {
        [SerializeField] private UIDocument _menuDocument;
        [SerializeField] private GameObject _joinLobby;
        [SerializeField] private GameObject _createGame;
        [SerializeField] private GameObject _settings;

        private VisualElement _root;

        private Button _createGameButton;
        private Button _joinGameButton;
        private Button _settingsButton;
        private Button _exitButton;
        
        private void Awake() {
            _root = _menuDocument.rootVisualElement;

            _createGameButton = _root.Q<Button>("Create");
            _createGameButton.clicked += CreateGame;
            _joinGameButton = _root.Q<Button>("Join");
            _joinGameButton.clicked += JoinGame;
            _settingsButton = _root.Q<Button>("Settings");
            _settingsButton.clicked += OnSettings;
            _exitButton = _root.Q<Button>("Exit");
            _exitButton.clicked += OnExit;
        }

        private void OnDestroy() {
            _createGameButton.clicked -= CreateGame;
            _joinGameButton.clicked -= JoinGame;
            _settingsButton.clicked -= OnSettings;
            _exitButton.clicked -= OnExit;
        }
        
        private void OnExit() {
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#else
                Application.Quit();
#endif
        }

        private void OnSettings() {
            _settings.SetActive(true);
            gameObject.SetActive(false);
        }
        
        private void JoinGame() {
            _joinLobby.SetActive(true);
            gameObject.SetActive(false);
        }
        
        private void CreateGame() {
            _createGame.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
