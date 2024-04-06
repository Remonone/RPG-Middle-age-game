using RPG.Combat.DamageDefinition;
using RPG.Combat.Modifiers.BaseTypes;
using RPG.Core.Predicate;
using RPG.Utils;
using UnityEngine;

namespace RPG.Combat.Modifiers {
    [CreateAssetMenu(menuName = "GumuPeachu/Combat/Create Fighter Event Modifier")]
    public class OnAttackEventModifier : Modification {
        
        protected Fighter Performer;
        [ReadOnly] [TextArea] public string returnArguments = "(0): attacker, (1): damage, (2): type, (3): target, (4): strength";

        public override void RegisterModification(GameObject performer) {
            Performer = performer.GetComponent<Fighter>();
            Performer.OnAttack += OnAttackTarget;
        }
        private void OnAttackTarget(DamageReport report) {
            report.Attacker.TryGet(out var attacker);
            report.Target.TryGet(out var target);
            var attackerID = ((PredicateMonoBehaviour)attacker.GetComponent(_performerComponent)).EntityID;
            var targetID = ((PredicateMonoBehaviour)target.GetComponent(_performToComponent)).EntityID;
            var preparedString = string.Format(_actionPredicate, attackerID, report.Damage, report.Type,
                targetID, Strength);
            PredicateWorker.ExecutePredicate(preparedString, attackerID, out _);
        }
        public override void UnregisterModification() {
            Performer.OnAttack -= OnAttackTarget;
        }
    }
}
