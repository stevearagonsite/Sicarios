using System.Collections;
using MyContent.Scripts;
using FP;
using UnityEngine;

public class AgentSicario : BaseAgent{
    public ThinkDelegatePerson randomWalk = ThinkingPerson.RandomWalk;
    private Coroutine _coroutine;

    private void Start() {
        _meshRenderer.material.color = Consts.AGENT_SICARIO_COLOR;

        var idle = new State<SicarioActions>("Idle");
        var pursuit = new State<SicarioActions>("Pursuit");
        var evade = new State<SicarioActions>("Evade");
        var kill = new State<SicarioActions>("Kill");
        var steal = new State<SicarioActions>("Steal");

        pursuit.SetTransition(SicarioActions.Idle, idle);
        idle.SetTransition(SicarioActions.Pursuit, pursuit);
        evade.SetTransition(SicarioActions.Evade, evade);
        kill.SetTransition(SicarioActions.Kill, kill);
        steal.SetTransition(SicarioActions.Steal, steal);
        
        // Idle set
        idle.OnEnter += () => OnRandomMove();
        idle.OnUpdate += () => WalkForward();
        idle.OnExit += () => OffRandomMove();
    }
    
    public void OnRandomMove() { _coroutine = StartCoroutine(RandomWalk()); }
    public void OffRandomMove() { _coroutine = StartCoroutine(RandomWalk()); }
    public void WalkForward() { transform.position += transform.forward * Time.deltaTime * MovementSpeed; }


    public virtual IEnumerator RandomWalk() {
        while (true) {
            var time = Random.Range(1f, 5f);
            randomWalk((BaseAgent) this);
            yield return new WaitForSeconds(time);
        }
    }
}

public enum SicarioActions{
    None,
    Idle,
    Pursuit,
    Evade,
    Kill,
    Steal
}