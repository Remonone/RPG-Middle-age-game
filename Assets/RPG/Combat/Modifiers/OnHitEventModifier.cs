using RPG.Combat.DamageDefinition;
using RPG.Combat.Modifiers.BaseTypes;
using RPG.Core.Predicate;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat.Modifiers {
    [CreateAssetMenu(menuName = "GumuPeachu/Combat/Create On Hit Event Modifier")]
    public class OnHitEventModifier : Modification {
        
        protected Health Performer;
        [ReadOnly] [TextArea] public string returnArguments = "(0): attacker, (1): damage, (2): type, (3): target, (4): strength";

        public override void RegisterModification(GameObject performer) {
            Performer = performer.GetComponent<Health>();
            Performer.OnHit += OnHit;
        }
        private void OnHit(DamageReport report) {
            var attackerID = ((PredicateMonoBehaviour)report.Attacker.GetComponent(_performerComponent)).EntityID;
            var targetID = ((PredicateMonoBehaviour)report.Target.GetComponent(_performToComponent)).EntityID;
            var preparedString = string.Format(_actionPredicate, attackerID, report.Damage, report.Type,
                targetID, Strength);
            PredicateWorker.ExecutePredicate(preparedString, attackerID, out _);
        }
        public override void UnregisterModification() {
            Performer.OnHit -= OnHit;
        }
    }
}
