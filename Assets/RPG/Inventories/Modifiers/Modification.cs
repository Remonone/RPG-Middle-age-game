using UnityEngine;

namespace RPG.Inventories.Modifiers {
    public abstract class Modification : ScriptableObject, IModification {

        [SerializeField] protected string _actionPredicate;
        public abstract void RegisterModification(GameObject performer);
        public abstract void UnregisterModification();
    }
}
