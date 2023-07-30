using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSM;
using UnityEngine;
using AgentConstants = AgentSicarioConstants.AgentSicarioStates;
using Debug = Logger.Debug;

public partial class AgentSicario {
    private Dictionary<string, IState> _agentExecutionStates;
    private Dictionary<string, State<string>> _agentStates;

    #region monobehaviour

    private void Start() {
        CreateAgentExecutionStates();
        FSMStates();

        CreatePlan(out var actions, out var from, out var to);
        StartCoroutine(GoapRunDelegate(from, to, actions));
        _fsm = new EventFSM<string>(_agentStates[AgentConstants.IDLE]);
        _fsm.Feed(AgentConstants.IDLE);
        // StartCoroutine(SicarioDecisions(1f));
    }

    private void Update() {
        _fsm.Update();
    }

    // public virtual IEnumerator SicarioDecisions(float time) {
    //     while (true) {
    //         _fsm.Feed(AgentConstants.IDLE);
    //         if (_goapPlan != null) {
    //             GOAPActionDelegate action = _goapPlan.First();
    //         }
    //
    //         yield return new WaitForSeconds(time);
    //     }
    // }

    #endregion #monobehaviour

    private void CreateAgentExecutionStates() {
        _agentExecutionStates = new Dictionary<string, IState>() {
            [AgentConstants.IDLE] = gameObject.AddComponent<StateIdle>().SetValues(AgentConstants.IDLE, this),
            [AgentConstants.PURSUIT] = new StatePursuit(AgentConstants.PURSUIT, this),
            [AgentConstants.EVADE] = new StateEvade(AgentConstants.EVADE, this),
            [AgentConstants.KILL] = new StateKill(AgentConstants.KILL, this),
            [AgentConstants.DEAD] = new StateDead(AgentConstants.DEAD, this),
            [AgentConstants.GO_TO] =  gameObject.AddComponent<StateGoTo>().SetValues(AgentConstants.GO_TO, this),
            [AgentConstants.PICK_UP] = new StatePickUp(AgentConstants.PICK_UP, this),
            // [AgentConstants.PLAN_STEP] = null,
            // [AgentConstants.PLAN_FAIL] = null,
            // [AgentConstants.PLAN_SUCCESS] = null,
            [AgentConstants.PLAN_STEP] = new StatePlanStep(AgentConstants.PLAN_STEP, this),
            [AgentConstants.PLAN_FAIL] = new StatePlanFail(AgentConstants.PLAN_FAIL, this),
            [AgentConstants.PLAN_SUCCESS] = new StatePlanSuccess(AgentConstants.PLAN_SUCCESS, this)
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
        
        idle = AddExecutionsByState(idle, _agentExecutionStates[AgentConstants.IDLE]);
        pursuit = AddExecutionsByState(pursuit, _agentExecutionStates[AgentConstants.PURSUIT]);
        evade = AddExecutionsByState(evade, _agentExecutionStates[AgentConstants.EVADE]);
        kill = AddExecutionsByState(kill, _agentExecutionStates[AgentConstants.KILL]);
        dead = AddExecutionsByState(dead, _agentExecutionStates[AgentConstants.DEAD]);
        goTo = AddExecutionsByState(goTo, _agentExecutionStates[AgentConstants.GO_TO]);
        pickUp = AddExecutionsByState(pickUp, _agentExecutionStates[AgentConstants.PICK_UP]);
        planStep = AddExecutionsByState(planStep, _agentExecutionStates[AgentConstants.PLAN_STEP]);
        planFail = AddExecutionsByState(planFail, _agentExecutionStates[AgentConstants.PLAN_FAIL]);
        planSuccess = AddExecutionsByState(planSuccess, _agentExecutionStates[AgentConstants.PLAN_SUCCESS]);
        
        planStep.OnEnter += () => {

        };
        planSuccess.OnEnter += () => {
#if UNITY_EDITOR
            Debug.LogColor("Sicario State", "Plan SUCCESS", "GREEN");
#endif
        };
        idle.SetTransition(AgentConstants.PURSUIT, pursuit);
        idle.SetTransition(AgentConstants.EVADE, evade);
        idle.SetTransition(AgentConstants.KILL, kill);
        idle.SetTransition(AgentConstants.DEAD, dead);
        idle.SetTransition(AgentConstants.PICK_UP, pickUp);
        idle.SetTransition(AgentConstants.GO_TO, goTo);
        idle.SetTransition(AgentConstants.PLAN_STEP, planStep);
        idle.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        idle.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        pursuit.SetTransition(AgentConstants.IDLE, idle);
        pursuit.SetTransition(AgentConstants.EVADE, evade);
        pursuit.SetTransition(AgentConstants.KILL, kill);
        pursuit.SetTransition(AgentConstants.DEAD, dead);
        pursuit.SetTransition(AgentConstants.PICK_UP, pickUp);
        pursuit.SetTransition(AgentConstants.GO_TO, goTo);
        pursuit.SetTransition(AgentConstants.PLAN_STEP, planStep);
        pursuit.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        pursuit.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        evade.SetTransition(AgentConstants.IDLE, idle);
        evade.SetTransition(AgentConstants.PURSUIT, pursuit);
        evade.SetTransition(AgentConstants.KILL, kill);
        evade.SetTransition(AgentConstants.DEAD, dead);
        evade.SetTransition(AgentConstants.PICK_UP, pickUp);
        evade.SetTransition(AgentConstants.GO_TO, goTo);
        evade.SetTransition(AgentConstants.PLAN_STEP, planStep);
        evade.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        evade.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        kill.SetTransition(AgentConstants.IDLE, idle);
        kill.SetTransition(AgentConstants.PURSUIT, pursuit);
        kill.SetTransition(AgentConstants.EVADE, evade);
        kill.SetTransition(AgentConstants.DEAD, dead);
        kill.SetTransition(AgentConstants.PICK_UP, pickUp);
        kill.SetTransition(AgentConstants.GO_TO, goTo);
        kill.SetTransition(AgentConstants.PLAN_STEP, planStep);
        kill.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        kill.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        dead.SetTransition(AgentConstants.IDLE, idle);
        dead.SetTransition(AgentConstants.PURSUIT, pursuit);
        dead.SetTransition(AgentConstants.EVADE, evade);
        dead.SetTransition(AgentConstants.KILL, kill);
        dead.SetTransition(AgentConstants.PICK_UP, pickUp);
        dead.SetTransition(AgentConstants.GO_TO, goTo);
        dead.SetTransition(AgentConstants.PLAN_STEP, planStep);
        dead.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        dead.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        goTo.SetTransition(AgentConstants.IDLE, idle);
        goTo.SetTransition(AgentConstants.PURSUIT, pursuit);
        goTo.SetTransition(AgentConstants.EVADE, evade);
        goTo.SetTransition(AgentConstants.KILL, kill);
        goTo.SetTransition(AgentConstants.DEAD, dead);
        goTo.SetTransition(AgentConstants.PICK_UP, pickUp);
        goTo.SetTransition(AgentConstants.PLAN_STEP, planStep);
        goTo.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        goTo.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        pickUp.SetTransition(AgentConstants.IDLE, idle);
        pickUp.SetTransition(AgentConstants.PURSUIT, pursuit);
        pickUp.SetTransition(AgentConstants.EVADE, evade);
        pickUp.SetTransition(AgentConstants.KILL, kill);
        pickUp.SetTransition(AgentConstants.DEAD, dead);
        pickUp.SetTransition(AgentConstants.GO_TO, goTo);
        pickUp.SetTransition(AgentConstants.PLAN_STEP, planStep);
        pickUp.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        pickUp.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        planStep.SetTransition(AgentConstants.IDLE, idle);
        planStep.SetTransition(AgentConstants.PURSUIT, pursuit);
        planStep.SetTransition(AgentConstants.EVADE, evade);
        planStep.SetTransition(AgentConstants.KILL, kill);
        planStep.SetTransition(AgentConstants.DEAD, dead);
        planStep.SetTransition(AgentConstants.PICK_UP, pickUp);
        planStep.SetTransition(AgentConstants.GO_TO, goTo);
        planStep.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        planStep.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        planFail.SetTransition(AgentConstants.IDLE, idle);
        planFail.SetTransition(AgentConstants.PURSUIT, pursuit);
        planFail.SetTransition(AgentConstants.EVADE, evade);
        planFail.SetTransition(AgentConstants.KILL, kill);
        planFail.SetTransition(AgentConstants.DEAD, dead);
        planFail.SetTransition(AgentConstants.PICK_UP, pickUp);
        planFail.SetTransition(AgentConstants.GO_TO, goTo);
        planFail.SetTransition(AgentConstants.PLAN_STEP, planStep);
        planFail.SetTransition(AgentConstants.PLAN_SUCCESS, planSuccess);

        planSuccess.SetTransition(AgentConstants.IDLE, idle);
        planSuccess.SetTransition(AgentConstants.PURSUIT, pursuit);
        planSuccess.SetTransition(AgentConstants.EVADE, evade);
        planSuccess.SetTransition(AgentConstants.KILL, kill);
        planSuccess.SetTransition(AgentConstants.DEAD, dead);
        planSuccess.SetTransition(AgentConstants.PICK_UP, pickUp);
        planSuccess.SetTransition(AgentConstants.GO_TO, goTo);
        planSuccess.SetTransition(AgentConstants.PLAN_STEP, planStep);
        planSuccess.SetTransition(AgentConstants.PLAN_FAIL, planFail);
        
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
    }

    public State<string> AddExecutionsByState(
        State<string> state, 
        IState actionExecutions,
        StateLifeCycle[] excludeLifeCycle = null
        ) {
        
        if (excludeLifeCycle == null) {
#if UNITY_EDITOR
            Debug.LogColor("Agent State AddExecutionsByState", "ALL", "blue");
#endif
            state.OnEnter += actionExecutions.OnEnter;
            state.OnUpdate += actionExecutions.OnUpdate;
            state.OnExit += actionExecutions.OnExit;
            return state;
        }

        if (excludeLifeCycle.Any((lc) => lc == StateLifeCycle.ON_ENTER)) {
#if UNITY_EDITOR
            Debug.LogColor("Agent State AddExecutionsByState", "ON_ENTER", "blue");
#endif
            state.OnEnter += actionExecutions.OnEnter;
        }

        if (!excludeLifeCycle.Contains(StateLifeCycle.ON_UPDATE)) {
#if UNITY_EDITOR
            Debug.LogColor("Agent State AddExecutionsByState", "ON_UPDATE", "blue");
#endif
            state.OnUpdate += actionExecutions.OnUpdate;
        }

        if (!excludeLifeCycle.Contains(StateLifeCycle.ON_EXIT)) {
#if UNITY_EDITOR
            Debug.LogColor("Agent State AddExecutionsByState", "ON_EXIT", "blue");
#endif
            state.OnExit += actionExecutions.OnExit;
        }
        
        return state;
    }
}