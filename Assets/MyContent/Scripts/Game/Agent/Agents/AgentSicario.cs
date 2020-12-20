using System.Collections;
using MyContent.Scripts;
using FP;
using UnityEngine;

public class AgentSicario : BaseAgent{
    public enum SicarioActions{
        None = 0,
        Idle,
        Pursuit,
        Evade,
        Kill
    }
    EventFSM<SicarioActions> _fsm;
    
    public ThinkDelegatePerson randomWalk = ThinkingPerson.RandomWalk;
    private Coroutine _coroutine;

    private void Start() {
        _meshRenderer.material.color = Consts.AGENT_SICARIO_COLOR;

        var idle = new State<SicarioActions>("Idle");
        var pursuit = new State<SicarioActions>("Pursuit");
        var evade = new State<SicarioActions>("Evade");
        var kill = new State<SicarioActions>("Kill");

        pursuit.SetTransition(SicarioActions.Idle, idle);
        idle.SetTransition(SicarioActions.Pursuit, pursuit);
        evade.SetTransition(SicarioActions.Evade, evade);
        kill.SetTransition(SicarioActions.Kill, kill);
        
        // Idle set
        idle.OnEnter += () => OnRandomMove();
        idle.OnUpdate += () => WalkForward();
        idle.OnExit += () => OffRandomMove();
        
        // Report Sicario
        idle.OnEnter += Consts.NOOB;
        idle.OnUpdate += Consts.NOOB;
        idle.OnExit += Consts.NOOB;
        
        // Report Sicario
        idle.OnEnter += Consts.NOOB;
        idle.OnUpdate += Consts.NOOB;
        idle.OnExit += Consts.NOOB;
        
        // Report Sicario
        idle.OnEnter += Consts.NOOB;
        idle.OnUpdate += Consts.NOOB;
        idle.OnExit += Consts.NOOB;
        
        // Report Sicario
        idle.OnEnter += Consts.NOOB;
        idle.OnUpdate += Consts.NOOB;
        idle.OnExit += Consts.NOOB;
        
        
        _fsm = new EventFSM<SicarioActions>(idle);
    }
    
    private void Update()
    {
        _fsm.Update();
    }
    
    public virtual IEnumerator PersonDecisions(float time) {
        while (true) {
            _fsm.Feed(SicarioActions.Idle);
            yield return new WaitForSeconds(time);
        }
    }

    public void OnRandomMove() { _coroutine = StartCoroutine(RandomWalk()); }
    public void OffRandomMove() { _coroutine = StartCoroutine(RandomWalk()); }

    public void WalkForward() {
        transform.position += transform.forward * Time.deltaTime * MovementSpeed;
    }
    
    public virtual IEnumerator RandomWalk() {
        while (true) {
            var time = Random.Range(1f, 5f);
            randomWalk((BaseAgent) this);
            yield return new WaitForSeconds(time);
        }
    }
}
