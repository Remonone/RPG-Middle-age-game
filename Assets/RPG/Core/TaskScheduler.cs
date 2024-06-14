using UnityEngine;

namespace RPG.Core {
    public class TaskScheduler : MonoBehaviour {
        
        private IAction _currentAction;

        public void SwitchAction(IAction action) {
            if (ReferenceEquals(_currentAction, action)) return;
            _currentAction?.Cancel();
            _currentAction = action;
        }

        public void CancelAction() {
            if (_currentAction == null) return;
            _currentAction.Cancel();
            _currentAction = null;
        }
    }
}
