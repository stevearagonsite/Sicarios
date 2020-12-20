using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour{
    public GameObject agentContainer;
    public GameObject itemsContainer;
    public GameObject baseAgent;
    public int quantitySicarios = 3;
    public int quantityPersons = 20;
    public int quantityDetective = 2;
    public int quantityTarget = 2;

    private List<BaseAgent> _agentsCollection;
    private List<IItem> _itemsCollection;
    
    // Random positioning
    private List<Waypoint> _listWaypoints;
    private List<Waypoint> _listAvaiableWaypoints;

    private void Start() {
        _agentsCollection = new List<BaseAgent>();
        _itemsCollection = new List<IItem>();
        _listWaypoints = FindObjectsOfType<Waypoint>().ToList();
        _listAvaiableWaypoints = _listWaypoints;
        
        if (quantitySicarios < 0 || quantityPersons < 0 || 
            quantityDetective < 0 || quantityTarget < 0) {
            Debug.LogError("The quantity creation can not be null");
            return;
        }
        
        SaveAgents();
        CreateAgents();
    }

    private void SaveAgents() {
        foreach (Transform agentObject in agentContainer.transform) {
            _agentsCollection.Add(agentObject.gameObject.GetComponent<BaseAgent>());
        }
    }

    private void CreateAgents() {
        for (var i = 0; i < quantitySicarios; i++) {
            var randomTransform = GetRandomPositionWaypoint();
            var gameObject = Instantiate(baseAgent, agentContainer.transform, true);
            gameObject.transform.position = randomTransform.position;
            _agentsCollection.Add(gameObject.AddComponent<AgentSicario>());
        }
        
        for (var i = 0; i < quantityPersons; i++) {
            var randomTransform = GetRandomPositionWaypoint();
            var gameObject = Instantiate(baseAgent, agentContainer.transform, true);
            gameObject.transform.position = randomTransform.position;
            _agentsCollection.Add(gameObject.AddComponent<AgentPerson>());
        }
        
        for (var i = 0; i < quantityDetective; i++) {
            var randomTransform = GetRandomPositionWaypoint();
            var gameObject = Instantiate(baseAgent, agentContainer.transform, true);
            gameObject.transform.position = randomTransform.position;
            _agentsCollection.Add(gameObject.AddComponent<AgentDetective>());
        }
        
        for (var i = 0; i < quantityTarget; i++) {
            var randomTransform = GetRandomPositionWaypoint();
            var gameObject = Instantiate(baseAgent, agentContainer.transform, true);
            gameObject.transform.position = randomTransform.position;
            _agentsCollection.Add(gameObject.AddComponent<AgentTarget>());
        }
    }

    Transform GetRandomPositionWaypoint() {
        int randomNumber = Random.Range(0, _listAvaiableWaypoints.Count);
        var randomWaypoint = _listAvaiableWaypoints[randomNumber];
        _listAvaiableWaypoints.RemoveAt(randomNumber);
        
        return randomWaypoint.transform;
    }

    private void SaveItems() {
        foreach (Transform itemObject in itemsContainer.transform) {
            _itemsCollection.Add(itemObject.gameObject.GetComponent<IItem>());
        }
    }
}