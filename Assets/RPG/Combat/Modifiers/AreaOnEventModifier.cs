using System;
using RPG.Combat.Modifiers.BaseTypes;
using RPG.Core.Predicate;
using UnityEngine;

namespace RPG.Combat.Modifiers {
    public class AreaOnEventModifier : AreaModifier {

        protected override void OnRegisterModification(Func<GameObject, PredicateMonoBehaviour[]> func) {
            throw new NotImplementedException();
        }
        protected override void OnUnregisterModification() {
            throw new NotImplementedException();
        }
    }
}
