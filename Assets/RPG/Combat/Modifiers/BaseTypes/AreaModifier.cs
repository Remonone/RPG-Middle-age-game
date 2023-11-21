using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Core.Predicate;
using UnityEngine;

namespace RPG.Combat.Modifiers.BaseTypes {
    public abstract class AreaModifier : Modification {

        [SerializeField] private float _range;
        [SerializeField] private LayerMask _layer;
        [SerializeField] private string _performerComponent;
        [SerializeField] private string _performToComponent;
        
        protected PredicateMonoBehaviour Performer;

        
        public override void RegisterModification(GameObject performer) {
            Performer = (PredicateMonoBehaviour)performer.GetComponent(_performerComponent);
            OnRegisterModification(CaptureObjects);
        }


        protected abstract void OnRegisterModification(Func<GameObject, PredicateMonoBehaviour[]> func);
        
        private PredicateMonoBehaviour[] CaptureObjects(GameObject performer) {
            if (ReferenceEquals(Performer, null)) return null;
            var position = performer.transform.position;
            var colliders = GetTargetsInRange(position);
            var objects = colliders.Select(collider => collider.gameObject).ToArray();
            return CollectBehaviours(objects);
            
        }

        private PredicateMonoBehaviour[] CollectBehaviours(GameObject[] targets) {
            List<PredicateMonoBehaviour> predicates = new();
            foreach (var obj in targets) {
                var component = (PredicateMonoBehaviour)obj.GetComponent(_performToComponent);
                if(component == null) continue;
                predicates.Add(component);
            }

            return predicates.ToArray();
        }
        
        public override void UnregisterModification() {
            OnUnregisterModification();
        }
        
        protected abstract void OnUnregisterModification();

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
        
        // Сделать разные модификаторы: Модификатор области при ивенте,
        // модификатор области в течении некоторого времени
        
    }
}
