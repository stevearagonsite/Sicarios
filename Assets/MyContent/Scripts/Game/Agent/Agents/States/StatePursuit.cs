using System;
using FP;
using FSM;

public class StatePursuit: IState {
    public readonly string name;
    private BaseAgent _agent;
    private EventFSM<string> _fsm;
    public StatePursuit(string name, BaseAgent agent, EventFSM<string> fsm) {
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