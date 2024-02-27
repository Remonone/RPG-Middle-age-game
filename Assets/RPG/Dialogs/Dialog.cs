using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogs {
    [CreateAssetMenu(fileName = "New Dialog", menuName = "GumuPeachu/Dialogs/Create New Dialog", order = 0)]
    public class Dialog : ScriptableObject, ISerializationCallbackReceiver {
        [SerializeField] private List<DialogNode> _nodes = new();

        private Dictionary<string, DialogNode> _nodeLookup = new();

        public DialogNode GetRootNode() => _nodes[0];

        public DialogNode GetNode(Vector2 point) => _nodes.FindLast(node => node.Rectangle.Contains(point));

        public IEnumerable<DialogNode> GetAllNodes() => _nodes;
        
        public IEnumerable<DialogNode> GetAllChildren(DialogNode current) {
            foreach (var id in current.Children) {
                if (_nodeLookup.ContainsKey(id)) yield return _nodeLookup[id];
            }
        }

        public IEnumerable<DialogNode> GetPlayerChildren(DialogNode current) {
            return GetAllChildren(current).Where(node => node.IsPlayer);
        }

        public IEnumerable<DialogNode> GetAIChildren(DialogNode current) {
            return GetAllChildren(current).Where(node => !node.IsPlayer);
        }


        private void Awake() {
            #if UNITY_EDITOR
            FillNodesDict();
            #endif
        }

        private void OnValidate() {
            #if UNITY_EDITOR
            if (_nodes.Count == 0) {
                AddNewNode(null);
            }
            _nodeLookup.Clear();
            FillNodesDict();
            #endif
        }
        
        #if UNITY_EDITOR
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
#endif
        
        public void OnBeforeSerialize() {
#if UNITY_EDITOR
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
#endif
        }
        
        public void OnAfterDeserialize() { }
    }
}
