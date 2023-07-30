using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyContent.Scripts;
using FSM;
using UnityEngine;
using Debug = Logger.Debug;
using Random = UnityEngine.Random;
using AgentConstants = AgentSicarioConstants.AgentSicarioStates;

public partial class AgentSicario {
    private EventFSM<string> _fsm;
    private Dictionary<string, IState> _agentExecutionStates;
    private Dictionary<string, State<string>> _agentStates;

    #region monobehaviour

    private void Start() {
        CreateAgentExecutionStates();
        FSMStates();
        SubscribeAgentExecutionStates();

        CreatePlan(out var actions, out var from, out var to);
        StartCoroutine(GoapRunDelegate(from, to, actions));
        _fsm = new EventFSM<string>(_agentStates[AgentConstants.IDLE]);
        _fsm.Feed(AgentConstants.IDLE);
        StartCoroutine(SicarioDecisions(1f));
    }
    
    private void Update() {
        _fsm.Update();
    }
    
    public virtual IEnumerator SicarioDecisions(float time) {
        while (true) {
            _fsm.Feed(AgentConstants.IDLE);
            if (_goapPlan != null) {
                GOAPActionDelegate action = _goapPlan.First();
            }

            yield return new WaitForSeconds(time);
        }
    }

    #endregion #monobehaviour

    private void CreateAgentExecutionStates() {
        _agentExecutionStates = new Dictionary<string, IState>() {
            [AgentConstants.IDLE] = gameObject.AddComponent<StateIdle>().SetValues(AgentConstants.IDLE, this, _fsm),
            [AgentConstants.PURSUIT] = new StatePursuit(AgentConstants.PURSUIT, this, _fsm),
            [AgentConstants.EVADE] = new StateEvade(AgentConstants.EVADE, this, _fsm),
            [AgentConstants.KILL] = new StateKill(AgentConstants.KILL, this, _fsm),
            [AgentConstants.DEAD] = new StateDead(AgentConstants.DEAD, this, _fsm),
            [AgentConstants.GO_TO] = new StateGoTo(AgentConstants.GO_TO, this, _fsm),
            [AgentConstants.PICK_UP] = new StatePickUp(AgentConstants.PICK_UP, this, _fsm),
            // [AgentConstants.PLAN_STEP] = new StatePlanStep(AgentConstants.PLAN_STEP, this, _fsm),
            // [AgentConstants.PLAN_FAIL] = new StatePlanFail(AgentConstants.PLAN_FAIL, this, _fsm),
            // [AgentConstants.PLAN_SUCCESS] = new StatePlanSuccess(AgentConstants.PLAN_SUCCESS, this, _fsm)
        };
    }

    private void FSMStates() {
        var idle = new State<string>(AgentConstants.IDLE);
        var pursuit = new State<string>(AgentConstants.PURSUIT);
        var evade = new State<string>(AgentConstants.EVADE);
        var kill = new State<string>(AgentConstants.KILL);
        var dead = new State<string>(AgentConstants.DEAD);
        var goTo = new State<string>(AgentConstants.GO_TO);
        var pickUp = new State<string>(AgentConstants.PICK_UP);
        var planStep = new State<string>(AgentConstants.PLAN_STEP);
        var planFail = new State<string>(AgentConstants.PLAN_FAIL);
        var planSuccess = new State<string>(AgentConstants.PLAN_SUCCESS);
        
        _agentStates = new Dictionary<string, State<string>>() {
            [AgentConstants.IDLE] = idle,
            [AgentConstants.PURSUIT] = pursuit,
            [AgentConstants.EVADE] = evade,
            [AgentConstants.KILL] = kill,
            [AgentConstants.DEAD] = dead,
            [AgentConstants.PICK_UP] = pickUp,
            [AgentConstants.GO_TO] = goTo,
            [AgentConstants.PLAN_STEP] = planStep,
            [AgentConstants.PLAN_FAIL] = planFail,
            [AgentConstants.PLAN_SUCCESS] = planSuccess
        };
        
        SetAllTransitions(idle);
        SetAllTransitions(pursuit);
        SetAllTransitions(evade);
        SetAllTransitions(kill);
        SetAllTransitions(dead);
        SetAllTransitions(pickUp);
        SetAllTransitions(goTo);
        SetAllTransitions(planStep);
        SetAllTransitions(planFail);
        SetAllTransitions(planSuccess);
    }

    private void SetAllTransitions(State<string> state) {
        foreach (var keyValue in _agentStates) {
            state.SetTransition(keyValue.Key, keyValue.Value);
        }
    }

    private void SubscribeAgentExecutionStates() {
        AddExecutionsByAction(AgentConstants.IDLE, _agentExecutionStates[AgentConstants.IDLE]);
        AddExecutionsByAction(AgentConstants.PURSUIT, _agentExecutionStates[AgentConstants.PURSUIT]);
        AddExecutionsByAction(AgentConstants.EVADE, _agentExecutionStates[AgentConstants.EVADE]);
        AddExecutionsByAction(AgentConstants.KILL, _agentExecutionStates[AgentConstants.KILL]);
        AddExecutionsByAction(AgentConstants.DEAD, _agentExecutionStates[AgentConstants.DEAD]);
        AddExecutionsByAction(AgentConstants.DEAD, _agentExecutionStates[AgentConstants.DEAD]);
        // AddExecutionsByAction(AgentConstants.PLAN_FAIL, _agentExecutionStates[AgentConstants.PLAN_FAIL]);
        // AddExecutionsByAction(AgentConstants.PLAN_STEP, _agentExecutionStates[AgentConstants.PLAN_STEP]);
        // AddExecutionsByAction(AgentConstants.DEAD, _agentExecutionStates[AgentConstants.SUCCESS]);
        _agentStates[AgentConstants.PLAN_STEP].OnEnter += () => {
            Debug.Log("Plan next step");
            var step = _plan.FirstOrDefault();
            if(step != null) {
                Debug.Log("Next step:" + step.Item1 + "," + step.Item2, "YELLOW");

                _plan = _plan.Skip(1);
                var oldTarget = target;
                target = step.Item2;
                if(!_fsm.Feed(step.Item1))
                    target = oldTarget;
                return;
            }
            _fsm.Feed(AgentConstants.PLAN_SUCCESS);
        };
        _agentStates[AgentConstants.PLAN_SUCCESS].OnEnter += () => {
            Debug.LogColor("Plan FAIL", "RED");
        };
        _agentStates[AgentConstants.PLAN_SUCCESS].OnEnter += () => {
            Debug.LogColor("Plan SUCCESS", "GREEN");
        };
    }

    public void AddExecutionsByAction(string actionKey, IState actionExecutions) {
        _agentStates[actionKey].OnEnter += actionExecutions.OnEnter;
        _agentStates[actionKey].OnUpdate += actionExecutions.OnUpdate;
        _agentStates[actionKey].OnExit += actionExecutions.OnExit;
    }
}