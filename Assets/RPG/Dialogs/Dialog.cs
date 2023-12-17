using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogs {
    [CreateAssetMenu(fileName = "New Dialog", menuName = "GumuPeachu/Dialogs/Create New Dialog", order = 0)]
    public class Dialog : ScriptableObject, ISerializationCallbackReceiver {
        // TODO: Create proper handling of dialogNodes
        [SerializeField] private List<DialogNode> _nodes = new();

        private Dictionary<string, DialogNode> _nodeLookup = new();

        public DialogNode GetRootNode() => _nodes[0];

        public DialogNode GetNode(Vector2 point) => _nodes.FindLast(node => node.Rectangle.Contains(point));

        public IEnumerable<DialogNode> GetAllNodes() => _nodes;
        
        public IEnumerable<DialogNode> GetAllChildren(DialogNode node) {
            foreach (var id in node.Children) {
                if (_nodeLookup.ContainsKey(id)) yield return _nodeLookup[id];
            }
        }

        private void Awake() {
            FillNodesDict();
        }

        private void OnValidate() {
            if (_nodes.Count == 0) {
                AddNewNode(null);
            }
            _nodeLookup.Clear();
            FillNodesDict();
        }
        
        public void AddNewNode(DialogNode node) {
            var newNode = CreateNode(node);
            Undo.RegisterCreatedObjectUndo(newNode, "Create Dialog Node");
            Undo.RecordObject(this, "Added Dialog Node");
            _nodes.Add(newNode);
            OnValidate();
        }
        
        private DialogNode CreateNode(DialogNode parent) {
            var node = CreateInstance<DialogNode>();
            node.name = Guid.NewGuid().ToString();
            if (parent == null) return node;
            parent.Children.Add(node.name);
            node.IsPlayer = !parent.IsPlayer;
            node.SetPosition(parent.Rectangle.position + new Vector2(250, 0));
            return node;
        }
        
        public void DeleteNode(DialogNode nodeToDelete) {
            Undo.RecordObject(this, "Delete Dialog Node");
            _nodes.Remove(nodeToDelete);
            OnValidate();
            CleanDanglingChildren(nodeToDelete);
            Undo.DestroyObjectImmediate(nodeToDelete);
        }

        private void CleanDanglingChildren(DialogNode nodeToDelete) {
            foreach (var node in _nodes) {
                node.Children.Remove(nodeToDelete.name);
            }
        }


        private void FillNodesDict() {
            foreach (var node in _nodes) {
                _nodeLookup[node.name] = node;
            }
        }
        
        public void OnBeforeSerialize() {
            if (_nodes.Count == 0) {
                DialogNode node = CreateNode(null);
                _nodes.Add(node);
                OnValidate();
            }
            if (AssetDatabase.GetAssetPath(this) != null) {
                foreach (var node in _nodes) {
                    if (AssetDatabase.GetAssetPath(node) == "") {
                        AssetDatabase.AddObjectToAsset(node, this);
                    }
                }
            }
        }
        public void OnAfterDeserialize() { }
    }
}
