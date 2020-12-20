using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour{
    public GameObject agentContainer;
    public GameObject itemsContainer;

    private List<BaseAgent> _agentsCollection;
    private List<IItem> _itemsCollection;

    private void CreateAgents() {
        _agentsCollection = new List<BaseAgent>();

        // Save all agents that have been created
        foreach (Transform agentObject in agentContainer.transform) {
            _agentsCollection.Add(agentObject.gameObject.GetComponent<BaseAgent>());
        }
    }

    private void CreateItems() {
        _itemsCollection = new List<IItem>();
        
        // Save all items that have been created
        foreach (Transform itemObject in itemsContainer.transform) {
            _itemsCollection.Add(itemObject.gameObject.GetComponent<IItem>());
        }
    }
}