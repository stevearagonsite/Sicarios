using System.Collections;
using FP;
using MyContent.Scripts;
using UnityEngine;

public class AgentPerson : BaseAgent{
    public enum PersonActions{
        None = 0,
        Idle,
        ReportSicario,
        Dead
    }

    public ThinkDelegateDetective randomWalk = ThinkingDetective.RandomWalk;
    private Coroutine _coroutine;
    private EventFSM<PersonActions> _fsm;

    private void Start() {
        _meshRenderer.material.color = Consts.AGENT_PERSON_COLOR;
        gameObject.name = "Person";

        var idle = new State<PersonActions>("Idle");
        var reportSicario = new State<PersonActions>("ReportSicario");
        var dead = new State<PersonActions>("Dead");

        idle.SetTransition(PersonActions.Idle, idle);
        idle.SetTransition(PersonActions.Dead, dead);
        idle.SetTransition(PersonActions.ReportSicario, reportSicario);

        reportSicario.SetTransition(PersonActions.ReportSicario, reportSicario);
        reportSicario.SetTransition(PersonActions.Dead, dead);
        reportSicario.SetTransition(PersonActions.Idle, idle);

        reportSicario.SetTransition(PersonActions.Idle, idle);
        reportSicario.SetTransition(PersonActions.Dead, dead);

        // Dead set
        idle.OnEnter += OnDead;
        idle.OnUpdate += Consts.NOOB;
        idle.OnExit += Consts.NOOB;

        // Idle set
        idle.OnEnter += OnRandomMove;
        idle.OnUpdate += WalkForward;
        idle.OnExit += OffRandomMove;

        // Report Sicario
        idle.OnEnter += Consts.NOOB;
        idle.OnUpdate += Consts.NOOB;
        idle.OnExit += Consts.NOOB;

        _fsm = new EventFSM<PersonActions>(idle);
        StartCoroutine(PersonDecisions(0.3f));
    }

    private void Update() {
        _fsm.Update();
    }

    public virtual IEnumerator PersonDecisions(float time) {
        while (true) {
            _fsm.Feed(PersonActions.Idle);
            yield return new WaitForSeconds(time);
        }
    }

    public void OnRandomMove() {
        _coroutine = StartCoroutine(RandomWalk());
    }

    public void OffRandomMove() {
        _coroutine = StartCoroutine(RandomWalk());
    }

    public virtual IEnumerator RandomWalk() {
        while (true) {
            var time = Random.Range(1f, 5f) * 10;
            randomWalk((BaseAgent) this);
            yield return new WaitForSeconds(time);
        }
    }
}