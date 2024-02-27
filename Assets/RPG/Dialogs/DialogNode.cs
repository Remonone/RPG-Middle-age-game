using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogs {
    
    [Serializable]
    
    public class DialogNode : ScriptableObject {
        [SerializeField] private string _text;
        [SerializeField] private List<string> _children = new();
        [SerializeField] private Rect _rectangle = new(10,10,200,100);
        [SerializeField] private bool _isPlayer;
        // TODO: Invoke predicate worker on entering and exiting dialog node
        [SerializeField] private string _onExitPredicate;
        [SerializeField] private string _onEnterPredicate;

        public string OnExitPredicate => _onExitPredicate;
        public string OnEnterPredicate => _onEnterPredicate;
        
        public string Text {
            get => _text;
            set {
                if (_text == value) return;
                #if UNITY_EDITOR
                Undo.RecordObject(this, "Update Dialog Node Text");
                #endif
                _text = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        public List<string> Children => _children;

        public bool IsPlayer {
            get => _isPlayer;
            set {
                #if UNITY_EDITOR
                Undo.RecordObject(this, "Change Dialog Speaker");
                #endif
                _isPlayer = value;
            }
        }

        public Rect Rectangle => _rectangle;
        
        #if UNITY_EDITOR
        public void SetPosition(Vector2 newPosition) {
            Undo.RecordObject(this, "Move Node");
            _rectangle.position = newPosition;
            EditorUtility.SetDirty(this);
        }
        
        public void AddChild(string childID) {
            Undo.RecordObject(this, "Link Node");
            _children.Add(childID);
            EditorUtility.SetDirty(this);
        }
        
        public void RemoveChild(string childID) {
            Undo.RecordObject(this, "Unlink Node");
            _children.Remove(childID);
            EditorUtility.SetDirty(this);
        }
        #endif
    }
}
