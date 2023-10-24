using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

namespace RPG.Core.Predicate {
    public class Parser {

        private int _position;
        private PredicateToken[] _tokens;
        private Dictionary<string, object> _scope;

        [CanBeNull]
        private PredicateToken Match(params TokenType[] expected) {
            if (_position > _tokens.Length) return null;
            var currentToken = _tokens[_position];
            if (expected.All(type => type.name != currentToken.type.name)) return null;
            _position++;
            return currentToken;

        }
        
        private PredicateToken Require(params TokenType[] expected) {
            var token = Match(expected);
            if (token == null) {
                Debug.Log($"Type mismatch. In {_position} position expected {expected[0]}");
                return null;
            }
            return token;
        }
    }
}
