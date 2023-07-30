using System;
using System.Collections.Generic;
using UnityEngine;
using Debug = Logger.Debug;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/LoggerSetup", order = 1)]
public class LoggerSetup : ScriptableObject {
    public List<string> keys = new List<string>();
    public List<bool> values = new List<bool>();

    private void OnEnable() {
        foreach (var kvp in Debug.classLogger) {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }
}