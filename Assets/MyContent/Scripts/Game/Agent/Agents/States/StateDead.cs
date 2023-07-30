using System;
using FSM;

public class StateDead: IState {
    public readonly string name;
    private BaseAgent _agent;

    public StateDead(string name, BaseAgent agent) {
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