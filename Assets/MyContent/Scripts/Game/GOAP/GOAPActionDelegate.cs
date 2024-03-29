﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GOAPActionDelegate {
    public Dictionary<string, Func<GOAPStateDelegate, bool>> preconditions { get; private set; }
    public Dictionary<string, Func<GOAPStateDelegate, GOAPStateDelegate>> effects { get; private set; }
    public string name { get; private set; }
    public float cost { get; private set; }

    public GOAPActionDelegate(string name) {
        this.name = name;
        cost = 1f;
        preconditions = new Dictionary<string, Func<GOAPStateDelegate, bool>>();
        effects = new Dictionary<string, Func<GOAPStateDelegate, GOAPStateDelegate>>();
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

    public bool ValidatePreconditions(GOAPStateDelegate state) {
        return preconditions.All(kvp => kvp.Value(state));
    }

    public GOAPActionDelegate Pre(string s, Func<GOAPStateDelegate, bool> value) {
        preconditions[s] = value;
        return this;
    }


    public GOAPActionDelegate Effect(string s, Func<GOAPStateDelegate, GOAPStateDelegate> value) {
        effects[s] = value;
        return this;
    }

    public void ExecuteAction(BaseAgent agent) {
        Debug.Log("Executing action: " + name);
    }
}