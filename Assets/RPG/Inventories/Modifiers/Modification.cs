using RPG.Inventories.Items;
using UnityEngine;

namespace RPG.Inventories.Modifiers {
    [CreateAssetMenu(fileName = "FILENAME", menuName = "MENUNAME", order = 0)]
    public abstract class Modification : ScriptableObject, IModification {

        [SerializeField] protected string _actionPredicate;
        
        public abstract void PerformModification(GameObject holder, params string[] prerequisites);
        
    }
}
