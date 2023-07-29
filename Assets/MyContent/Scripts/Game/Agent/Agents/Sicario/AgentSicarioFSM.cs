using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyContent.Scripts;
using FP;
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

        _fsm = new EventFSM<string>(_agentStates[AgentConstants.IDLE]);
        _fsm.Feed(AgentConstants.IDLE);
    }
    
    private void Update() {
        _fsm.Update();
    }

    #endregion     #monobehaviour

    private void CreateAgentExecutionStates() {
        _agentExecutionStates = new Dictionary<string, IState>() {
            [AgentConstants.IDLE] = gameObject.AddComponent<StateIdle>().SetValues(AgentConstants.IDLE, this, _fsm),
            [AgentConstants.PURSUIT] = new StatePursuit(AgentConstants.PURSUIT, this, _fsm),
            [AgentConstants.EVADE] = new StateEvade(AgentConstants.EVADE, this, _fsm),
            [AgentConstants.KILL] = new StateKill(AgentConstants.KILL, this, _fsm),
            [AgentConstants.DEAD] = new StateDead(AgentConstants.DEAD, this, _fsm),
            // [AgentConstants.PLANSTEP] = new StatePlanStep("PlanStep", this, _fsm),
            // [AgentConstants.PLANFAIL] = new StatePlanFail("PlanFail", this, _fsm)
        };
    }

    private void FSMStates() {
        var idle = new State<string>(AgentConstants.IDLE);
        var pursuit = new State<string>(AgentConstants.PURSUIT);
        var evade = new State<string>(AgentConstants.EVADE);
        var kill = new State<string>(AgentConstants.KILL);
        var dead = new State<string>(AgentConstants.DEAD);

        // Idle set
        idle.SetTransition(AgentConstants.IDLE, idle);
        idle.SetTransition(AgentConstants.PURSUIT, pursuit);
        idle.SetTransition(AgentConstants.KILL, kill);
        idle.SetTransition(AgentConstants.EVADE, evade);
        idle.SetTransition(AgentConstants.DEAD, dead);

        // Pursuit set
        pursuit.SetTransition(AgentConstants.PURSUIT, pursuit);
        pursuit.SetTransition(AgentConstants.EVADE, evade);
        pursuit.SetTransition(AgentConstants.DEAD, dead);
        pursuit.SetTransition(AgentConstants.KILL, kill);
        pursuit.SetTransition(AgentConstants.IDLE, idle);

        // Evade set
        evade.SetTransition(AgentConstants.EVADE, evade);
        evade.SetTransition(AgentConstants.IDLE, idle);
        evade.SetTransition(AgentConstants.PURSUIT, pursuit);
        evade.SetTransition(AgentConstants.KILL, kill);
        evade.SetTransition(AgentConstants.DEAD, dead);

        // Kill set
        kill.SetTransition(AgentConstants.KILL, kill);
        kill.SetTransition(AgentConstants.EVADE, evade);
        kill.SetTransition(AgentConstants.DEAD, dead);
        kill.SetTransition(AgentConstants.PURSUIT, pursuit);
        kill.SetTransition(AgentConstants.DEAD, dead);

        _agentStates = new Dictionary<string, State<string>>() {
            [AgentConstants.IDLE] = idle,
            [AgentConstants.PURSUIT] = pursuit,
            [AgentConstants.EVADE] = evade,
            [AgentConstants.KILL] = kill,
            [AgentConstants.DEAD] = dead
        };
    }

    private void SubscribeAgentExecutionStates() {
        AddExecutionsByAction(AgentConstants.IDLE, _agentExecutionStates[AgentConstants.IDLE]);
        AddExecutionsByAction(AgentConstants.PURSUIT, _agentExecutionStates[AgentConstants.PURSUIT]);
        AddExecutionsByAction(AgentConstants.EVADE, _agentExecutionStates[AgentConstants.EVADE]);
        AddExecutionsByAction(AgentConstants.KILL, _agentExecutionStates[AgentConstants.KILL]);
        AddExecutionsByAction(AgentConstants.DEAD, _agentExecutionStates[AgentConstants.DEAD]);
        // AddExecutionsByAction(AgentConstants.PLAN_FAIL, _agentExecutionStates[AgentConstants.PLAN_FAIL]);
        // AddExecutionsByAction(AgentConstants.PLAN_STEP, _agentExecutionStates[AgentConstants.PLAN_STEP]);
    }

    private void AddExecutionsByAction(string actionKey, IState actionExecutions) {
        
        _agentStates[actionKey].OnEnter += actionExecutions.OnEnter;
        _agentStates[actionKey].OnUpdate += actionExecutions.OnUpdate;
        _agentStates[actionKey].OnExit += actionExecutions.OnExit;
    }
}