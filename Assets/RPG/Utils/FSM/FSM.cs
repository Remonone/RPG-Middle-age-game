using System.Collections.Generic;
using UnityEngine;

namespace RPG.Utils.FSM {
    public class FSM {
        private Stack<FSMState> stateStack = new();

        public delegate void FSMState(FSM fsm, GameObject go);

        public void Update(GameObject go) {
            if (stateStack.Peek() != null) {
                stateStack.Peek().Invoke(this, go);
            }
        }
        
        public void PushState(FSMState state) => stateStack.Push(state);
    
        public void PopState() => stateStack.Pop();
    }
}
