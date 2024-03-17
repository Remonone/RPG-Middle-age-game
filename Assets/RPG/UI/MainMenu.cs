using System;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UIElements;

namespace RPG.UI {
    public class MainMenu : MonoBehaviour {
        [SerializeField] private UIDocument _document;

        private VisualElement _root;

        private Button _button;

        private void Awake() {
            _root = _document.rootVisualElement;
        }

        private void OnEnable() {
            _button = _root.Q<Button>();
            _button.clicked += OnButtonClick;
        }

        private void OnDestroy() {
            _button.clicked -= OnButtonClick;
        }
        
        private void OnButtonClick() {
            try {
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetConnectionData(NetworkEndPoint.AnyIpv4); // change to ip
            }
            catch (Exception e) {
                Label label = new Label(e.Message);
                _root.Add(label);
            }
        }
    }
}
