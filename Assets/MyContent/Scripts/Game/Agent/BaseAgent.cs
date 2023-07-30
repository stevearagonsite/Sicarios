using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using MyContent.Scripts;
using UnityEngine;
using Debug = Logger.Debug;
using AgentSicarioActions = AgentSicarioConstants.AgentSicarioActions;
using AgentSicarioStates = AgentSicarioConstants.AgentSicarioStates;
using ItemsConstants = Items.ItemConstants;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GridEntity))]
[RequireComponent(typeof(MeshRenderer))]
public abstract class BaseAgent : MonoBehaviour {
    protected TerrainChecker _terrainChecker;
    protected MeshRenderer _meshRenderer;
    protected float _life = 100;
    public CircleQuerier radiusQuerier;
    public GridEntity _gridEntity;
    protected IEnumerable<GOAPActionDelegate> _goapPlan;
    protected int _securityStopWatch = 200;
    protected Item target;
    protected List<Item> _items = new List<Item>();

    public IEnumerable<Item> items {
        get { return _items; }
    }

    protected IEnumerable<GOAPActionDelegate> GoapPlan {
        set {
            var actionToItem = new Dictionary<string, string>() {
                { AgentSicarioActions.ACTION_REST_IN_THE_COMMUNE, ItemsConstants.ITEM_TYPE_COMMUNE },
                { AgentSicarioActions.ACTION_STUDY_LOCATIONS, ItemsConstants.ITEM_TYPE_LOCATION },
                { AgentSicarioActions.ACTION_STEAL_MONEY, ItemsConstants.ITEM_TYPE_MONEY },
                { AgentSicarioActions.ACTION_KILL_GUARD_GUN, ItemsConstants.ITEM_TYPE_GUARD },
                { AgentSicarioActions.ACTION_KILL_GUARD_SNIPER, ItemsConstants.ITEM_TYPE_GUARD },
                { AgentSicarioActions.ACTION_KILL_TARGET_GUN, ItemsConstants.ITEM_TYPE_TARGET },
                { AgentSicarioActions.ACTION_KILL_TARGET_SNIPER, ItemsConstants.ITEM_TYPE_TARGET },
                { AgentSicarioActions.ACTION_BUY_GUN, ItemsConstants.ITEM_TYPE_STORE },
                { AgentSicarioActions.ACTION_BUY_SNIPER, ItemsConstants.ITEM_TYPE_STORE },
            };

            var actionToState = new Dictionary<string, string>() {
                { AgentSicarioActions.ACTION_REST_IN_THE_COMMUNE, AgentSicarioStates.GO_TO },
                { AgentSicarioActions.ACTION_STUDY_LOCATIONS, AgentSicarioStates.GO_TO },
                { AgentSicarioActions.ACTION_STEAL_MONEY, AgentSicarioStates.PURSUIT },
                { AgentSicarioActions.ACTION_KILL_GUARD_GUN, AgentSicarioStates.KILL },
                { AgentSicarioActions.ACTION_KILL_GUARD_SNIPER, AgentSicarioStates.KILL },
                { AgentSicarioActions.ACTION_KILL_TARGET_GUN, AgentSicarioStates.KILL },
                { AgentSicarioActions.ACTION_KILL_TARGET_SNIPER, AgentSicarioStates.KILL },
                { AgentSicarioActions.ACTION_BUY_GUN, AgentSicarioStates.GO_TO },
                { AgentSicarioActions.ACTION_BUY_SNIPER, AgentSicarioStates.GO_TO },
            };

            var everything = Navigation.instance.AllItems().Union(Navigation.instance.AllInventories());
            _goapPlan = value;
            var plan = _goapPlan
                .Select(goapAction => goapAction.name)
                .Select(actionName => {
                    var itemForAction = everything.FirstOrDefault(item =>
                        actionToItem.Any(kv => actionName == kv.Key) &&
                        item.type == actionToItem.First(kv => actionName == kv.Key).Value
                    );
#if UNITY_EDITOR
                    if (itemForAction == null) {
                        Debug.LogColor("GOAP planner", "ITEM FOR ACTION FAIL", "red");
                        Debug.LogColor("GOAP planner", "action: " + actionName, "red");
                        Debug.LogColor("GOAP planner", "everything count: " + everything.Count(), "red");
                        Debug.LogColor("GOAP planner", "condition actionToItem: "  + actionToItem.Any(kv => actionName == kv.Key), "red");
                        var firstItem = actionToItem.First(kv => actionName == kv.Key);
                        Debug.LogColor("GOAP planner", "item FIRST: " + firstItem.Key + " " + firstItem.Value, "red");
                        Debug.LogError("GOAP planner");
                    }
#endif
                    if (actionToState.Any(kv => actionName == kv.Key) && itemForAction != null) {
                        return Tuple
                            .Create(
                                actionToState.First(kv => actionName.StartsWith(kv.Key)).Value,
                                itemForAction
                            );
                    }
                    return null;
                }).Where(a => a != null)
                .ToList();
#if UNITY_EDITOR
            Debug.Log("GOAP planner", "GoapPlan: " + string.Join(", ", _goapPlan.Select(pa => pa.name)));
            Debug.Log("GOAP planner", "Plan: " + string.Join(", ", plan.Select(pa => plan.IndexOf(pa) + ": " + pa.Item1 + " " + pa.Item2.name).ToArray()));
#endif
        }
    }

