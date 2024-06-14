using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core;
using RPG.Core.Predicate;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace RPG.Dialogs {
    public class PlayerConversant : NetworkBehaviour, IAction {
        [SerializeField] private string _entityName;
        [SerializeField] private float _conversationRange;

        private Dialog _dialog;
        private DialogNode _currentNode;
        private bool _isChoosing;
        private AIConversant _aiConversant;
        private PredicateMonoBehaviour _predicate;
        private TaskScheduler _scheduler;

        public event Action OnUpdate;

        public string CurrentConversantName => _isChoosing ? _entityName : _aiConversant.EntityName;
        public bool IsActive => _dialog != null;
        public bool IsChoosing => _isChoosing;

        
        public string GetCurrentText() => ReferenceEquals(_currentNode, null) ? "" : _currentNode.Text;

        public IEnumerable<DialogNode> GetChoices() => _dialog.GetPlayerChildren(_currentNode);

        private void Awake() {
            _predicate = GetComponent<PredicateMonoBehaviour>();
            _scheduler = GetComponent<TaskScheduler>();
        }
        
        private void Update() {
            if (ReferenceEquals(_aiConversant, null)) return;
            if (IsConversantInRange()) {
                InitDialog();
            }
        }
        private void InitDialog() {
            _currentNode = _dialog.GetRootNode();
            OnEnterAction();
            OnUpdate?.Invoke();
        }

        public void StartDialog(Dialog dialog, AIConversant associate) {
            _aiConversant = associate;
            _dialog = dialog;
            _scheduler.SwitchAction(this);
            StartDialogServerRpc(new FixedString512Bytes(dialog.name), associate.GetComponent<NetworkObject>());
        }

        [ServerRpc]
        private void StartDialogServerRpc(FixedString512Bytes dialogName, NetworkObjectReference conversant) {
            conversant.TryGet(out var conversantNet);
            var dialog = Dialog.GetDialogByName(dialogName.Value);
            var associate = conversantNet.GetComponent<AIConversant>();
            _aiConversant = associate;
            _dialog = dialog;
            _scheduler.SwitchAction(this);
        }

        public void Quit() {
            OnExitAction();
            Cancel();
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
            TriggerAction(_currentNode.OnEnterPredicate, GetIdFromExecutor(_currentNode.EnterExecutor));
        }

        private void OnExitAction() {
            if (ReferenceEquals(_dialog, null)) return;
            TriggerAction(_currentNode.OnExitPredicate, GetIdFromExecutor(_currentNode.ExitExecutor));
        }
        
        private void TriggerAction(string actionPredicate, string executor) {
            if (actionPredicate == "" || IsServer) return;
            var formattedPredicate = string.Format(actionPredicate, executor);
            PredicateWorker.ExecutePredicate(formattedPredicate, _predicate.EntityID, out _);
        }

        private string GetIdFromExecutor(Executor executor) {
            switch (executor) {
                case Executor.Player:
                    return GetComponent<PredicateMonoBehaviour>().EntityID;
                case Executor.Bot:
                    return _aiConversant.GetComponent<PredicateMonoBehaviour>().EntityID;
                default:
                    return "";
            }
        }

        public void SelectChoice(DialogNode choice) {
            TriggerAction(_currentNode.OnExitPredicate, GetIdFromExecutor(_currentNode.ExitExecutor));
            _currentNode = choice;
            TriggerAction(_currentNode.OnEnterPredicate, GetIdFromExecutor(_currentNode.ExitExecutor));
            _isChoosing = !_isChoosing;
            SelectChoiceServerRpc(choice.Rectangle.position);
        }

        [ServerRpc]
        private void SelectChoiceServerRpc(Vector2 choicePosition) {
            TriggerAction(_currentNode.OnExitPredicate, GetIdFromExecutor(_currentNode.ExitExecutor));
            _currentNode = _dialog.GetNode(choicePosition);
            TriggerAction(_currentNode.OnEnterPredicate, GetIdFromExecutor(_currentNode.EnterExecutor));
            _isChoosing = !_isChoosing;
            Next();
        }
        
        public bool HasNext() {
            return _dialog.GetAllChildren(_currentNode).Any();
        }
        
        private bool IsConversantInRange() {
            var distanceToTarget = Vector3.Distance(transform.position, _aiConversant.transform.position);
            return distanceToTarget <= _conversationRange;
        }
        
        public void Cancel() {
            _aiConversant = null;
            _currentNode = null;
            _dialog = null;
            _isChoosing = false;
        }
    }
}
