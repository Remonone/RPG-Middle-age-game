using UnityEngine;

namespace RPG.Combat.Modifiers.BaseTypes {
    public abstract class Modification : ScriptableObject, IModification {
        [Tooltip(ACTION_DESCRIPTION)]
        [SerializeField] protected string _actionPredicate;
        [SerializeField] private string _modifierDescription;
        [SerializeField] protected string _performerComponent;
        [SerializeField] protected string _performToComponent;

        public const string ACTION_DESCRIPTION = "";
        public string ModifierDescription => _modifierDescription;
        
        public abstract void RegisterModification(GameObject performer);
        public abstract void UnregisterModification();
    }
}
