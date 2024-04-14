using System.Collections.Generic;
using RPG.Utils.FSM;
using Unity.Netcode;
using UnityEngine;

namespace RPG.Creatures.AI.Core {
    public sealed class GoapAgent : NetworkBehaviour {
        [SerializeField] private List<GoapAction> _availableActions = new();
        
        private Queue<GoapAction> _currentActions = new();
        private readonly FSM _stateMachine = new();
        private readonly GoapPlanner _planner = new();
        
        private FSM.FSMState _idleState;
        private FSM.FSMState _moveToState;
        private FSM.FSMState _performActionState;
        private IGoap _provider;

        public override void OnNetworkSpawn() {
            if (!IsServer) return;
            _provider = GetComponent<IGoap>();
            CreateIdleState();
            CreateMoveToState();
            CreatePerformActionState();
            _stateMachine.PushState(_idleState);
        }

        private void Update() {
            if (!IsServer) return;
            _stateMachine.Update(gameObject);
        }

        private void CreateIdleState() {
            _idleState = (fsm, go) => {
                List<StateObject> currentState = _provider.GetCurrentState();
                List<StateObject> goal = _provider.CreateGoal();

                Queue<GoapAction> plan = _planner.BuildPlan(go, _availableActions, currentState, goal);
                if (plan != null) {
                    _currentActions = plan;
                    _provider.OnPlanFound(goal, plan);
                    fsm.PopState();
                    fsm.PushState(_performActionState);
                } else {
                  _provider.OnPlanFailed(goal);
                  fsm.PopState();
                  fsm.PushState(_idleState);
                }
            };
        }

        private void CreateMoveToState() {
            _moveToState = (fsm, _) => {
                GoapAction action = _currentActions.Peek();

                if (action.RequiresInRange() && action.Target == null) {
                    fsm.PopState();
                    fsm.PopState();
                    fsm.PushState(_idleState);
                }

                if (_provider.MoveAgent(action)) {
                    fsm.PopState();
                }
            };
        }

        private void CreatePerformActionState() {
            _performActionState = (fsm, agent) => {
                if (_currentActions.Count == 0) {
                    fsm.PopState();
                    fsm.PushState(_idleState);
                    _provider.OnActionsFinished();
                }

                GoapAction action = _currentActions.Peek();

                if (action.IsDone()) _currentActions.Dequeue();

                if (_currentActions.Count > 0) {
                    action = _currentActions.Peek();
                    if (!action.RequiresInRange() || action.InRange) {
                        if (!action.PerformAction(agent)) {
                            fsm.PopState();
                            fsm.PushState(_idleState);
                            _provider.OnPlanAborted(action);
                        }
                    } else {
                      fsm.PushState(_moveToState);  
                    }
                } else {
                  fsm.PopState();
                  fsm.PushState(_idleState);
                  _provider.OnActionsFinished();
                }
            };
        }
    }

}
