using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MyContent.Scripts;
using UnityEngine;
using Debug = Logger.Debug;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GridEntity))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class BaseAgent : MonoBehaviour {
    protected TerrainChecker _terrainChecker;
    protected MeshRenderer _meshRenderer;
    protected float _life = 100;
    public CircleQuerier radiusQuerier;
    public GridEntity _gridEntity;
    protected IEnumerable<GOAPActionDelegate> _plan;
    protected int _securityStopWatch = 200;
    protected Transform target;

    public event Action<BaseAgent, Waypoint, bool>	OnReachDestination = delegate {};

    Vector3 _vel;
    public float speed = 2f;

    // Waypoint _gizmoRealTarget;
    // IEnumerable<Waypoint> _gizmoPath;

    public virtual float MovementSpeed { get; set; }

    protected virtual void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _terrainChecker = GetComponentInChildren<TerrainChecker>();
    }
    
    public bool IsTerrain { get { return _terrainChecker.isTerrain; } }

    Vector3 FloorPos(MonoBehaviour b) {
        return FloorPos(b.transform.position);
    }
    Vector3 FloorPos(Vector3 v) {
        return new Vector3(v.x, 0f, v.z);
    }

    
    protected virtual IEnumerator Navigate(Vector3 destination)
    {
        var srcWp = Navigation.instance.NearestTo(transform.position);
        var dstWp = Navigation.instance.NearestTo(destination);
		
        // _gizmoRealTarget = dstWp;
        Waypoint reachedDst = srcWp;

        if(srcWp != dstWp)
        {
            var path = AStarNormal<Waypoint>.Run(
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
                        .Select(a => new AStarNormal<Waypoint>.Arc(a, Vector3.Distance(a.transform.position, w.transform.position)))
            );
            if(path != null) {
                Debug.Log("COUNT" + path.Count());
                foreach(var next in path.Select(w => FloorPos(w))) {
                    Debug.Log("NEXT "+ next.ToString());

                    while((next - FloorPos(this)).sqrMagnitude >= 0.05f) {
                        _vel = (next - FloorPos(this)).normalized;
                        yield return null;
                    }
                    //_vel = (next - FloorPos(this)).normalized;
                    //yield return new WaitUntil(() => (next - FloorPos(this)).sqrMagnitude < 0.05f);
                }
            }
            reachedDst = path.Last();
        }

        if(reachedDst == dstWp) {
            _vel = (FloorPos(destination) - FloorPos(this)).normalized;
            yield return new WaitUntil(() => (FloorPos(destination) - FloorPos(this)).sqrMagnitude < 0.05f);
        }
		
        _vel = Vector3.zero;
        OnReachDestination(this, reachedDst, reachedDst == dstWp);
    }


    public void OnDead() {
        // Hotfix
        if (_life > 0) return;

        _meshRenderer.material.color = Consts.AGENT_DEAD_COLOR;
        Destroy(GetComponent<BoxCollider>());
        StartCoroutine("DestroyAgent");
    }

    private IEnumerator DestroyAgent() {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }

    public int GetSecurityStopWatch() {
        return _securityStopWatch;
    }

    public IEnumerator GoapRunDelegate(
        GOAPStateDelegate from,
        GOAPStateDelegate to,
        IEnumerable<GOAPActionDelegate> actions
    ) {
        Func<GOAPStateDelegate, GOAPStateDelegate, float> heuristic = (curr, goal) => {
            return goal.caseFromValues.Count(goalKv => !goalKv.Value(curr));
        };

        yield return AStarTimeSlice<GOAPStateDelegate>.Run(
            from,
            to,
            heuristic,
            curr => { return to.caseFromValues.All(kv => kv.Value(curr)); },
            curr => {
                if (_securityStopWatch == 0) {
                    return Enumerable.Empty<AStarTimeSlice<GOAPStateDelegate>.Arc>();
                }

                _securityStopWatch--;

                return actions
                    .Where(action => action.ValidatePreconditions(curr))
                    .Aggregate(new FList<AStarTimeSlice<GOAPStateDelegate>.Arc>(), (possibleList, action) => {
                        var newState = new GOAPStateDelegate(curr);
                        newState.ApplyEffects(action.effects);
                        newState.generatingAction = action;
                        newState.step = curr.step + 1;
                        return possibleList + new AStarTimeSlice<GOAPStateDelegate>.Arc(newState, action.cost);
                    });
            });
        var seq = AStarTimeSlice<GOAPStateDelegate>.GetSequence();

        if (seq == null) {
            yield break;
        }

        var costSoFar = 0f;
        // foreach (var act in seq.Skip(1)) {
        //     var heuristicValue = heuristic(act, to);
        //     var dijkstraValue = act.generatingAction.cost;
        //     costSoFar += dijkstraValue + heuristicValue;
        //     var stringValues = "\n" + "(CostSoFar: " + costSoFar + ")" + "\n" +
        //                        "(Heuristic: " + heuristicValue + ")" + "\n" +
        //                        "(Dijkstra: " + dijkstraValue + ")" + "\n" +
        //                        "(Step: " + act.step + ")";
        //     Debug.Log(act + "----------------------------" + stringValues);
        // }

        this._plan = seq.Skip(1).Select(x => x.generatingAction);
    }
}