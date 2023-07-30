using System;
using System.Linq;
using FP;
using FSM;
using Debug = Logger.Debug;

public class StatePlanStep: IState {
    public readonly string name;
    private BaseAgent _agent;
    
    public StatePlanStep(string name, BaseAgent agent) {
        this.name = name;
        _agent = agent;
    }
    
    public void OnEnter() {
#if UNITY_EDITOR
        Debug.Log(this, "Plan next step");
        Debug.LogColor(this, "current state: " + _agent.fsm.current.name, "yellow");
#endif
        var step = _agent.plan.FirstOrDefault();
        if (step != null) {
#if UNITY_EDITOR
            Debug.LogColor(this, "Next step: " + step.Item1 + ", " + step.Item2.type, "YELLOW");
#endif
            _agent.plan = _agent.plan.Skip(1);
            var oldTarget = _agent.target;
            _agent.target = step.Item2;
            if (!_agent.fsm.Feed(step.Item1))
                _agent.target = oldTarget;
            return;
        }

        _agent.fsm.Feed("Success");
    }
    
    public void OnUpdate() {
        // throw new NotImplementedException();
    }
    
    public void OnExit() {
        // throw new NotImplementedException();
    }
}