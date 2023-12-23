using System.Collections.Generic;
using UnityEngine;

namespace RPG.Creatures.AI.Core {
    public interface IGoap {
        List<GoapAction.KeyPair> GetCurrentState();

        List<GoapAction.KeyPair> CreateGoal();

        void OnPlanFailed(List<GoapAction.KeyPair> failedGoal);

        void OnPlanFound(List<GoapAction.KeyPair> goal, Queue<GoapAction> actions);

        void OnActionsFinished();

        void OnPlanAborted(GoapAction aborter);

        bool MoveAgent(GoapAction action);
    }
}
