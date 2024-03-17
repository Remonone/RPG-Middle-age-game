using RPG.Network.Client;
using RPG.Utils;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RPG.UI {
    public class LoginPage : MonoBehaviour {
        [SerializeField] private UIDocument _document;

        private VisualElement _root;

        private TextField _loginField;
        private TextField _passwordField;

        private Button _signInButton;
        private Button _cancelButton;
        
        private void Start() {
            _root = _document.rootVisualElement;
            _loginField = _root.Q<TextField>("Login");
            _passwordField = _root.Q<TextField>("Password");

            _signInButton = _root.Q<Button>("SignIn");
            _signInButton.clicked += OnSignIn;
            _cancelButton = _root.Q<Button>("Exit");
            _cancelButton.clicked += OnCancel;
        }
        
        private async void OnSignIn() {
            var login = _loginField.value;
            var password = _passwordField.value;
            // Create logic to send to fetch from backend side a user
            // if (result != AuthState.Authenticated) {
            //     return;
            // }

            SceneManager.LoadScene("Main Screen");
        }
        private void OnCancel() {
            
        }
        
        
    }
}
