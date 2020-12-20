using System.Collections;
using FP;
using MyContent.Scripts;
using UnityEngine;

public class AgentDetective: BaseAgent{
    public ThinkDelegateDetective randomWalk = ThinkingDetective.RandomWalk;
    private Coroutine _coroutine;

    private void Start() {
        _meshRenderer.material.color = Consts.AGENT_DETECTIVE_COLOR;

        var idle = new State<DetectiveActions>("Idle");
        var pursuit = new State<DetectiveActions>("Pursuit");
        var patrol = new State<DetectiveActions>("Patrol");
        var search = new State<DetectiveActions>("Search");
        var kill = new State<DetectiveActions>("Kill");

        kill.SetTransition(DetectiveActions.Kill, kill);
    }
}

public enum DetectiveActions{
    None,
    Idle,
    Pursuit,
    Kill,
}