using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FSM;
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
[RequireComponent(typeof(LineOfSight))]
public abstract class BaseAgent : MonoBehaviour {
    protected TerrainChecker _terrainChecker;
    protected MeshRenderer _meshRenderer;
    protected float _life = 100;
    protected CircleQuerier radiusQuerier;
    protected GridEntity _gridEntity;
    protected IEnumerable<GOAPActionDelegate> _goapPlan;
    protected List<Tuple<string, Item>> _plan = new List<Tuple<string, Item>>();
    protected int _securityStopWatch = 200;
    protected Item _target;
    protected List<Item> _items = new List<Item>();
    protected EventFSM<string> _fsm;
    protected LineOfSight _lineOfSight;
    public IEnumerable<Item> items {
        get { return _items; }
    }
    
    public virtual void AddItem(Item item) {
        _items.Add(item);
    }

    protected abstract IEnumerable<GOAPActionDelegate> goapPlan { set; }

    public virtual EventFSM<string> fsm => _fsm;
    public virtual LineOfSight lineOfSight => _lineOfSight;

    public virtual Item target {
        get {
            return _target;
        }
        set {
            _target = value;
        }
    }
    
    public void Feed(string state) {
        _fsm.Feed(state);
    }


    public virtual IEnumerable<Tuple<string, Item>> plan {
        get {
            return _plan;
        }
        set {
            _plan = value.ToList();
        }
    }

    protected abstract void ExecutePlan(List<Tuple<string, Item>> plan);

    public event Action<BaseAgent, Waypoint, bool> OnReachDestination = delegate { };

    Vector3 _vel;
    public float speed = 2f;

    // Waypoint _gizmoRealTarget;
    // IEnumerable<Waypoint> _gizmoPath;

    public virtual float MovementSpeed { get; set; }

    protected virtual void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _terrainChecker = GetComponentInChildren<TerrainChecker>();
        _lineOfSight = GetComponentInChildren<LineOfSight>();
        _gridEntity = GetComponent<GridEntity>();
    }

    protected void Start() {
    }

    public bool IsTerrain {
        get { return _terrainChecker.isTerrain; }
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
        goapPlan = seq.Skip(1).Select(x => x.generatingAction);
    }
}