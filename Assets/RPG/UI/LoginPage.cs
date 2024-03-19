using RPG.Network.Controllers;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI {
    public class LoginPage : MonoBehaviour {
        [SerializeField] private UIDocument _document;

        private VisualElement _root;

        private TextField _loginField;
        private TextField _passwordField;

        private Button _signInButton;
        private Button _cancelButton;
        private bool _isFailed;
        private bool _isConnected;
        
        private void Start() {
            _root = _document.rootVisualElement;
            _loginField = _root.Q<TextField>("Login");
            _passwordField = _root.Q<TextField>("Password");

            _signInButton = _root.Q<Button>("SignIn");
            _signInButton.clicked += SendSignInRequest;
            _cancelButton = _root.Q<Button>("Exit");
            _cancelButton.clicked += OnCancel;
        }

        void SendSignInRequest() {
            if (_isConnected) return;
            StartCoroutine(AuthenticationController.SignIn(_loginField.value, _passwordField.value));
        }
        private void OnCancel() {
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.ExitPlaymode();
            #else
                Application.Quit();
            #endif
        }
        
        
    }
}
