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
            report.Attacker.TryGet(out var attacker);
            report.Target.TryGet(out var target);
            var attackerID = attacker.GetComponent<PredicateMonoBehaviour>().EntityID;
            var targetID = target.GetComponent<PredicateMonoBehaviour>().EntityID;
            var preparedString = string.Format(_actionPredicate, attackerID, _performToComponent, report.Damage, report.Type,
                targetID, Strength);
            PredicateWorker.ExecutePredicate(preparedString, attackerID, out _);
        }
        public override void UnregisterModification() {
            Performer.OnHit -= OnHit;
        }
    }
}
