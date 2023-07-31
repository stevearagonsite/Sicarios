using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AgentSicarioConstants;
using FSM;
using Items;
using LINQExtension;
using MyContent.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = Logger.Debug;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(LineOfSight))]
public class StatePursuit : MonoBehaviour, IState {
    private ThinkGetItemByView _getItemByView = ThinkingAgent.Targets;
    public event Action<Item, bool> OnCatchItem = delegate { };
    private float minTimeRamdomWalk = 2f;
    private float maxTimeRamdomWalk = 4f;
    private float _timeRamdomWalk;
    public readonly string name;
    private BaseAgent _agent;
    private Coroutine _coroutine;
    private Transform _target;
    private Rigidbody _rb;
    private LineOfSight _lineOfSight;

    public Transform target {
        get => _target;
        set {
#if UNITY_EDITOR
            Debug.LogColor(this, $"Pursuit: ({value.name})", "yellow");
#endif
            _lineOfSight.target = value.gameObject;
            _target = value;
        }
    }

    private void Start() {
        _rb = GetComponent<Rigidbody>();
        _lineOfSight = _agent.lineOfSight;
        _timeRamdomWalk = maxTimeRamdomWalk;
        OnCatchItem += (i, b) => {
#if UNITY_EDITOR
            Debug.LogColor(this, $"OnCatchItem: {i.name} {b}", "yellow");
#endif
            if (!b) return;
            i.OnInventoryAdd(_agent);
            _agent.Feed(AgentSicarioStates.PLAN_STEP);
        };
    }

    public IState SetValues(string name, BaseAgent agent) {
#if UNITY_EDITOR
        Debug.LogColor(this, $"SetValues: {name}", "yellow");
#endif
        name = name;
        _agent = agent;
        // Grid.instance.Query();
        return this;
    }

    public void OnEnter() {
#if UNITY_EDITOR
        Debug.LogColor(this, "OnEnter", "yellow");
#endif
        OnPursuitItem();
    }

    public void OnUpdate() {
        if(_target == null) {
            WalkRandomForward();
            return;
        }

        var direction = (target.position - transform.position).normalized;
        var lookRotation = Quaternion.LookRotation(direction);
        _rb.rotation = Quaternion.Slerp(_rb.rotation, lookRotation, Time.fixedDeltaTime * 5);
        _rb.MovePosition(transform.position + Time.fixedDeltaTime * direction * _agent.MovementSpeed);
    }

    public void OnExit() {
#if UNITY_EDITOR
        Debug.LogColor(this, "OnExit", "yellow");
#endif
        OffPursuitItem();
    }

    private void OnPursuitItem() {
        _coroutine = StartCoroutine(SearchItem(0.3f));
    }

    private void OffPursuitItem() {
        StopCoroutine(_coroutine);
    }

    private IEnumerator SearchItem(float time) {
        while (true) {
            var item = _agent.target;
            var everything = Navigation.instance
                .AllItems()
                .Union(Navigation.instance.AllInventories());
            
            var itemInFieldOfView = _getItemByView(_agent, everything);
            // Prevent errors when the agent is not view of any item
            if (itemInFieldOfView == null || itemInFieldOfView.Count() == 0) {
                yield return new WaitForSeconds(time);
                continue;
            }

            target = itemInFieldOfView.First(item => item.type == _agent.target.type).transform;
            yield return new WaitForSeconds(time);
        }
    }

    public void WalkRandomForward() {
        _agent.transform.position += transform.forward * Time.deltaTime * _agent.MovementSpeed;

        if (_timeRamdomWalk > 0) {
            _timeRamdomWalk -= Time.deltaTime;
            return;
        }
        _timeRamdomWalk = Random.Range(minTimeRamdomWalk, maxTimeRamdomWalk);
    }

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.layer != Layers.AGENT_NUM_LAYER && _agent.fsm != null && _agent.fsm.current.name == AgentSicarioStates.PURSUIT) return;
        var item = other.gameObject.GetComponent<Item>();
        OnCatchItem(item, item.name == _agent.target.name);
    }
}