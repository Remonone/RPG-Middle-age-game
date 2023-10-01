using UnityEngine;

namespace RPG.Core {
    public class TaskScheduler : MonoBehaviour {
        
        private IAction _currentAction;

        public void SwitchAction(IAction action) {
            _currentAction.Cancel();
            _currentAction = action;
        }

        public void CancelAction() {
            _currentAction.Cancel();
            _currentAction = null;
        }
    }
}
