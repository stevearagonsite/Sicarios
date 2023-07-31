using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FP;
using FSM;
using Items;
using UnityEngine;
using Debug = Logger.Debug;
using AgentSicarioStates = AgentSicarioConstants.AgentSicarioStates;

[RequireComponent(typeof(Rigidbody))]
public class StateKill : MonoBehaviour, IState {
    private string _name;
    private BaseAgent _agent;
    private Coroutine _coroutine;
    Vector3 _vel;
    private Rigidbody _rb;

    private Waypoint _gizmoRealTarget;
    private IEnumerable<Waypoint> _gizmoPath;

    public event Action<BaseAgent, Waypoint, bool> OnScopeDestination = delegate { };

    public void Start() {
        _rb = GetComponent<Rigidbody>();
        OnScopeDestination += (a, w, b) => {
#if UNITY_EDITOR
            Debug.LogColor(this, $"OnReachDestination: {w.name} {b}", "yellow");
#endif
            if (!b) return;
            _agent.target.OnInventoryAdd(_agent);
            _agent.Feed(AgentSicarioStates.PLAN_STEP);
        };
    }

    public StateKill(string name, BaseAgent agent) {
        this.name = name;
        _agent = agent;
    }
    
    public IState SetValues(string name, BaseAgent agent) {
#if UNITY_EDITOR
        Debug.LogColor(this, $"SetValues: {name}", "yellow");
#endif
        _name = name;
        _agent = agent;

        return this;
    }

    public void OnEnter() {
#if UNITY_EDITOR
        Debug.LogColor(this, "OnEnter", "yellow");
#endif
        OnGetTarget();
    }

    public void OnUpdate() {
        _rb.MovePosition(transform.position + Time.fixedDeltaTime * _vel * 10);

        if (_vel != Vector3.zero) {
            Quaternion rotation = Quaternion.LookRotation(_vel);
            _rb.rotation = Quaternion.Slerp(_rb.rotation, rotation, Time.fixedDeltaTime * 5);
        }
    }

    public void OnExit() {
#if UNITY_EDITOR
        Debug.LogColor(this, "OnExit", "yellow");
#endif
        OffGetTarget();
    }

    Vector3 FloorPos(MonoBehaviour b) {
        return FloorPos(b.transform.position);
    }

    Vector3 FloorPos(Vector3 v) {
        return new Vector3(v.x, 0f, v.z);
    }

    private void OnGetTarget() {
        _coroutine = StartCoroutine(Navigate(_agent.target));
    }

    private void OffGetTarget() {
        if (_coroutine == null) return; 
        StopCoroutine(_coroutine);
    }

    private IEnumerator Navigate(Item destination) {
        var srcWp = Navigation.instance.NearestTo(transform.position);
        var dstWp = Navigation.instance.NearestTo(destination.transform.position);

        _gizmoRealTarget = dstWp;
        Waypoint reachedDst = srcWp;

        if (srcWp != dstWp) {
            var path = _gizmoPath = AStarNormal<Waypoint>.Run(
                srcWp
                , dstWp
                , (wa, wb) => Vector3.Distance(wa.transform.position, wb.transform.position)
                , w => w == dstWp
                , w =>
                    //w.nearbyItems.Any(it => it.type == ItemType.Door)
                    //? null
                    //:
                    w.adyacent
                        //.Where(a => a.nearbyItems.All(it => it.type != ItemType.Door))
                        .Select(a =>
                            new AStarNormal<Waypoint>.Arc(a,
                                Vector3.Distance(a.transform.position, w.transform.position)))
            );
            if (path != null) {
                Debug.Log(this, "COUNT" + path.Count());
                foreach (var next in path.Select(w => FloorPos(w))) {
                    Debug.Log(this, "NEXT " + next.ToString());

                    while ((next - FloorPos(this)).sqrMagnitude >= 0.05f) {
                        _vel = (next - FloorPos(this)).normalized;
                        yield return null;
                    }
                    //_vel = (next - FloorPos(this)).normalized;
                    //yield return new WaitUntil(() => (next - FloorPos(this)).sqrMagnitude < 0.05f);
                }
            }

            reachedDst = path.Last();
        }

        var distance = Vector3.Distance(reachedDst.transform.position, dstWp.transform.position);
        if (distance > 0.05f) {
            _vel = (FloorPos(destination) - FloorPos(this)).normalized;
            yield return new WaitUntil(() => (FloorPos(destination) - FloorPos(this)).sqrMagnitude < 0.05f);
        }

        _vel = Vector3.zero;
        OnScopeDestination(_agent, reachedDst, distance <= 0.05f);
    }


    void OnDrawGizmos() {
        if (_gizmoPath == null)
            return;

        Gizmos.color = Color.yellow;
        var points = _gizmoPath.Select(w => FloorPos(w));
        Vector3 last = points.First();
        foreach (var p in points.Skip(1)) {
            Gizmos.DrawLine(p + Vector3.up * 2f, last + Vector3.up * 2f);
            last = p;
        }

        if (_gizmoRealTarget != null)
            Gizmos.DrawCube(_gizmoRealTarget.transform.position + Vector3.up * 1f, Vector3.one * 0.3f);
    }
}