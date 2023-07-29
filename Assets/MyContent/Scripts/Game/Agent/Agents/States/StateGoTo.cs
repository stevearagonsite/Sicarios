using System;
using FP;
using FSM;

public class StateGoTo<AgentActions>: IState {
    public readonly string name;
    private BaseAgent _agent;
    private EventFSM<AgentActions> _fsm;
    
    public StateGoTo(string name, BaseAgent agent, EventFSM<AgentActions> fsm) {
        this.name = name;
        _agent = agent;
        _fsm = fsm;
    }
    
    public void OnEnter() {
        throw new NotImplementedException();
    }
    
    public void OnUpdate() {
        throw new NotImplementedException();
    }
    
    public void OnExit() {
        throw new NotImplementedException();
    }
}