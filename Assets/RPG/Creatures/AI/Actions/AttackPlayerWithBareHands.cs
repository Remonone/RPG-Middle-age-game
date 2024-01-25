using RPG.Combat;
using RPG.Creatures.AI.Core;
using RPG.Creatures.AI.Roles;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    public class AttackPlayerWithBareHands : GoapAction {

        private Health _targetToAttack;
        
        public AttackPlayerWithBareHands() {
            _prerequisites.Add(new StateObject {Name = "is_enemy_visible", Value = true});
            _prerequisites.Add(new StateObject {Name = "is_armed", Value = false});
            
            _effects.Add(new StateObject {Name = "liquidate_target", Value = true});
        }

        public override bool PerformAction(GameObject agent) {
            return !_targetToAttack.IsAlive;
        }
        
        public override void DoReset() {
            InRange = false;
            Target = null;
        }
        
        public override bool IsDone() {
            return !_targetToAttack.IsAlive;
        }
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            Target = agent.GetComponent<BaseAgentBehaviour>().CompletionTarget;
            if (!Target) return false;
            _targetToAttack = Target.GetComponent<Health>();
            return true;
        }
        
        public override bool RequiresInRange() {
            return true;
        }
    }
}
