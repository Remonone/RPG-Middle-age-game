using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Inventories.Modifiers {
    public interface IModification {
        public void RegisterModification(GameObject performer);
        public void UnregisterModification();

    }
}
