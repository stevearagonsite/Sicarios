using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPActionDelegate{
    public Dictionary<string, Delegate> preconditions { get; private set; }
    public Dictionary<string, Delegate> effects { get; private set; }
    public string name { get; private set; }
    public float cost { get; private set; }

    public GOAPActionDelegate(string name) {
        this.name = name;
        cost = 1f;
        preconditions = new Dictionary<string, Delegate>();
        effects = new Dictionary<string, Delegate>();
    }

    public GOAPActionDelegate Cost(float cost) {
        if (cost < 1f) {
            //Costs < 1f make the heuristic non-admissible. h() could overestimate and create sub-optimal results.
            //https://en.wikipedia.org/wiki/A*_search_algorithm#Properties
            Debug.Log(string.Format("Warning: Using cost < 1f for '{0}' could yield sub-optimal results", name));
        }

        this.cost = cost;
        return this;
    }

    public GOAPActionDelegate Pre(string s, Delegate value) {
        preconditions[s] = value;
        return this;
    }
    

    public GOAPActionDelegate Effect(string s, Delegate value) {
        effects[s] = value;
        return this;
    }
}