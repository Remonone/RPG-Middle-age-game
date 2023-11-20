using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core.Predicate.Nodes;
using UnityEngine;

namespace RPG.Core.Predicate {
    public static class PredicateWorker {
        
        private static readonly Dictionary<string, PredicateMonoBehaviour> PredicateStore = new Dictionary<string, PredicateMonoBehaviour>();

        private static readonly PredicateLexer Lexer = new();
        private static readonly PredicateParser Parser = new();

        // Internal 
        private static readonly Dictionary<string, Dictionary<string, object>> VariableStore = new Dictionary<string, Dictionary<string, object>>();

        public static bool RegisterPredicate(string id, PredicateMonoBehaviour predicate) {
            if (PredicateStore.ContainsKey(id)) return false;
            PredicateStore[id] = predicate;
            return true;
        }

        public static bool DestroyPredicate(string id) {
            return PredicateStore.Remove(id);
        }

        public static PredicateMonoBehaviour GetPredicateMonoBehaviour(string id) {
            return PredicateStore[id];
        }

        public static void ParsePredicate(string parse, string sessionID) {
            Debug.Log(parse);
            try {
                var list = Lexer.LexAnalysis(parse);
                // foreach (var item in list) {
                //     Debug.Log("Type: " + item.type + "; Value: " + item.text);
                // }
                var treeNode = Parser.ParseCode(list);
                // TODO: Log information
                RunNodes(treeNode, sessionID);
            }
            catch (Exception e) {
                Debug.LogError(e);
            }
        }

        private static object RunNodes(ExpressionNode node, string sessionID) {
            object objects = null;
            if (node.GetType() == typeof(StatementNode)) {
                foreach (var expression in ((StatementNode)node).Nodes) RunNodes(expression, sessionID);
            }
            if (node.GetType() == typeof(SenderNode)) {
                var sender = (SenderNode)node;
                var receiver = sender.Receiver.ID;
                var id = Convert.ToString(RunNodes(receiver, sessionID));
                var component = PredicateStore[id];
                if (component != null) {
                    component.Predicate(sender.Action.Name.text,
                        sender.Action.Args.Select(arg => RunNodes(arg, sessionID)).ToArray(), out var result);
                    objects = result;
                }
            }

            if (node.GetType() == typeof(AssignNode)) {
                var assign = (AssignNode)node;
                VariableStore[sessionID][assign.Name.text] = RunNodes(assign.Value, sessionID);
            }

            if (node.GetType() == typeof(DeleteNode)) {
                var delete = (DeleteNode)node;
                VariableStore[sessionID].Remove(delete.ToDelete.text);
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
                var left = (decimal)RunNodes(oper.LeftExpression, sessionID);
                var right = (decimal)RunNodes(oper.LeftExpression, sessionID);
                if (oper.Operator.type.name == PredicateLexicon.TokenTypes["PLUS"].name) objects = left + right;
                if (oper.Operator.type.name == PredicateLexicon.TokenTypes["MINUS"].name) objects = left - right;
                if (oper.Operator.type.name == PredicateLexicon.TokenTypes["MULTIPLY"].name) objects = left * right;
                if (oper.Operator.type.name == PredicateLexicon.TokenTypes["DIVIDE"].name) objects = left / right;
            }
            
            return objects;
        }
    }
}
