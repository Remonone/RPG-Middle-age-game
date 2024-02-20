using UnityEngine;

namespace RPG.Core.Predicate.Interfaces {
    public interface IPredicateHandler {
        public object Predicate(string command, object[] arguments);
    }
}
