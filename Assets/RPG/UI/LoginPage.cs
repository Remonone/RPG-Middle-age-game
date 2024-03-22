using RPG.Network.Controllers;
using RPG.Utils.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI {
    public class LoginPage : MonoBehaviour {
        [SerializeField] private UIDocument _document;
        [SerializeField] private RegisterPage _registerPage;

        private VisualElement _root;

        private TextField _loginField;
        private TextField _passwordField;

        private Button _signUpButton;
        private Button _signInButton;
        private Button _cancelButton;
        private bool _isFailed;
        private bool _isConnected;
        
        private void Start() {
            _root = _document.rootVisualElement;
            _loginField = _root.Q<TextField>("Login");
            _passwordField = _root.Q<TextField>("Password");
            
            _signUpButton = _root.Q<Button>("SignUp");
            _signUpButton.clicked += RegisterAccount;
            
            _signInButton = _root.Q<Button>("SignIn");
            _signInButton.clicked += SendSignInRequest;
            _cancelButton = _root.Q<Button>("Exit");
            _cancelButton.clicked += OnCancel;
        }
        
        private void OnDestroy() {
            _signInButton.clicked -= SendSignInRequest;
            _cancelButton.clicked -= OnCancel;
            _signUpButton.clicked -= RegisterAccount;
        }

        void SendSignInRequest() {
            if (_isConnected) return;
            if (DocumentUtils.CheckOnEmptyValues(_loginField, _passwordField)) return;
            StartCoroutine(AuthenticationController.SignIn(_loginField.value, _passwordField.value));
        }

        void RegisterAccount() {
            _registerPage.gameObject.SetActive(true);
            gameObject.SetActive(false);
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
