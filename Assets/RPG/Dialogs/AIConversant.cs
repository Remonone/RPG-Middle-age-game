using RPG.Creatures.Controls;
using RPG.UI.Cursors;
using UnityEngine;

namespace RPG.Dialogs {
    public class AIConversant : MonoBehaviour, ITrajectory {
        [SerializeField] private Dialog _dialog;
        [SerializeField] private string _entityName;
        
        public DialogTrigger[] Triggers;

        public string EntityName => _entityName;

        private void Awake() {
            Triggers = GetComponents<DialogTrigger>();
        }

        public CursorType GetCursorType() {
            return CursorType.EMPTY;
        }
        public bool HandleRaycast(PlayerController invoker) {
            if (_dialog == null) {
                return false;
            }
            if (invoker.Map["Action"].WasPerformedThisFrame()) {
                invoker.GetComponent<PlayerConversant>().StartDialog(_dialog, this);
            }
            return true;
        }
    }
}
