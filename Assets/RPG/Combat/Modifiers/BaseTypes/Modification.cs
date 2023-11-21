using System;
using UnityEngine;

namespace RPG.Combat.Modifiers.BaseTypes {
    public abstract class Modification : ScriptableObject, IModification {

        [SerializeField] protected string _actionPredicate;
        public abstract void RegisterModification(GameObject performer);
        public abstract void UnregisterModification();
    }
}
