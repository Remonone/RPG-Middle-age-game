using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting.Antlr3.Runtime;

namespace RPG.Core.Predicate {
    public static class PredicateLexicon {
        public static Dictionary<string, TokenType> TokenTypes = new Dictionary<string, TokenType> {
            {"VALUE", new TokenType("VALUE", @"[a-zA-Z0-9_.-]*")},
            {"IDENTIFICATOR", new TokenType("IDENTIFICATOR", @"#")},
            {"STEP", new TokenType("STEP", @":")},
            {"PRE_CONDITION", new TokenType("PRE_CONDITION", @"[c_]")},
            {"POST_CONDITION", new TokenType("POST_CONDITION", @"[_c]")},
            {"VARIABLE", new TokenType("VARIABLE", @"&")},
            {"PLUS", new TokenType("PLUS", @"+")},
            {"MINUS", new TokenType("MINUS", @"-")},
            {"MULTIPLY", new TokenType("MULTIPLY", @"\*")},
            {"DIVIDE", new TokenType("DIVIDE", @"\/")},
            {"TRIGGER", new TokenType("TRIGGER", @"%")},
            {"EMPTY", new TokenType("EMPTY", @"@")},
            {"END", new TokenType("END", @".")},
            {"NEXT", new TokenType("NEXT", @";")},
            {"CONDITION_START", new TokenType("CONDITION_START", @"<")},
            {"CONDITION_END", new TokenType("CONDITION_END", @">")}
        };
    }
}
