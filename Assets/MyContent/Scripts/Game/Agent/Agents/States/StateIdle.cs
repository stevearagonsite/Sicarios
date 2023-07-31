using System;
using System.Collections;
using FSM;
using MyContent.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;
using Debug = Logger.Debug;

public class StateIdle : MonoBehaviour, IState {
    public Action<BaseAgent> randomWalk = ThinkingAgent.RandomWalk;

    private string _name;
    private BaseAgent _agent;
    private Coroutine _coroutine;
    
    public string name => _name;

    public IState SetValues(string name, BaseAgent agent) {
#if UNITY_EDITOR
        Debug.LogColor(this, $"SetValues: {name}", "yellow");
#endif
        _name = name;
        _agent = agent;
        
        return this;
    }

    #region IState

    public void OnEnter() {
#if UNITY_EDITOR
        Debug.LogColor(this, "OnEnter", "yellow");
#endif
        OnRandomMove();
    }

    public void OnUpdate() {
        WalkForward();
    }

    public void OnExit() {
#if UNITY_EDITOR
        Debug.LogColor(this, "OnExit", "yellow");
#endif
        OffRandomMove();
        _agent.MovementSpeed = Consts.AGENT_MAX_MOVEMENT_SPEED;
    }
    
    #endregion IState
    
    public void WalkForward() {
        _agent.transform.position += transform.forward * Time.deltaTime * _agent.MovementSpeed;
    }

    private void OnRandomMove() {
        _coroutine = StartCoroutine(RandomWalk());
    }

    private void OffRandomMove() {
        StopCoroutine(_coroutine);
    }

    private IEnumerator RandomWalk() {
        while (true) {
            var time = Random.Range(1f, 5f);
            randomWalk((BaseAgent)_agent);
            yield return new WaitForSeconds(time);
        }
    }
}