using System.Collections.Generic;
using UnityEngine;

namespace RPG.Utils.FSM {
    public class FSM {
        private readonly Stack<FSMState> _stateStack = new();

        public delegate void FSMState(FSM fsm, GameObject go);

        public void Update(GameObject go) {
            if (_stateStack.Peek() != null) {
                _stateStack.Peek().Invoke(this, go);
            }
        }
        
        public void PushState(FSMState state) => _stateStack.Push(state);
    
        public void PopState() => _stateStack.Pop();
    }
}
