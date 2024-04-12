using Newtonsoft.Json.Linq;
using RPG.Network.Client;
using RPG.Network.Controllers;
using RPG.UI.Elements.Snackbar;
using RPG.Utils.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        
        private void Awake() {
            _root = _document.rootVisualElement;
            _loginField = _root.Q<TextField>("Login");
            _passwordField = _root.Q<TextField>("Password");
            _signUpButton = _root.Q<Button>("SignUp");
            _signInButton = _root.Q<Button>("SignIn");
            _cancelButton = _root.Q<Button>("Exit");
        }

        private void OnEnable() {
            _signInButton.clicked += SendSignInRequest;
            _cancelButton.clicked += OnCancel;
            _signUpButton.clicked += RegisterAccount;
        }

        private void OnDisable() {
            _signInButton.clicked -= SendSignInRequest;
            _cancelButton.clicked -= OnCancel;
            _signUpButton.clicked -= RegisterAccount;
        }

        private void Start() {
            _root.Q<VisualElement>().Add(new SnackbarElement {
                Text = "This is an error snackbar",
                Type = SnackbarType.Error,
                Position = SnackbarPosition.TopLeft,
                TimeToClose = 10000f
            });
        }

        void SendSignInRequest() {
            if (_isConnected) return;
            if (DocumentUtils.CheckOnEmptyValues(_loginField, _passwordField)) return;
            StartCoroutine(AuthenticationController.SignIn(_loginField.value, _passwordField.value, OnLogin));
        }

        private void OnLogin(JToken data) {
            ClientSingleton.Instance.Manager.SetData((string)data["token"]);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address: (string)data["ip"], port: (ushort)data["port"]);
            SceneManager.LoadScene("Main Screen");
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
