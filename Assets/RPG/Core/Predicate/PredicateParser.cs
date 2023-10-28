using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using RPG.Core.Predicate.Nodes;

namespace RPG.Core.Predicate {
    public class PredicateParser {

        private int _position;
        private List<PredicateToken> _tokens;
        private Dictionary<string, decimal> _scope;

        [CanBeNull]
        private PredicateToken Match(params TokenType[] expected) {
            if (_position > _tokens.Count) return null;
            var currentToken = _tokens[_position];
            if (expected.All(type => type.name != currentToken.type.name)) return null;
            _position++;
            return currentToken;

        }
        
        private PredicateToken Require(params TokenType[] expected) {
            var token = Match(expected);
            if (token == null) throw new Exception($"Type mismatch. In {_position} position expected {expected[0]}. Received: {_tokens[_position]}");
            return token;
        }

        public ExpressionNode ParseCode(List<PredicateToken> tokens) {
            _tokens = tokens;
            _position = 0;
            var root = new StatementNode();
            while (_position < _tokens.Count - 1) {
                var nodeString = ParseExpression();
                Require(PredicateLexicon.TokenTypes["END"]);
                root.AddNode(nodeString);
            }
            _tokens.Clear();
            return root;
        }
        
        private ExpressionNode ParseExpression() {
            if (Match(PredicateLexicon.TokenTypes["IDENTIFIER"]) != null) {
                var identifierNode = ParseIdentifier();
                var functionNode = ParseFunctionInvocation();
                return new SenderNode { Receiver = identifierNode, Action = functionNode};
            }
            if (Match(PredicateLexicon.TokenTypes["DELETE"]) != null) {
                CheckOnStep();
                var deletingNode = Require(PredicateLexicon.TokenTypes["VALUE"]);
                return new DeleteNode{ToDelete = deletingNode};
            }

            if (Match(PredicateLexicon.TokenTypes["VARIABLE"]) == null)
                throw new Exception("Expected operating with component or variable operator");
            CheckOnStep();
            var variableName = Match(PredicateLexicon.TokenTypes["VALUE"]);
            if (variableName == null) throw new Exception("Variable name is empty/incorrect");
            CheckOnStep();
            var value = ParseFormula();
            return new AssignNode { Name = variableName, Value = value };
        }

        private void CheckOnStep() {
            if (Match(PredicateLexicon.TokenTypes["STEP"]) == null)
                throw new Exception("Empty invocation");
        }
        
        private FunctionNode ParseFunctionInvocation() {
            var functionName = Match(PredicateLexicon.TokenTypes["VALUE"]);
            if (functionName == null) throw new Exception("Incorrect method call");
            CheckOnStep();
            if (Match(PredicateLexicon.TokenTypes["EMPTY"]) != null) return new FunctionNode{Name = functionName, Args = null};
            var args = new List<ExpressionNode>();
            do {
                var argument = ParseFormula();
                args.Add(argument);
            } while (Match(PredicateLexicon.TokenTypes["NEXT"]) != null);

            return new FunctionNode { Name = functionName, Args = args };
        }
        
        private ExpressionNode ParseParentheses() {
            if (Match(PredicateLexicon.TokenTypes["LPAR"]) == null) return ParseVariableOrNumber();
            var node = ParseFormula();
            Require(PredicateLexicon.TokenTypes["RPAR"]);
            return node;

        }
        private ExpressionNode ParseVariableOrNumber() {
            var token = Match(PredicateLexicon.TokenTypes["VALUE"], PredicateLexicon.TokenTypes["REFERENCE"], PredicateLexicon.TokenTypes["IDENTIFIER"]);
            if (token == null) throw new Exception("Awaits reference or value...");
            if (token.type.name == "VALUE") return new ValueNode { Value = token };
            if(token.type.name == "REFERENCE") return new VariableNode { Variable = token };
            var identifier = ParseIdentifier();
            var function = ParseFunctionInvocation();
            return new SenderNode{Receiver = identifier, Action = function};
        }

        private ExpressionNode ParseFormula() {
            var leftNode = ParseParentheses();
            var nodeOperator = Match(PredicateLexicon.TokenTypes["MINUS"], PredicateLexicon.TokenTypes["PLUS"], PredicateLexicon.TokenTypes["MULTIPLY"], PredicateLexicon.TokenTypes["DIVIDE"]);
            while (nodeOperator != null) {
                var rightNode = ParseParentheses();
                leftNode = new BinOperationNode{ Operator = nodeOperator, LeftExpression = leftNode, RightExpression = rightNode };
                nodeOperator = Match(PredicateLexicon.TokenTypes["MINUS"], PredicateLexicon.TokenTypes["PLUS"], PredicateLexicon.TokenTypes["MULTIPLY"], PredicateLexicon.TokenTypes["DIVIDE"]);
            }
            return leftNode;
        }

        
        private IdentifierNode ParseIdentifier() {
            var id = Require(PredicateLexicon.TokenTypes["VALUE"]);
            if (id == null) throw new Exception("ID of component is empty.");
            CheckOnStep();
            return new IdentifierNode { ID = id };
        }
    }
}
