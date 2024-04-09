using System;
using RPG.Utils.Constants;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogs.Editor {
    public class DialogEditor : EditorWindow {
        // STYLES
        [NonSerialized] private static GUIStyle _style;
        [NonSerialized] private static GUIStyle _playerStyle;
        
        // NODES
        [NonSerialized] private DialogNode _creatingNode;
        [NonSerialized] private DialogNode _deleteNode;
        [NonSerialized] private DialogNode _linkingParentNode;
        
        // DRAGGING
        [NonSerialized] private bool _isScreenDragging = false;
        [NonSerialized] private DialogNode _draggingNode;
        [NonSerialized] private Vector2 _draggingOffset;
        [NonSerialized] private Vector2 _draggingCanvasOffset;
        
        private static Dialog _selectedDialog;
        private Vector2 _scrollPosition;
        
        [MenuItem("GumuPeachu/Dialog Editor")]
        private static void ShowEditorWindow() {
            var window = GetWindow<DialogEditor>();
            window.titleContent = new GUIContent("Dialog Builder");
            window.Show();
        }

        private void OnEnable() {
            Selection.selectionChanged += OnSelectionChanged;

            _style = new GUIStyle();
            _style.normal.background = EditorGUIUtility.Load("node0") as Texture2D;
            _style.border = new RectOffset(12, 12, 12, 12);
            _style.padding = new RectOffset(20, 20, 20, 20);

            _playerStyle = new GUIStyle();
            _playerStyle.normal.background = EditorGUIUtility.Load("node1") as Texture2D;
            _playerStyle.border = new RectOffset(12, 12, 12, 12);
            _playerStyle.padding = new RectOffset(20, 20, 20, 20);
        }

        private void OnDisable() {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged() {
            var dialog = EditorUtility.InstanceIDToObject(Selection.activeInstanceID) as Dialog;
            if (dialog == null) return;
            _selectedDialog = dialog;
            Repaint();
        }

        private void OnGUI() {
            if (_selectedDialog == null) return;
            HandleEvents();
                
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            var canvas = GUILayoutUtility.GetRect(PropertyConstants.CANVAS_SIZE, PropertyConstants.CANVAS_SIZE);
            var backgroundTexture = Resources.Load("background") as Texture2D;
            var textureSize = PropertyConstants.CANVAS_SIZE / PropertyConstants.BACKGROUND_SIZE;
            var textureCoords = new Rect(0, 0, textureSize, textureSize);
            GUI.DrawTextureWithTexCoords(canvas, backgroundTexture, textureCoords);
            foreach (var node in _selectedDialog.GetAllNodes()) {
                Debug.Log("drawing relations..");
                DrawRelations(node);
            }
            foreach (var node in _selectedDialog.GetAllNodes()) DrawNode(node);
            EditorGUILayout.EndScrollView();

            if (_creatingNode != null) {
                _selectedDialog.AddNewNode(_creatingNode);
                _creatingNode = null;
            }

            if (_deleteNode != null) {
                _selectedDialog.DeleteNode(_deleteNode);
                _deleteNode = null;
            }
        }
        private void DrawNode(DialogNode node) {
            var style = _style;
            if (node.IsPlayer) style = _playerStyle;
            GUILayout.BeginArea(node.Rectangle, style);
            var newText = EditorGUILayout.TextField(node.Text);
            node.Text = newText;
            
            var enumValues = Enum.GetValues(typeof(Executor));
            
            var enterPredicate = EditorGUILayout.TextField(node.OnEnterPredicate);
            node.OnEnterPredicate = enterPredicate;
            var enterMenu = new GenericMenu();
            foreach (var value in enumValues) {
                Debug.Log(value);
                enterMenu.AddItem(new GUIContent($"{value}"), false, (data) => {
                    node.EnterExecutor = (Executor)data;
                }, value);
            }
            
            if (EditorGUILayout.DropdownButton(new GUIContent("Select Actor..."), FocusType.Keyboard)) {
                enterMenu.ShowAsContext();
            }
            
            var exitPredicate = EditorGUILayout.TextField(node.OnExitPredicate);
            node.OnExitPredicate = exitPredicate;
            var exitMenu = new GenericMenu();
            foreach (var value in enumValues) {
                Debug.Log(value);
                exitMenu.AddItem(new GUIContent($"{value}"), false, (data) => {
                    node.EnterExecutor = (Executor)data;
                }, value);
            }
            
            if (EditorGUILayout.DropdownButton(new GUIContent("Select Actor..."), FocusType.Keyboard)) {
                exitMenu.ShowAsContext();
            }
            
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("+")) _creatingNode = node;
            if (_linkingParentNode == null) {
                if (GUILayout.Button("Link")) _linkingParentNode = node;
            } else if (ReferenceEquals(node, _linkingParentNode)) {
                if (GUILayout.Button("Cancel")) _linkingParentNode = null;
            } else if (_linkingParentNode.Children.Contains(node.name)) {
                if (GUILayout.Button("Unlink")) {
                    _linkingParentNode.RemoveChild(node.name);
                    _linkingParentNode = null;
                }
            } else {
                if (GUILayout.Button("Apply")) {
                    _linkingParentNode.AddChild(node.name);
                    _linkingParentNode = null;
                }
            }

            if (GUILayout.Button("-")) _deleteNode = node;
            
            GUILayout.EndHorizontal();
            GUILayout.EndArea();
        }


        private void DrawRelations(DialogNode node) {
            var start = new Vector3(node.Rectangle.xMax, node.Rectangle.center.y, 0);

            foreach (var child in _selectedDialog.GetAllChildren(node)) {
                var end = new Vector3(child.Rectangle.xMin, child.Rectangle.center.y, 0);
                var controlPoint = end - start;
                controlPoint.y = 0;
                controlPoint.x *= .7f;
                Handles.DrawBezier(start, end, start + controlPoint, end - controlPoint, Color.white, null, 3f);
            }
        }

        private void HandleEvents() {
            if (Event.current.type == EventType.MouseDown && _draggingNode == null) {
                _draggingNode = GetNodeAtPoint(Event.current.mousePosition + _scrollPosition);
                if (_draggingNode != null) {
                    _draggingOffset = _draggingNode.Rectangle.position - Event.current.mousePosition;
                    Selection.activeObject = _draggingNode;
                } else {
                    _isScreenDragging = true;
                    _draggingCanvasOffset = Event.current.mousePosition + _scrollPosition;
                    Selection.activeObject = _selectedDialog;
                }
            }

            if (Event.current.type == EventType.MouseDrag && _draggingNode != null) {
                _draggingNode.SetPosition(Event.current.mousePosition + _draggingOffset);
                Repaint();
            }

            if (Event.current.type == EventType.MouseDrag && _isScreenDragging) {
                _scrollPosition = _draggingCanvasOffset - Event.current.mousePosition;
                Repaint();
            }

            if (Event.current.type == EventType.MouseUp) {
                _isScreenDragging = false;
                _draggingNode = null;
            }
        }
        private DialogNode GetNodeAtPoint(Vector2 currentPosition) => _selectedDialog.GetNode(currentPosition);
    }
}
