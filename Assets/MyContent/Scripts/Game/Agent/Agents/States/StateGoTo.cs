using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FP;
using FSM;
using Items;
using UnityEngine;

public class StateGoTo : MonoBehaviour, IState {
    private string _name;
    private BaseAgent _agent;
    private Coroutine _coroutine;
    Vector3 _vel;

    Waypoint _gizmoRealTarget;
    IEnumerable<Waypoint> _gizmoPath;
    public event Action<BaseAgent, Waypoint, bool> OnReachDestination = delegate { };

    public string name => _name;

    public StateGoTo SetValues(string name, BaseAgent agent) {
        this._name = name;
        _agent = agent;
        OnReachDestination += (a, w, b) => {
            if (b) {
                throw new NotImplementedException();
                // _agent.fsm.Feed(AgentSicarioStates.PickUp);
            }
        };

        return this;
    }

    public void OnEnter() {
        OnNavigateMove();
    }

    public void OnUpdate() {
        // throw new NotImplementedException();
    }

    public void OnExit() {
        OffNavigateMove();
    }

    private void OnNavigateMove() {
        _coroutine = StartCoroutine(Navigate(_agent.target));
    }

    private void OffNavigateMove() {
        StopCoroutine(_coroutine);
    }

    Vector3 FloorPos(MonoBehaviour b) {
        return FloorPos(b.transform.position);
    }

    Vector3 FloorPos(Vector3 v) {
        return new Vector3(v.x, 0f, v.z);
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
                Debug.Log("COUNT" + path.Count());
                foreach (var next in path.Select(w => FloorPos(w))) {
                    Debug.Log("NEXT " + next.ToString());

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

        if (reachedDst == dstWp) {
            _vel = (FloorPos(destination) - FloorPos(this)).normalized;
            yield return new WaitUntil(() => (FloorPos(destination) - FloorPos(this)).sqrMagnitude < 0.05f);
        }

        _vel = Vector3.zero;
        OnReachDestination(_agent, reachedDst, reachedDst == dstWp);
    }

    void OnDrawGizmos() {
        if (_gizmoPath == null)
            return;

        Gizmos.color = Color.magenta;
        var points = _gizmoPath.Select(w => FloorPos(w));
        Vector3 last = points.First();
        foreach (var p in points.Skip(1)) {
            Gizmos.DrawLine(p + Vector3.up, last + Vector3.up);
            last = p;
        }

        if (_gizmoRealTarget != null)
            Gizmos.DrawCube(_gizmoRealTarget.transform.position + Vector3.up * 1f, Vector3.one * 0.3f);
    }
}