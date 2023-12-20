using UnityEngine;
using UnityEngine.Events;

namespace RPG.Dialogs {
    public class DialogTrigger : MonoBehaviour {
        [SerializeField] private string _trigger;
        [SerializeField] private UnityEvent _onTrigger;

        public void Trigger(string action) {
            if(_trigger == action) _onTrigger?.Invoke();
        }
    }
}
