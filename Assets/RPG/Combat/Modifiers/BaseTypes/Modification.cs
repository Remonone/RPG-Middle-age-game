using UnityEngine;

namespace RPG.Combat.Modifiers.BaseTypes {
    public abstract class Modification : ScriptableObject, IModification {

        [SerializeField] protected string _actionPredicate;
        [SerializeField] protected string _performerComponent;
        [SerializeField] protected string _performToComponent;
        
        public abstract void RegisterModification(GameObject performer);
        public abstract void UnregisterModification();
    }
}
