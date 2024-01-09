using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core.Predicate.Nodes;
using RPG.Utils;
using UnityEngine;

namespace RPG.Core.Predicate {
    public static class PredicateWorker {
        
        private static readonly Dictionary<string, PredicateMonoBehaviour> PredicateBehavioursStore = new();

        private static readonly PredicateLexer Lexer = new();
        private static readonly PredicateParser Parser = new();

        private static readonly Dictionary<string, Dictionary<string, object>> VariableStore = new();

        public static bool RegisterPredicate(string id, PredicateMonoBehaviour predicate) {
            if (PredicateBehavioursStore.ContainsKey(id)) return false;
            PredicateBehavioursStore[id] = predicate;
            return true;
        }

        public static bool DestroyPredicate(string id) {
            return PredicateBehavioursStore.Remove(id);
        }

        public static PredicateMonoBehaviour GetPredicateMonoBehaviour(string id) {
            return PredicateBehavioursStore[id];
        }

        public static object ParsePredicate(string parse, string sessionID) {
            try {
                var list = Lexer.LexAnalysis(parse);
                if (PropertyConstants.SHOULD_PRINT_PREDICATES) {
                    foreach (var item in list) {
                        Debug.Log("Type: " + item.type + "; Value: " + item.text);
                    }
                }
                if (!VariableStore.ContainsKey(sessionID)) VariableStore[sessionID] = new Dictionary<string, object>();
                var treeNode = Parser.ParseCode(list);
                var result = RunNodes(treeNode, sessionID, out object getResult);
                if (PropertyConstants.PREVIEW_PREDICATE_RESULT) {
                    Debug.Log(result);
                }

                return getResult;
            }
            catch (Exception e) {
                Debug.LogError(e);
            }

            return null;
        }

        private static object RunNodes(ExpressionNode node, string sessionID, out object getResult) {
            object objects = null;
            getResult = "";
            if (node.GetType() == typeof(StatementNode)) {
                foreach (var expression in ((StatementNode)node).Nodes) RunNodes(expression, sessionID, out getResult);
            }
            if (node.GetType() == typeof(SenderNode)) {
                var sender = (SenderNode)node;
                var receiver = sender.Receiver.ID;
                var id = Convert.ToString(RunNodes(receiver, sessionID, out getResult));
                var component = PredicateBehavioursStore[id];
                if (component != null) {
                    var args = sender.Action.Args;
                    var argsToSend = args != null ? 
                        args.Select(arg => RunNodes(arg, sessionID, out _)).ToArray() : Array.Empty<object>();
                    component.Predicate(sender.Action.Name.text,
                        argsToSend, out var result);
                    objects = result;
                }
            }

            if (node.GetType() == typeof(AssignNode)) {
                var assign = (AssignNode)node;
                VariableStore[sessionID][assign.Name.text] = RunNodes(assign.Value, sessionID, out _);
            }

            if (node.GetType() == typeof(DeleteNode)) {
                var delete = (DeleteNode)node;
                if (VariableStore[sessionID].ContainsKey(delete.ToDelete.text)) {
                    VariableStore[sessionID].Remove(delete.ToDelete.text);
                }
                else {
                    throw new Exception($"Variable {delete.ToDelete.text} has not declared");
                }
            }

            if (node.GetType() == typeof(ValueNode)) {
                objects = ((ValueNode)node).Value.text;
            }

            if (node.GetType() == typeof(VariableNode)) {
                var variable = (VariableNode)node;

                if (VariableStore[sessionID].ContainsKey(variable.Variable.text)) {
                    objects = VariableStore[sessionID][variable.Variable.text];
                }
                else {
                    throw new Exception($"Variable {variable.Variable.text} has not declared");
                }
            }

            if (node.GetType() == typeof(BinOperationNode)) {
                var oper = (BinOperationNode)node;
                var left = Convert.ToDouble(RunNodes(oper.LeftExpression, sessionID, out _));
                var right = Convert.ToDouble(RunNodes(oper.RightExpression, sessionID, out _));
                if (oper.Operator.type == PredicateLexicon.TokenTypes["PLUS"]) objects = left + right;
                if (oper.Operator.type == PredicateLexicon.TokenTypes["MINUS"]) objects = left - right;
                if (oper.Operator.type == PredicateLexicon.TokenTypes["MULTIPLY"]) objects = left * right;
                if (oper.Operator.type == PredicateLexicon.TokenTypes["DIVIDE"]) objects = left / right;
            }

            if (node.GetType() == typeof(GetNode)) {
                var getNode = (GetNode)node;
                var variableName = Convert.ToString(getNode.Variable);
                getResult = VariableStore[sessionID][variableName];
            }
            
            return objects;
        }
    }
}
