using System;
using RPG.Dialogs;
using UnityEngine;

namespace RPG.UI.Dialog {
    public class DialogUI : MonoBehaviour {
        private Conversant _selectedConversant;

        private void Awake() {
            _selectedConversant = GameObject.FindWithTag("Player").GetComponent<Conversant>();
        }

        private void Start() {
            _selectedConversant.OnUpdate += UpdateUI;
            UpdateUI();
        }
        private void UpdateUI() {
            gameObject.SetActive(_selectedConversant.IsActive);
            if (!_selectedConversant.IsActive) return;
        }
    }
}
