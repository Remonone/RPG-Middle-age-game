using Newtonsoft.Json.Linq;
using RPG.Lobby;
using RPG.Network.Controllers;
using RPG.Network.Management;
using RPG.UI.Elements.Input;
using RPG.UI.Elements.Snackbar;
using RPG.Utils.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RPG.UI {
    public class LoginPage : MonoBehaviour {
        [SerializeField] private UIDocument _document;
        [SerializeField] private RegisterPage _registerPage;

        private VisualElement _root;

        private ValueInput _loginField;
        private ValueInput _passwordField;

        private Button _signUpButton;
        private Button _signInButton;
        private Button _cancelButton;

        private SnackbarElement _snackbar;
        
        private bool _isFailed;
        private bool _isConnected;

        private void OnEnable() {
            _root = _document.rootVisualElement;
            _loginField = _root.Q<ValueInput>("Login");
            _passwordField = _root.Q<ValueInput>("Password");
            _signUpButton = _root.Q<Button>("SignUp");
            _signInButton = _root.Q<Button>("SignIn");
            _cancelButton = _root.Q<Button>("Exit");
            _snackbar = _root.Q<SnackbarElement>();
            _signInButton.clicked += SendSignInRequest;
            _cancelButton.clicked += OnCancel;
            _signUpButton.clicked += RegisterAccount;
        }

        private void OnDisable() {
            _signInButton.clicked -= SendSignInRequest;
            _cancelButton.clicked -= OnCancel;
            _signUpButton.clicked -= RegisterAccount;
        }


        void SendSignInRequest() {
            if (_isConnected) return;
            if (DocumentUtils.CheckOnEmptyValues(_loginField, _passwordField)) return;
            StartCoroutine(AuthenticationController.SignIn(_loginField.Value, _passwordField.Value, OnLogin, OnFail));
        }
        private void OnFail(JToken obj) {
            if ((string)obj["type"] == "internal") {
                OnServerIssue();
                return;
            }

            _root.Q<ValueInput>((string)obj["type"]).Error = (string)obj["error_message"];
        }
        
        private void OnServerIssue() {
            _snackbar.Position = SnackbarPosition.BottomLeft;
            _snackbar.Type = SnackbarType.Error;
            _snackbar.Text = "Some error occured on server side. Try later...";
            _snackbar.ShowSnackbar(5000);
        }

        private void OnLogin(JToken data) {
            var application = FindObjectOfType<ApplicationManager>();
            application.Token = (string)data["token"];
            application.PlayerData = new PlayerData((string)data["user"]["_id"], (string)data["user"]["username"]);
            application.IP = (string)data["ip"];
            application.Port = (ushort)data["port"];
            Debug.Log(application.IP + ":" + application.Port);
            SceneManager.LoadScene("Main");
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
