using UnityEngine;

namespace RPG.Dialogs {
    public class AIConversant : MonoBehaviour {
        [SerializeField] private Dialog _dialog;
        [SerializeField] private string _entityName;
        
        public DialogTrigger[] Triggers;

        public string EntityName => _entityName;

        private void Awake() {
            Triggers = GetComponents<DialogTrigger>();
        }

    }
}
