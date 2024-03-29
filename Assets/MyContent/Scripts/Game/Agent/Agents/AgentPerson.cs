﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FP;
using FSM;
using Items;
using MyContent.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Item))]
public class AgentPerson : BaseAgent{
    public enum PersonActions{
        None = 0,
        Idle,
        ReportSicario,
        Dead
    }

    public Action<BaseAgent> randomWalk = ThinkingAgent.RandomWalk;
    private Coroutine _coroutine;
    private EventFSM<PersonActions> _fsm;

    private void Start() {
        base.Start();
        _meshRenderer.material.color = Consts.AGENT_PERSON_COLOR;
        gameObject.name = "Person";
        var item = GetComponent<Item>();
        item.type = ItemConstants.ITEM_TYPE_MONEY;

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
    
    public void WalkForward() {
        if (_terrainChecker.isTerrain) {
            return;
        }

        transform.position += transform.forward * Time.deltaTime * MovementSpeed;
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

    protected override IEnumerable<GOAPActionDelegate> goapPlan {
        set => throw new System.NotImplementedException();
    }

    protected override void ExecutePlan(List<Tuple<string, Item>> plan) {
        throw new System.NotImplementedException();
    }
}