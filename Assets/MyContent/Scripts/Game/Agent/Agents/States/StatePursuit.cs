using System;
using FP;
using FSM;

public class StatePursuit: IState {
    public readonly string name;
    private BaseAgent _agent;
    public StatePursuit(string name, BaseAgent agent) {
        this.name = name;
        _agent = agent;
    }
    
    public void OnEnter() {
        // throw new NotImplementedException();
    }
    
    public void OnUpdate() {
        // throw new NotImplementedException();
    }
    
    public void OnExit() {
        // throw new NotImplementedException();
    }
}