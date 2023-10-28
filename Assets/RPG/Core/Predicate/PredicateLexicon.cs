using System.Collections;
using System.Collections.Generic;

namespace RPG.Core.Predicate {
    
    public static class PredicateLexicon {
        public static readonly Dictionary<string, TokenType> TokenTypes = new Dictionary<string, TokenType> {
            {"VALUE", new TokenType("VALUE", @"[A-Za-z0-9._]*")}, // (?=.*[a-zA-Z_])[\w\d_]+)"
            // {"NUMBER", new TokenType("NUMBER", @"[-+]?(\d+\.\d+|\.\d+|\d+)")},
            {"IDENTIFIER", new TokenType("IDENTIFIER", @"#")},
            {"STEP", new TokenType("STEP", @":")},
            {"PRE_CONDITION", new TokenType("PRE_CONDITION", @"[c_]")},
            {"POST_CONDITION", new TokenType("POST_CONDITION", @"[_c]")},
            {"VARIABLE", new TokenType("VARIABLE", "VAR")},
            {"REFERENCE", new TokenType("REFERENCE", @"&")},
            {"PLUS", new TokenType("PLUS", @"+")},
            {"MINUS", new TokenType("MINUS", @"-")},
            {"MULTIPLY", new TokenType("MULTIPLY", @"\*")},
            {"DIVIDE", new TokenType("DIVIDE", @"\/")},
            {"EMPTY", new TokenType("EMPTY", @"@")},
            {"END", new TokenType("END", @"\;")},
            {"NEXT", new TokenType("NEXT", @"\,")},
            {"CONDITION_START", new TokenType("CONDITION_START", @"<")},
            {"CONDITION_END", new TokenType("CONDITION_END", @">")},
            {"LPAR", new TokenType("LPAR", @"\(")},
            {"RPAR", new TokenType("RPAR", @"\)")},
            {"DELETE", new TokenType("DELETE", @"DEL")}
        };
        
    }
}
