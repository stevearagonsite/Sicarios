using System;
using System.Collections;
using FSM;
using UnityEngine;
using Random = UnityEngine.Random;

public class StateIdle : MonoBehaviour, IState {
    public ThinkDelegatePerson randomWalk = ThinkingPerson.RandomWalk;

    private string _name;
    private BaseAgent _agent;
    private Coroutine _coroutine;
    
    public string name => _name;

    public StateIdle SetValues(string name, BaseAgent agent) {
        this._name = name;
        _agent = agent;
        
        return this;
    }

    #region IState

    public void OnEnter() {
        OnRandomMove();
    }

    public void OnUpdate() {
        WalkForward();
    }

    public void OnExit() {
        OffRandomMove();
    }
    
    #endregion IState
    
    public void WalkForward() {
        if (_agent.IsTerrain) {
            return;
        }

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