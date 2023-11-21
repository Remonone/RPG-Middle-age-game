using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace RPG.Core.Predicate {
    public class PredicateLexer {
        private string _code;
        private int _position;
        private readonly List<PredicateToken> _predicateTokens = new();
        
        private const int MAX_ITER = 100;

        public List<PredicateToken> LexAnalysis(string code) {
            _code = code;
            _position = 0;
            var iteration = 0;
            while (NextToken() && iteration < MAX_ITER) { 
                iteration++;
            }
            if (_position != _code.Length) 
                throw new Exception($"Error has appeared in symbol {_position}.");
                
            return _predicateTokens;
        }

        public bool NextToken() {
            if (_position > _code.Length) return false;
            foreach (var type in PredicateLexicon.TokenTypes.Values) {
                var cutCode = _code.Substring(_position);
                var match = Regex.Match(cutCode, '^' + type.regex);
                if (!match.Success || match.Value.Length < 1) continue;
                var token = new PredicateToken(type, match.Value, _position); 
                _position += match.Value.Length;
                _predicateTokens.Add(token);
                return true;
            }

            return false;
        }
    }
}
