using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core.Predicate;
using RPG.Core.Predicate.Interfaces;
using UnityEngine;

namespace RPG.Dialogs {
    public class PlayerConversant : MonoBehaviour {
        [SerializeField] private string _entityName;

        private Dialog _dialog;
        private DialogNode _currentNode;
        private bool _isChoosing;
        private AIConversant _aiConversant;
        private PredicateMonoBehaviour _predicate;

        public event Action OnUpdate;

        public string CurrentConversantName => _isChoosing ? _entityName : _aiConversant.EntityName;
        public bool IsActive => _dialog != null;
        public bool IsChoosing => _isChoosing;

        
        public string GetCurrentText() => _currentNode == null ? "" : _currentNode.Text;

        public IEnumerable<DialogNode> GetChoices() => _dialog.GetPlayerChildren(_currentNode);

        public void StartDialog(Dialog dialog, AIConversant associate) {
            _aiConversant = associate;
            _dialog = dialog;
            _currentNode = dialog.GetRootNode();
            OnEnterAction();
            OnUpdate?.Invoke();
        }

        private void Awake() {
            _predicate = GetComponent<PredicateMonoBehaviour>();
        }

        public void Quit() {
            OnExitAction();
            _aiConversant = null;
            _currentNode = null;
            _dialog = null;
            _isChoosing = false;
            OnUpdate?.Invoke();
        }

        public void Next() {
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
        }
        
        private void OnEnterAction() {
            if (ReferenceEquals(_dialog, null)) return;
            TriggerAction(_currentNode.OnEnterPredicate);
        }

        private void OnExitAction() {
            if (ReferenceEquals(_dialog, null)) return;
            TriggerAction(_currentNode.OnExitPredicate);
        }
        // BUG: Need to pass component ID; Use Predicate as Component ID Receiver
        private void TriggerAction(string actionPredicate) {
            if (actionPredicate == "") return;
            PredicateWorker.ExecutePredicate(actionPredicate, _predicate.EntityID, out _);
        }

        public void SelectChoice(DialogNode choice) {
            TriggerAction(_currentNode.OnExitPredicate);
            _currentNode = choice;
            TriggerAction(_currentNode.OnEnterPredicate);
            _isChoosing = !_isChoosing;
            Next();
        }
        public bool HasNext() {
            return _dialog.GetAllChildren(_currentNode).Any();
        }
    }
}
