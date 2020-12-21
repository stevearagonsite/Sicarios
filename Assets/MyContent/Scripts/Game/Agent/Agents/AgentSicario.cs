using System.Collections;
using MyContent.Scripts;
using FP;
using UnityEngine;

[RequireComponent(typeof(LineOfSight))]
public class AgentSicario : BaseAgent{
    public enum SicarioActions{
        None = 0,
        Idle,
        Pursuit,
        Evade,
        Kill,
        Dead
    }
    private EventFSM<SicarioActions> _fsm;
    
    public ThinkDelegatePerson randomWalk = ThinkingPerson.RandomWalk;
    private Coroutine _coroutine;

    private void Start() {
        _meshRenderer.material.color = Consts.AGENT_SICARIO_COLOR;

        var idle = new State<SicarioActions>("Idle");
        var pursuit = new State<SicarioActions>("Pursuit");
        var evade = new State<SicarioActions>("Evade");
        var kill = new State<SicarioActions>("Kill");
        var dead = new State<SicarioActions>("Dead");

        pursuit.SetTransition(SicarioActions.Pursuit, pursuit);
        pursuit.SetTransition(SicarioActions.Evade, evade);
        pursuit.SetTransition(SicarioActions.Dead, dead);
        pursuit.SetTransition(SicarioActions.Kill, kill);
        pursuit.SetTransition(SicarioActions.Idle, idle);

        idle.SetTransition(SicarioActions.Idle, idle);
        idle.SetTransition(SicarioActions.Pursuit, pursuit);
        idle.SetTransition(SicarioActions.Kill, kill);
        idle.SetTransition(SicarioActions.Evade, evade);
        idle.SetTransition(SicarioActions.Dead, dead);
        
        evade.SetTransition(SicarioActions.Evade, evade);
        evade.SetTransition(SicarioActions.Idle, idle);
        evade.SetTransition(SicarioActions.Pursuit, pursuit);
        evade.SetTransition(SicarioActions.Kill, kill);
        evade.SetTransition(SicarioActions.Dead, dead);
        
        kill.SetTransition(SicarioActions.Kill, kill);
        kill.SetTransition(SicarioActions.Evade, evade);
        kill.SetTransition(SicarioActions.Dead, dead);
        kill.SetTransition(SicarioActions.Pursuit, pursuit);
        kill.SetTransition(SicarioActions.Dead, dead);
        
        // Idle set
        idle.OnEnter += OnRandomMove;
        idle.OnUpdate += WalkForward;
        idle.OnExit += OffRandomMove;
        
        // Pursuit set
        pursuit.OnEnter += Consts.NOOB;
        pursuit.OnUpdate += Consts.NOOB;
        pursuit.OnExit += Consts.NOOB;
        
        // Evade set
        evade.OnEnter += Consts.NOOB;
        evade.OnUpdate += Consts.NOOB;
        evade.OnExit += Consts.NOOB;
        
        // Kill set
        kill.OnEnter += Consts.NOOB;
        kill.OnUpdate += Consts.NOOB;
        kill.OnExit += Consts.NOOB;
        
        // Dead set
        dead.OnEnter += Consts.NOOB;
        dead.OnUpdate += Consts.NOOB;
        dead.OnExit += Consts.NOOB;

        _fsm = new EventFSM<SicarioActions>(idle);
        StartCoroutine(SicarioDecisions(0.3f));
    }
    
    private void Update()
    {
        _fsm.Update();
    }
    
    public virtual IEnumerator SicarioDecisions(float time) {
        while (true) {
            _fsm.Feed(SicarioActions.Idle);
            yield return new WaitForSeconds(time);
        }
    }

    public void OnRandomMove() { _coroutine = StartCoroutine(RandomWalk()); }
    public void OffRandomMove() { _coroutine = StartCoroutine(RandomWalk()); }

    public virtual IEnumerator RandomWalk() {
        while (true) {
            var time = Random.Range(1f, 5f);
            randomWalk((BaseAgent) this);
            yield return new WaitForSeconds(time);
        }
    }
}
