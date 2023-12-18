using System;
using System.Linq;
using RPG.Core.Predicate;
using UnityEngine;

namespace RPG.Dialogs {
    public class Conversant : PredicateMonoBehaviour {
        [SerializeField] private string _entityName;
        [SerializeField] private bool _isPlayer;
        
        private Dialog _dialog;
        private DialogNode _currentNode;
        private bool _isChoosing;
        private Conversant _conversant;

        public event Action OnUpdate;

        public string CurrentConversantName => _isChoosing ? _entityName : _conversant._entityName;
        public bool IsActive => _dialog != null;
        
        public string GetCurrentText() {
            return _currentNode == null ? "" : _currentNode.Text;
        }

        public void StartDialog(Dialog dialog, Conversant associate) {
            _conversant = associate;
            _dialog = dialog;
            _currentNode = dialog.GetRootNode();
            OnEnterAction();
            OnUpdate?.Invoke();
        }

        public void Quit() {
            OnExitAction();
            _conversant = null;
            _currentNode = null;
            _dialog = null;
            _isChoosing = false;
            OnUpdate?.Invoke();
        }

        public void Next() {
            if (_isPlayer) {
                var nodes = _dialog.GetPlayerChildren(_currentNode);
                if (nodes.Any()) {
                    _isChoosing = true;
                    OnExitAction();
                    OnUpdate?.Invoke();
                    return;
                }
                var aiNodes = _dialog.GetAIChildren(_currentNode).ToArray();
                OnExitAction();
                _currentNode = aiNodes[0];
                OnEnterAction();
                OnUpdate?.Invoke();
                return;
            } 
            _conversant.Next();
        }
        
        private void OnEnterAction() {
            if (_dialog == null) return;
            TriggerAction(_currentNode.OnEnterPredicate);
        }

        private void OnExitAction() {
            if (_dialog == null) return;
            TriggerAction(_currentNode.OnExitPredicate);
        }
        private void TriggerAction(string actionPredicate) {
            if (actionPredicate == "") return;
            PredicateWorker.ParsePredicate(actionPredicate, ComponentID);
        }
        
        public override void Predicate(string command, object[] arguments, out object result) {
            result = "";
        }
    }
}
