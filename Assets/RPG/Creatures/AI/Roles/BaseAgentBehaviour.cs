using System.Collections.Generic;
using RPG.Creatures.AI.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Creatures.AI.Roles {
    public abstract class BaseAgentBehaviour : MonoBehaviour, IGoap {
        
        [SerializeField] protected AiVision _vision;
        [SerializeField] private Mover _mover;
        [SerializeField] private float _rangeThreshold;

        protected GameObject Target;

        public GameObject CompletionTarget => Target;

        public abstract List<StateObject> GetCurrentState();
        public abstract List<StateObject> CreateGoal();
        public virtual void OnPlanFailed(List<StateObject> failedGoal) { }
        public virtual void OnPlanFound(List<StateObject> goal, Queue<GoapAction> actions) { }
        public virtual void OnActionsFinished() { }
        public virtual void OnPlanAborted(GoapAction aborter) { }
        public virtual bool MoveAgent(GoapAction action) {
            _mover.StartMovingToPoint(action.Target.transform.position);
            if (!((transform.position - action.Target.transform.position).magnitude < _rangeThreshold)) return false;
            action.InRange = true;
            return true;

        }
    }
}
