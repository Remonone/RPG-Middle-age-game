using System.Collections;
using RPG.Combat;
using RPG.Core.Predicate;
using UnityEngine;

namespace RPG.Inventories.Modifiers {
    [CreateAssetMenu(fileName = "AreaModifier", menuName = "GumuPeachu/Create Modifier", order = 0)]
    public class AreaModifier : Modification {

        [SerializeField] private float _range;
        [SerializeField] private LayerMask _layer;
        [SerializeField] private string _performerComponent;
        [SerializeField] private string _performToComponent;
        [SerializeField] private float _cooldown;
        
        private Coroutine _registeredCoroutine;
        private Fighter _coroutinePerformer;
        
        public override void RegisterModification(GameObject performer) {
            _coroutinePerformer = performer.GetComponent<Fighter>();
            _registeredCoroutine = _coroutinePerformer.StartCoroutine(BurnArea(performer));
        }
        
        private IEnumerator BurnArea(GameObject performer) {
            while (true) {
                if (string.IsNullOrEmpty(_performerComponent)) yield break;
                var holderPosition = performer.transform.position;
                var invokerComponent = (PredicateMonoBehaviour)performer.GetComponent(_performerComponent);
                var targets = GetTargetsInRange(holderPosition);
                foreach (var target in targets) {
                    var component = (PredicateMonoBehaviour)target.GetComponent(_performToComponent);
                    PredicateWorker.ParsePredicate(
                        component == null
                            ? string.Format(_actionPredicate, invokerComponent.ComponentID)
                            : string.Format(_actionPredicate, invokerComponent.ComponentID, 
                                component.ComponentID), "all");
                }

                yield return new WaitForSeconds(_cooldown);
            }
        }
        
        public override void UnregisterModification() {
            _coroutinePerformer.StopCoroutine(_registeredCoroutine);
        }

        private Collider[] GetTargetsInRange(Vector3 position) {
            return Physics.OverlapSphere(position, _range, _layer);
        }
        
        // Когда персонаж надевает предмет, его пассивная часть должна
        // начать вызываться когда-либо и как-либо
        // Варианты: Регистрация модификации, где будет установлено то,
        // как оно будет вызываться и когда.
        // Вопросы: 
        // 1. Как передавать необходимую информацию модификатору?
        // Ответы:
        // 1. Представим на примере. При атаке противника предмет наложит
        //    сверху стак дебафа проклятия. В этом случае нужно обладать
        //    информацией о том, кто атаковал и кто был атакован.
        //    Вариантом станет добавление ивента в Fighter
        //    который будет отслеживать, когда был нанесен урон противнику.

    }
}
