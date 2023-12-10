using UnityEngine;

namespace RPG.Combat.Modifiers.BaseTypes {
    public abstract class Modification : ScriptableObject, IModification {
        [SerializeField] protected string _actionPredicate;
        [SerializeField] private string _modificationName;
        [SerializeField] [TextArea] private string _modificationDescription;
        [SerializeField] protected string _performerComponent;
        [SerializeField] protected string _performToComponent;
        
        protected int Strength;
        
        public string ModificationDescription => _modificationDescription;
        public string ModificationName => _modificationName;
        
        public abstract void RegisterModification(GameObject performer);
        public abstract void UnregisterModification();

        public void SetStrength(int strength) {
            Strength = strength;
        }
    }
}
