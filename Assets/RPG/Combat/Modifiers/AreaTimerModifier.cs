using System;
using System.Collections;
using RPG.Combat.Modifiers.BaseTypes;
using RPG.Core.Predicate;
using UnityEngine;

namespace RPG.Combat.Modifiers {
    [CreateAssetMenu(fileName = "Timer Area Modifier", menuName = "GumuPeachu/Combat/Create Timer Area Modifier")]
    public class AreaTimerModifier : AreaModifier {

        [SerializeField] private float _cooldown;

        private Coroutine _performerCoroutine;

        protected override void OnRegisterModification(Func<GameObject, PredicateMonoBehaviour[]> func) {
            _performerCoroutine = this.Performer.StartCoroutine(HandleTargets(func));
        }
        
        private IEnumerator HandleTargets(Func<GameObject, PredicateMonoBehaviour[]> func) {
            while (true) {
                var components = func.Invoke(Performer.gameObject);
                foreach (var com in components) {
                    string predicate = string.Format(_actionPredicate, Performer.ComponentID, com.ComponentID);
                    PredicateWorker.ExecutePredicate(predicate, Performer.ComponentID, out _);
                }
                yield return new WaitForSeconds(_cooldown);
            }
        }
        
        protected override void OnUnregisterModification() {
            Performer.StopCoroutine(_performerCoroutine);
        }
    }
}
