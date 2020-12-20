using System.Collections;
using System.Collections.Generic;
using FP;
using MyContent.Scripts;
using UnityEngine;

public class AgentPerson : BaseAgent{
    public ThinkDelegateDetective randomWalk = ThinkingDetective.RandomWalk;
    private Coroutine _coroutine;

    private void Start() {
        _meshRenderer.material.color = Consts.AGENT_PERSON_COLOR;

        var idle = new State<PersonActions>("Idle");
        var pursuit = new State<PersonActions>("Pursuit");
        var patrol = new State<PersonActions>("Patrol");
        var search = new State<PersonActions>("Search");
        var kill = new State<PersonActions>("Kill");

        kill.SetTransition(PersonActions.Kill, kill);
    }
}

public enum PersonActions{
    None,
    Idle,
    Pursuit,
    Evade,
    Kill,
    Steal
}
