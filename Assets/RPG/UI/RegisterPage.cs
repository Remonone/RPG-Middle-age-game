
using Newtonsoft.Json.Linq;
using RPG.Network.Client;
using RPG.Network.Controllers;
using RPG.UI.Elements.Input;
using RPG.Utils.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace RPG.UI {
    public class RegisterPage : MonoBehaviour {
        [SerializeField] private UIDocument _document;
        [SerializeField] private LoginPage _loginPage;

        private VisualElement _root;

        private ValueInput _loginField;
        private ValueInput _usernameField;
        private ValueInput _passwordField;
        private ValueInput _confirmPasswordField;
        private DropdownField _dropdown;

        private Button _signUpButton;
        private Button _cancelButton;
        private bool _isFailed;
        private bool _isConnected;
        
        private void Awake() {
            _root = _document.rootVisualElement;
            _dropdown = _root.Q<DropdownField>();
            InitDropdown();
            _loginField = _root.Q<ValueInput>("Login");
            _usernameField = _root.Q<ValueInput>("Username");
            _passwordField = _root.Q<ValueInput>("Password");
            _confirmPasswordField = _root.Q<ValueInput>("ConfirmPassword");
            _signUpButton = _root.Q<Button>("Register");
            _cancelButton = _root.Q<Button>("Cancel");
        }

        private void InitDropdown() {
            _dropdown.choices.Add("Sample server");
            _dropdown.value = "Sample server";
        }

        private void OnEnable() {
            _signUpButton.clicked += SendSignUpRequest;
            _cancelButton.clicked += Cancel;
        }

        private void OnDestroy() {
            _signUpButton.clicked -= SendSignUpRequest;
            _cancelButton.clicked -= Cancel;
        }

        void SendSignUpRequest() {
            if (_isConnected) return;
            if (DocumentUtils.CheckOnEmptyValues(_loginField, _usernameField, _passwordField, _confirmPasswordField)) return;
            if (_passwordField.Value != _confirmPasswordField.Value) return;
            StartCoroutine(AuthenticationController.SignUp(_loginField.Value, _usernameField.Value, _passwordField.Value, _dropdown.value, OnRegister));
        }

        private void OnRegister(JToken data) {
            ClientSingleton.Instance.Manager.SetData((string)data["token"]);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(ipv4Address: (string)data["ip"], port: (ushort)data["port"]);
            SceneManager.LoadScene("Main Screen");
        }

        void Cancel() {
            _loginPage.gameObject.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
