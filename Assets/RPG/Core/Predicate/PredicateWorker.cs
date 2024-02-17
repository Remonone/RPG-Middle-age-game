using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
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
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="parse">Input string that should be operated</param>
        /// <param name="sessionID">Component ID that should be used as authority to operate with the system</param>
        /// <param name="result">output from parse string</param>
        /// <returns>True: has result, False: no result, NULL: unexpected error during parsing...</returns>
        public static bool? ExecutePredicate(string parse, string sessionID, out object result) {
            try {
                var list = Lexer.LexAnalysis(parse);
                // foreach (var item in list) {
                //     Debug.Log("Type: " + item.type + "; Value: " + item.text);
                // }
                if (!VariableStore.ContainsKey(sessionID)) VariableStore[sessionID] = new Dictionary<string, object>();
                var treeNode = Parser.ParseCode(list);
                result = RunNodes(treeNode, sessionID);
                if (result == null) return false;
                return true;
            }
            catch (Exception e) {
                Debug.LogError(e);
                result = "";
                return null;
            }
        }

        [CanBeNull]
        private static object RunNodes(ExpressionNode node, string sessionID) {
            if (node.GetType() == typeof(StatementNode)) {
                List<object> returnState = new();
                foreach (var expression in ((StatementNode)node).Nodes) returnState.Add(RunNodes(expression, sessionID));
                return returnState.Find(result => result != null);
            }
            if (node.GetType() == typeof(SenderNode)) {
                var sender = (SenderNode)node;
                var receiver = sender.Receiver.ID;
                var id = Convert.ToString(RunNodes(receiver, sessionID));
                var component = PredicateBehavioursStore[id];
                if (component != null) {
                    var args = sender.Action.Args;
                    var argsToSend = args != null ? args.Select(arg => RunNodes(arg, sessionID)) : Array.Empty<object>();
                    
                    var result = component.Predicate(sender.Action.Name.text, argsToSend.ToArray());
                    return result;
                }
            }

            if (node.GetType() == typeof(AssignNode)) {
                var assign = (AssignNode)node;
                VariableStore[sessionID][assign.Name.text] = RunNodes(assign.Value, sessionID);
                return null;
            }

            if (node.GetType() == typeof(DeleteNode)) {
                var delete = (DeleteNode)node;
                if (VariableStore[sessionID].ContainsKey(delete.ToDelete.text)) {
                    VariableStore[sessionID].Remove(delete.ToDelete.text);
                    return null;
                }
                
                throw new Exception($"Variable {delete.ToDelete.text} has not declared");
            }

            if (node.GetType() == typeof(ValueNode)) {
                return ((ValueNode)node).Value.text;
            }

            if (node.GetType() == typeof(VariableNode)) {
                var variable = (VariableNode)node;

                if (VariableStore[sessionID].ContainsKey(variable.Variable.text)) {
                    return VariableStore[sessionID][variable.Variable.text];
                }
                
                throw new Exception($"Variable {variable.Variable.text} has not declared");
            }

            if (node.GetType() == typeof(BinOperationNode)) {
                object result = null;
                var operation = (BinOperationNode)node;
                var left = Convert.ToDouble(RunNodes(operation.LeftExpression, sessionID));
                var right = Convert.ToDouble(RunNodes(operation.RightExpression, sessionID));
                if (operation.Operator.type == PredicateLexicon.TokenTypes["PLUS"]) result = left + right;
                if (operation.Operator.type == PredicateLexicon.TokenTypes["MINUS"]) result = left - right;
                if (operation.Operator.type == PredicateLexicon.TokenTypes["MULTIPLY"]) result = left * right;
                if (operation.Operator.type == PredicateLexicon.TokenTypes["DIVIDE"]) result = left / right;
                return result;
            }

            if (node.GetType() == typeof(ReturnNode)) {
                var returnNode = (ReturnNode)node;
                var toReturn = RunNodes(returnNode.ValueToReturn, sessionID);
                return toReturn;
            }

            return null;
        }
    }
}
