using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core.Predicate {
    public static class PredicateWorker {
        
        private static Dictionary<string, PredicateMonoBehaviour> _predicateStore = new Dictionary<string, PredicateMonoBehaviour>();

        private static PredicateLexer _lexer = new PredicateLexer();

        // Internal 
        private static List<string> _triggerStore = new List<string>();
        private static Dictionary<string, object> _variableStore = new Dictionary<string, object>();

        public static bool RegisterPredicate(string id, PredicateMonoBehaviour predicate) {
            if (_predicateStore.ContainsKey(id)) return false;
            _predicateStore[id] = predicate;
            return true;
        }

        public static bool DestroyPredicate(string id) {
            return _predicateStore.Remove(id);
        }

        public static void ParsePredicate(string parse) { // PIZDA
            var list = _lexer.LexAnalysis(parse);
            foreach (var item in list) {
                Debug.Log("Command type: " + item.type.name + "; Command regex: " + item.type.regex + "; Command value: " + item.text);
            }
        }
    }
}
