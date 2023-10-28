using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core.Predicate.Nodes;
using UnityEngine;

namespace RPG.Core.Predicate {
    public static class PredicateWorker {
        
        private static Dictionary<string, PredicateMonoBehaviour> _predicateStore = new Dictionary<string, PredicateMonoBehaviour>();

        private static PredicateLexer _lexer = new PredicateLexer();
        private static PredicateParser _parser = new PredicateParser();

        // Internal 
        private static Dictionary<string, Dictionary<string, object>> _variableStore = new Dictionary<string, Dictionary<string, object>>();

        public static bool RegisterPredicate(string id, PredicateMonoBehaviour predicate) {
            if (_predicateStore.ContainsKey(id)) return false;
            _predicateStore[id] = predicate;
            return true;
        }

        public static bool DestroyPredicate(string id) {
            return _predicateStore.Remove(id);
        }

        public static void ParsePredicate(string parse, string sessionID) {
            try {
                var list = _lexer.LexAnalysis(parse);
                // foreach (var item in list) {
                //     Debug.Log("Type: " + item.type + "; Value: " + item.text);
                // }
                var treeNode = _parser.ParseCode(list);
                var result = RunNodes(treeNode, sessionID);
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
                var component = _predicateStore[sender.Receiver.ID.text];
                if (component != null) {
                    component.Predicate(sender.Action.Name.text,
                        sender.Action.Args.Select(arg => RunNodes(arg, sessionID)).ToArray(), out var result);
                    objects = result;
                }
            }

            if (node.GetType() == typeof(AssignNode)) {
                var assign = (AssignNode)node;
                _variableStore[sessionID][assign.Name.text] = RunNodes(assign.Value, sessionID);
            }

            if (node.GetType() == typeof(DeleteNode)) {
                var delete = (DeleteNode)node;
                _variableStore[sessionID].Remove(delete.ToDelete.text);
            }

            if (node.GetType() == typeof(ValueNode)) {
                objects = ((ValueNode)node).Value.text;
            }

            if (node.GetType() == typeof(VariableNode)) {
                var variable = (VariableNode)node;

                if (_variableStore[sessionID].ContainsKey(variable.Variable.text)) {
                    objects = _variableStore[sessionID][variable.Variable.text];
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
