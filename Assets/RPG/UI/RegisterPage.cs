using RPG.Network.Controllers;
using RPG.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI {
    public class RegisterPage : MonoBehaviour {
        [SerializeField] private UIDocument _document;
        [SerializeField] private LoginPage _loginPage;

        private VisualElement _root;

        private TextField _loginField;
        private TextField _usernameField;
        private TextField _passwordField;
        private TextField _confirmPasswordField;

        private Button _signUpButton;
        private Button _cancelButton;
        private bool _isFailed;
        private bool _isConnected;
        
        private void Start() {
            _root = _document.rootVisualElement;
            _loginField = _root.Q<TextField>("Login");
            _usernameField = _root.Q<TextField>("Username");
            _passwordField = _root.Q<TextField>("Password");
            _confirmPasswordField = _root.Q<TextField>("ConfirmPassword");
            
            _signUpButton = _root.Q<Button>("Register");
            _signUpButton.clicked += SendSignUpRequest;
            _cancelButton = _root.Q<Button>("Cancel");
            _cancelButton.clicked += Cancel;
        }

        private void OnDestroy() {
            _signUpButton.clicked -= SendSignUpRequest;
            _cancelButton.clicked -= Cancel;
        }

        void SendSignUpRequest() {
            if (_isConnected) return;
            if (DocumentUtils.CheckOnEmptyValues(_loginField, _usernameField, _passwordField, _confirmPasswordField)) return;
            if (_passwordField.value != _confirmPasswordField.value) return;
            StartCoroutine(AuthenticationController.SignUp(_loginField.value, _usernameField.value, _passwordField.value));
        }

        

        void Cancel() {
            _loginPage.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
