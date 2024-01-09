using RPG.Creatures.AI.Core;
using UnityEngine;

namespace RPG.Creatures.AI.Actions {
    public class GoToPointAction : GoapAction {

        public GoToPointAction() {
            _prerequisites.Add(new StateObject {Name = "IsSuspicious", Value = false});
            
            _effects.Add(new StateObject {Name = "reachDestination", Value = true});
        }

        public override bool PerformAction(GameObject agent) {
            // TODO: Aftermath on reaching point
            return false;
        }
        public override void DoReset() {
            InRange = false;
            Target = null;
        }
        public override bool IsDone() {
            return Target == null || (transform.position - Target.transform.position).magnitude < .3f;
        }
        public override bool CheckProceduralPrerequisites(GameObject agent) {
            // TODO: Checking if reaches a point
            return true;
        }
        public override bool RequiresInRange() {
            return true;
        }
    }
}
