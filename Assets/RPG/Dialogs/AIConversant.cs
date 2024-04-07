using RPG.Combat;
using RPG.Creatures.Player;
using RPG.UI.Cursors;
using UnityEngine;

namespace RPG.Dialogs {
    public class AIConversant : MonoBehaviour, ITrajectory {
        [SerializeField] private Dialog _dialog;
        [SerializeField] private string _entityName;
        [SerializeField] private SelectableTarget _selectable;
        
        public DialogTrigger[] Triggers;

        public string EntityName => _entityName;

        private void Awake() {
            Triggers = GetComponents<DialogTrigger>();
            _selectable = GetComponent<SelectableTarget>();
        }

        public CursorType GetCursorType() {
            return CursorType.EMPTY;
        }
        public bool HandleRaycast(PlayerController invoker) {
            if (_selectable.IsAggressiveTo(invoker.GetOrganisation()) || ReferenceEquals(_dialog, null)) return false;
            if (invoker.Map["Action"].WasPerformedThisFrame()) {
                invoker.GetComponent<PlayerConversant>().StartDialog(_dialog, this);
            }
            return true;
        }
    }
}
