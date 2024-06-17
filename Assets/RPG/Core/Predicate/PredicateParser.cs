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

            if (Match(PredicateLexicon.TokenTypes["GET"]) != null) {
                CheckOnStep();
                var variable = ParseVariableOrNumber();
                return new ReturnNode{ValueToReturn = variable};
            }
            if (Match(PredicateLexicon.TokenTypes["VARIABLE"]) == null)
                throw new Exception("Expected operating with component or variable operator");
            CheckOnStep();
            var variableName = Match(PredicateLexicon.TokenTypes["VALUE"]);
            if (variableName == null) throw new Exception("Variable name is empty/incorrect");
            CheckOnStep();
            ExpressionNode value;
            if (Match(PredicateLexicon.TokenTypes["IDENTIFIER"]) != null) {
                var identifierNode = ParseIdentifier();
                var functionNode = ParseFunctionInvocation();
                value = new SenderNode { Receiver = identifierNode, Action = functionNode };
            } else {
                value = ParseFormula();   
            }
            return new AssignNode { Name = variableName, Value = value };
        }

        private void CheckOnStep() {
            if (Match(PredicateLexicon.TokenTypes["STEP"]) == null)
                throw new Exception("Empty invocation");
        }
        
        private FunctionNode ParseFunctionInvocation() {
            var functionName = Require(PredicateLexicon.TokenTypes["VALUE"]);
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
            var token = Require(PredicateLexicon.TokenTypes["VALUE"], PredicateLexicon.TokenTypes["REFERENCE"]);
            if (token.type.name == "VALUE") return new ValueNode { Value = token };
            if(token.type.name == "REFERENCE") return new VariableNode { Variable = Require(PredicateLexicon.TokenTypes["VALUE"]) };
            throw new Exception("Type does not match.");
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
            var id = Require(PredicateLexicon.TokenTypes["VALUE"], PredicateLexicon.TokenTypes["REFERENCE"]);
            ExpressionNode node;
            if (id.type.name == "REFERENCE") {
                id = Require(PredicateLexicon.TokenTypes["VALUE"]);
                node = new VariableNode { Variable = id };
            }
            else {
                node = new ValueNode { Value = id };
            }
            Require(PredicateLexicon.TokenTypes["NEXT"]);
            ExpressionNode componentNode;
            var component = Require(PredicateLexicon.TokenTypes["VALUE"], PredicateLexicon.TokenTypes["REFERENCE"]);
            if (component.type.name == "REFERENCE") {
                component = Require(PredicateLexicon.TokenTypes["VALUE"]);
                componentNode = new VariableNode { Variable = component };
            }
            else {
                componentNode = new ValueNode { Value = component };
            }
            CheckOnStep();
            return new IdentifierNode { ID = node, Component = componentNode };
        }
    }
}
