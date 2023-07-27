using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UpdateUIGoap : MonoBehaviour {
    public TMP_Text tmStopwatch;
    public TMP_Text tmActionsCounter;
    private GoapMiniTest _goapMiniTest;
    public static  Dictionary<string, int> actionsCounter = new Dictionary<string, int>();

    private void Start() {
        _goapMiniTest = FindObjectOfType<GoapMiniTest>();
    }

    private void Update() {
        if (_goapMiniTest == null) return;
        tmStopwatch.text = "Stopwatch: " + _goapMiniTest.GetSecurityStopWatch();
        var actionsText = "ACTIONS: \n";
        foreach (var VARIABLE in actionsCounter) {
            actionsText += VARIABLE.Key + ": " + VARIABLE.Value + "\n";
        }
        tmActionsCounter.text = actionsText;
    }
}
