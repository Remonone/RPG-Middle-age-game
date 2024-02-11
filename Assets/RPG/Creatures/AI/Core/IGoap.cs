using System.Collections.Generic;

namespace RPG.Creatures.AI.Core {
    public interface IGoap {
        List<StateObject> GetCurrentState();

        List<StateObject> CreateGoal();

        void OnPlanFailed(List<StateObject> failedGoal);

        void OnPlanFound(List<StateObject> goal, Queue<GoapAction> actions);

        void OnActionsFinished();

        void OnPlanAborted(GoapAction aborter);

        bool MoveAgent(GoapAction action);
    }
}
