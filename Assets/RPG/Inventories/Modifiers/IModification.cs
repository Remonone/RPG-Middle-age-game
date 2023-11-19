using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Inventories.Modifiers {
    public interface IModification {
        /// <summary>
        ///     Perform any changes whenever called
        ///     Invokes each time when event is invoked. Event is declared inside RegisterModification
        /// </summary>
        /// <param name="holder">Who is holding the item</param>
        /// <param name="prerequisites">Array of strings which used as flags to perform any modification</param>>
        /// <returns>Returns Predicate Strings</returns>
        public void PerformModification(GameObject holder, params string[] prerequisites);
        
    }
}
