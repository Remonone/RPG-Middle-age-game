using System.Collections.Generic;
using UnityEngine;

namespace RPG.Creatures.AI.Core {
    public class GoapPlanner {
        
        public Queue<GoapAction> BuildPlan(GameObject agent, List<GoapAction> actions, List<GoapAction.KeyPair> prerequisites,
            List<GoapAction.KeyPair> goals) {
            foreach(var action in actions) action.DoReset();

            List<GoapAction> usableActions = new();
            foreach (var action in actions) {
                if (action.CheckProceduralPrerequisites(agent)) usableActions.Add(action);
            }

            List<Node> nodes = new();

            Node start = new Node(null, 0, prerequisites, null);

            if (!BuildGraph(start, nodes, usableActions, goals)) return null;

            Node cheapest = null;
            foreach (var node in nodes) {
                if (cheapest == null) {
                    cheapest = node;
                } else {
                    if (node.Cost < cheapest.Cost)
                        cheapest = node;
                }
            }

            List<GoapAction> result = new();
            Node n = cheapest;
            while (n != null) {
                if (n.Action != null) {
                    result.Insert(0, n.Action);
                }

                n = n.Parent;
            }

            var queue = new Queue<GoapAction>();
            foreach(var action in result) 
                queue.Enqueue(action);
            
            return null;
        }
        private bool BuildGraph(Node parent, List<Node> nodes, List<GoapAction> usableActions, List<GoapAction.KeyPair> goals) {
            bool isFound = false;

            foreach (var action in usableActions) {
                if (!IsInRequisite(parent.States, action.Prerequisites)) continue;
                List<GoapAction.KeyPair> newStates = PopulateState(parent.States, action.Effects);
                var node = new Node(parent, parent.Cost + action.Cost, newStates, action);

                if (IsInRequisite(newStates, goals)) {
                    nodes.Add(node);
                    isFound = true;
                } else {
                    List<GoapAction> subset = ActionSubset(usableActions, action);
                    var found = BuildGraph(node, nodes, subset, goals);
                    if (found) isFound = true;
                }
            }

            return isFound;
        }

        private bool IsInRequisite(List<GoapAction.KeyPair> current, List<GoapAction.KeyPair> requisites) {
            foreach (var requisite in requisites) {
                if (!current.Contains(requisite))
                    return false;
            }

            return true;
        }
        
        private List<GoapAction.KeyPair> PopulateState(List<GoapAction.KeyPair> parentStates, List<GoapAction.KeyPair> actionEffects) {
            List<GoapAction.KeyPair> newState = new();
            foreach(var state in parentStates) newState.Add(state);

            foreach (var effect in actionEffects) {
                bool isExisting = newState.Contains(effect);

                if (isExisting) {
                    newState.RemoveAll(pair => pair.Name == effect.Name);
                    var updatedEffect = effect.Clone();
                    newState.Add(updatedEffect);
                } else {
                    newState.Add(effect);
                }
            }

            return newState;
        }
        
        private List<GoapAction> ActionSubset(List<GoapAction> usableActions, GoapAction toRemove) {
            var subset = new List<GoapAction>();
            foreach (var action in usableActions) {
                if(!action.Equals(toRemove))
                    subset.Add(action);
            }

            return subset;
        }

        private class Node {
            public Node Parent;
            public float Cost;
            public List<GoapAction.KeyPair> States;
            public GoapAction Action;

            public Node(Node parent, float runningCost, List<GoapAction.KeyPair> prerequisites, GoapAction action) {
                Parent = parent;
                Cost = runningCost;
                States = prerequisites;
                Action = action;
            }
        }
    }
}
