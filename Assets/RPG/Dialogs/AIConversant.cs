using RPG.Combat;
using RPG.Core.Cursors;
using RPG.Creatures.Player;
using RPG.Stats.Relations;
using UnityEngine;

namespace RPG.Dialogs {
    public class AIConversant : MonoBehaviour, ITrajectory {
        [SerializeField] private Dialog _dialog;
        [SerializeField] private string _entityName;
        private SelectableTarget _selectable;
        
        public string EntityName => _entityName;

        private void Awake() {
            _selectable = GetComponent<SelectableTarget>();
        }

        public CursorType GetCursorType() {
            return CursorType.EMPTY;
        }
        public bool HandleRaycast(PlayerController invoker) {
            if (_selectable.IsAggressiveTo(invoker.GetComponent<OrganisationWrapper>().Organisation) || ReferenceEquals(_dialog, null)) return false;
            if (invoker.Map["Action"].WasPerformedThisFrame()) {
                invoker.GetComponent<PlayerConversant>().StartDialog(_dialog, this);
            }
            return true;
        }
    }
}
