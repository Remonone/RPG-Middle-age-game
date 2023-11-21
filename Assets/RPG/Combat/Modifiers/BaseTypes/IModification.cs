using UnityEngine;

namespace RPG.Combat.Modifiers.BaseTypes {
    public interface IModification {
        public void RegisterModification(GameObject performer);
        public void UnregisterModification();

    }
}