    public event Action<BaseAgent, Waypoint, bool> OnReachDestination = delegate { };

    Vector3 _vel;
    public float speed = 2f;

    // Waypoint _gizmoRealTarget;
    // IEnumerable<Waypoint> _gizmoPath;

    public virtual float MovementSpeed { get; set; }

    protected virtual void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _terrainChecker = GetComponentInChildren<TerrainChecker>();
    }

    public bool IsTerrain {
        get { return _terrainChecker.isTerrain; }
    }

    // Vector3 FloorPos(MonoBehaviour b) {
    //     return FloorPos(b.transform.position);
    // }
    // Vector3 FloorPos(Vector3 v) {
    //     return new Vector3(v.x, 0f, v.z);
    // }
    // protected virtual IEnumerator Navigate(Vector3 destination)
    // {
    //     var srcWp = Navigation.instance.NearestTo(transform.position);
    //     var dstWp = Navigation.instance.NearestTo(destination);
    //
    //     // _gizmoRealTarget = dstWp;
    //     Waypoint reachedDst = srcWp;
    //
    //     if(srcWp != dstWp)
    //     {
    //         var path = AStarNormal<Waypoint>.Run(
    //             srcWp
    //             , dstWp
    //             , (wa, wb) => Vector3.Distance(wa.transform.position, wb.transform.position)
    //             , w => w == dstWp
    //             , w =>
    //                 //w.nearbyItems.Any(it => it.type == ItemType.Door)
    //                 //? null
    //                 //:
    //                 w.adyacent
    //                     //.Where(a => a.nearbyItems.All(it => it.type != ItemType.Door))
    //                     .Select(a => new AStarNormal<Waypoint>.Arc(a, Vector3.Distance(a.transform.position, w.transform.position)))
    //         );
    //         if(path != null) {
    //             Debug.Log("COUNT" + path.Count());
    //             foreach(var next in path.Select(w => FloorPos(w))) {
    //                 Debug.Log("NEXT "+ next.ToString());
    //
    //                 while((next - FloorPos(this)).sqrMagnitude >= 0.05f) {
    //                     _vel = (next - FloorPos(this)).normalized;
    //                     yield return null;
    //                 }
    //                 //_vel = (next - FloorPos(this)).normalized;
    //                 //yield return new WaitUntil(() => (next - FloorPos(this)).sqrMagnitude < 0.05f);
    //             }
    //         }
    //         reachedDst = path.Last();
    //     }
    //
    //     if(reachedDst == dstWp) {
    //         _vel = (FloorPos(destination) - FloorPos(this)).normalized;
    //         yield return new WaitUntil(() => (FloorPos(destination) - FloorPos(this)).sqrMagnitude < 0.05f);
    //     }
    //
    //     _vel = Vector3.zero;
    //     OnReachDestination(this, reachedDst, reachedDst == dstWp);
    // }


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
#if UNITY_EDITOR
        foreach (var act in seq.Skip(1)) {
            var heuristicValue = heuristic(act, to);
            var dijkstraValue = act.generatingAction.cost;
            costSoFar += dijkstraValue + heuristicValue;
            var stringValues = "\n" + "(CostSoFar: " + costSoFar + ")" + "\n" +
                               "(Heuristic: " + heuristicValue + ")" + "\n" +
                               "(Dijkstra: " + dijkstraValue + ")" + "\n" +
                               "(Step: " + act.step + ")";
            Debug.Log("GOAP", act + "----------------------------" + stringValues);
        }
#endif
        GoapPlan = seq.Skip(1).Select(x => x.generatingAction);
    }
}