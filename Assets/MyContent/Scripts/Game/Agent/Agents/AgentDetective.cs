﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSM;
using Items;
using MyContent.Scripts;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class AgentDetective : BaseAgent{
    public enum DetectiveActions{
        None = 0,
        Pursuit,
        Patrol,
        Search,
        Kill,
    }

    public Action<BaseAgent> randomWalk = ThinkingAgent.RandomWalk;
    Stack<Waypoint> _seach = new Stack<Waypoint>();
    private Coroutine _coroutine;
    private EventFSM<DetectiveActions> _fsm;
    private AgentSicario _target;
    private Vector3 _lastPositionTarget;
    private Waypoint _initialPatrol;
    private Waypoint _toVisit;

    private void Start() {
        base.Start();
        _meshRenderer.material.color = Consts.AGENT_DETECTIVE_COLOR;
        gameObject.name = "Detective";
        var item = GetComponent<Item>();
        item.type = ItemConstants.ITEM_TYPE_GUARD;

        var pursuit = new State<DetectiveActions>("Pursuit");
        var patrol = new State<DetectiveActions>("Patrol");
        var search = new State<DetectiveActions>("Search");
        var kill = new State<DetectiveActions>("Kill");

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

        // Report Sicario
        pursuit.OnEnter += Consts.NOOB;
        pursuit.OnUpdate += Consts.NOOB;
        pursuit.OnExit += Consts.NOOB;

        // Report Sicario
        patrol.OnEnter += EnterPatrol;
        patrol.OnUpdate += UpdatePatrol;
        patrol.OnExit += Consts.NOOB;

        // Report Sicario
        search.OnEnter += Consts.NOOB;
        search.OnUpdate += Consts.NOOB;
        search.OnExit += Consts.NOOB;

        // Report Sicario
        kill.OnEnter += Consts.NOOB;
        kill.OnUpdate += Consts.NOOB;
        kill.OnExit += Consts.NOOB;

        _fsm = new EventFSM<DetectiveActions>(patrol);
        StartCoroutine(DetectiveDecisions(0.3f));
    }

    private void Update() {
        _fsm.Update();
    }

    private void EnterPatrol() {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2);
        
        foreach (var hitCollider in hitColliders) {
            var waypoint = hitCollider.GetComponent<Waypoint>();
            if (waypoint) {
                Debug.Log("initial waypoint");
                this._initialPatrol = waypoint;
                return;
            }
        }
        
        // _initialPatrol = radiusQuerier.Query()
        //     .Where(e => e.GetComponent<Waypoint>())
        //     .Select(e => e.GetComponent<Waypoint>())
        //     .OrderBy(w => Vector3.Distance(transform.position, w.transform.position))
        //     .FirstOrDefault();
    }

    private void UpdatePatrol() {
        // transform.position += transform.forward * Time.deltaTime * MovementSpeed;
        // var vector3Direction = transform.position - _toVisit.transform.position;
        // transform.eulerAngles = Vector3.Normalize(vector3Direction);
    }


    public virtual IEnumerator DetectiveDecisions(float time) {
        while (true) {
            
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