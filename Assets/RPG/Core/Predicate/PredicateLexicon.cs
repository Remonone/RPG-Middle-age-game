using System.Collections;
using System.Collections.Generic;

namespace RPG.Core.Predicate {
    
    public static class PredicateLexicon {
        public static readonly Dictionary<string, TokenType> TokenTypes = new (){
            {"VARIABLE", new TokenType("VARIABLE", @"VAR")},
            {"DELETE", new TokenType("DELETE", @"DEL")},
            {"VALUE", new TokenType("VALUE", @"[A-Za-z0-9._]*")},
            {"IDENTIFIER", new TokenType("IDENTIFIER", @"#")},
            {"STEP", new TokenType("STEP", @":")},
            {"REFERENCE", new TokenType("REFERENCE", @"&")},
            {"PLUS", new TokenType("PLUS", @"+")},
            {"MINUS", new TokenType("MINUS", @"-")},
            {"MULTIPLY", new TokenType("MULTIPLY", @"\*")},
            {"DIVIDE", new TokenType("DIVIDE", @"\/")},
            {"EMPTY", new TokenType("EMPTY", @"@")},
            {"END", new TokenType("END", @"\;")},
            {"NEXT", new TokenType("NEXT", @"\,")},
            // {"CONDITION_START", new TokenType("CONDITION_START", @"<")},
            // {"CONDITION_END", new TokenType("CONDITION_END", @">")},
            {"LPAR", new TokenType("LPAR", @"\(")},
            {"RPAR", new TokenType("RPAR", @"\)")},
            
        };
        
    }
}
