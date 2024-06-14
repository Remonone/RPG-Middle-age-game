using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace RPG.Dialogs {

    public enum Executor {
        Player,
        Bot,
    }
    
    [Serializable]
    
    public class DialogNode : ScriptableObject {
        [SerializeField] private string _text;
        [SerializeField] private List<string> _children = new();
        [SerializeField] private Rect _rectangle = new(10,10,200,200);
        [SerializeField] private bool _isPlayer;
        [SerializeField] private Executor _onEnterExecutor;
        [SerializeField] private Executor _onExitExecutor;
        [SerializeField] private string _onExitPredicate;
        [SerializeField] private string _onEnterPredicate;

        public string OnExitPredicate {
            get =>  _onExitPredicate;
            set {
                if (_onExitPredicate == value) return;
#if UNITY_EDITOR
                Undo.RecordObject(this, "Update Dialog Node Exit Predicate");
#endif
                _onExitPredicate = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        public string OnEnterPredicate {
            get => _onEnterPredicate;
            set {
                if (_onEnterPredicate == value) return;
#if UNITY_EDITOR
                Undo.RecordObject(this, "Update Dialog Node Enter Predicate");
#endif
                _onEnterPredicate = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }

        public Executor EnterExecutor {
            get => _onEnterExecutor;
            set {
                if (_onEnterExecutor == value) return;
#if UNITY_EDITOR
                Undo.RecordObject(this, "Update Dialog Node Exit Predicate");
#endif
                _onEnterExecutor = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
    }

        public Executor ExitExecutor {
            get => _onExitExecutor;
            set {
                if (_onExitExecutor == value) return;
#if UNITY_EDITOR
                Undo.RecordObject(this, "Update Dialog Node Exit Predicate");
#endif
                _onExitExecutor = value;
#if UNITY_EDITOR
                EditorUtility.SetDirty(this);
#endif
            }
        }
        
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
