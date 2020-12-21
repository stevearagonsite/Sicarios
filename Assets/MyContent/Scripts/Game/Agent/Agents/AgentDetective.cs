using System.Collections;
using FP;
using MyContent.Scripts;
using UnityEngine;

public class AgentDetective: BaseAgent{
    public enum DetectiveActions{
        None,
        Idle,
        Pursuit,
        Patrol,
        Search,
        Kill,
    }
    public ThinkDelegateDetective randomWalk = ThinkingDetective.RandomWalk;
    private Coroutine _coroutine;
    private EventFSM<DetectiveActions> _fsm;
    private TerrainChecker _terrainChecker;
    private AgentSicario _target;
    private Vector3 _lastPositionTarget;

    private void Start() {
        _terrainChecker = GetComponentInChildren<TerrainChecker>();
        _meshRenderer.material.color = Consts.AGENT_DETECTIVE_COLOR;

        var idle = new State<DetectiveActions>("Idle");
        var pursuit = new State<DetectiveActions>("Pursuit");
        var patrol = new State<DetectiveActions>("Patrol");
        var search = new State<DetectiveActions>("Search");
        var kill = new State<DetectiveActions>("Kill");

        idle.SetTransition(DetectiveActions.Idle, idle);
        idle.SetTransition(DetectiveActions.Pursuit, pursuit);
        
        pursuit.SetTransition(DetectiveActions.Pursuit, pursuit);
        pursuit.SetTransition(DetectiveActions.Patrol, patrol);
        pursuit.SetTransition(DetectiveActions.Search, search);
        pursuit.SetTransition(DetectiveActions.Kill, kill);
        
        patrol.SetTransition(DetectiveActions.Patrol, patrol);
        patrol.SetTransition(DetectiveActions.Search, search);
        patrol.SetTransition(DetectiveActions.Pursuit, pursuit);
        
        search.SetTransition(DetectiveActions.Search, search);
        search.SetTransition(DetectiveActions.Pursuit, pursuit);
        search.SetTransition(DetectiveActions.Patrol, patrol);
        
        kill.SetTransition(DetectiveActions.Kill, kill);
        kill.SetTransition(DetectiveActions.Pursuit, pursuit);
        kill.SetTransition(DetectiveActions.Patrol, patrol);
        kill.SetTransition(DetectiveActions.Search, search);
        
        // Idle set
        idle.OnEnter += Consts.NOOB;
        idle.OnUpdate += Consts.NOOB;
        idle.OnExit += Consts.NOOB;
        
        // Report Sicario
        pursuit.OnEnter += Consts.NOOB;
        pursuit.OnUpdate += Consts.NOOB;
        pursuit.OnExit += Consts.NOOB;
        
        // Report Sicario
        patrol.OnEnter += Consts.NOOB;
        patrol.OnUpdate += Consts.NOOB;
        patrol.OnExit += Consts.NOOB;
        
        // Report Sicario
        search.OnEnter += Consts.NOOB;
        search.OnUpdate += Consts.NOOB;
        search.OnExit += Consts.NOOB;
        
        // Report Sicario
        kill.OnEnter += Consts.NOOB;
        kill.OnUpdate += Consts.NOOB;
        kill.OnExit += Consts.NOOB;
        
        _fsm = new EventFSM<DetectiveActions>(idle);
        StartCoroutine(DetectiveDecisions(0.3f));
    }
    
    private void Update()
    {
        _fsm.Update();
    }
    
    public virtual IEnumerator DetectiveDecisions(float time) {
        while (true) {
            _fsm.Feed(DetectiveActions.Idle);
            yield return new WaitForSeconds(time);
        }
    }
}