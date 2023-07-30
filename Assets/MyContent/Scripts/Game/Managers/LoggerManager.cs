using System;
using System.Collections;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Debug = Logger.Debug;

#if UNITY_EDITOR
[ExecuteAlways]
#endif
public class LoggerManager : MonoBehaviour {
    public LoggerSetup proxy;
    public bool UpdateScriptInformation;

    private void Awake() {
        AllowScriptsLogs();
    }

    private void Start() {
        StartCoroutine(UpdateExecution());
    }

    private IEnumerator UpdateExecution() {
        while (true) {
            UpdateScritableObjectInformation();
            AllowScriptsLogs();
            yield return new WaitForSeconds(3);
        }
    }
    
    private void UpdateScritableObjectInformation() {
        if (!UpdateScriptInformation) {
            return;
        }
        proxy.keys.Clear();
        proxy.values.Clear();
        foreach (var kvp in Debug.classLogger) {
            proxy.keys.Add(kvp.Key);
            proxy.values.Add(kvp.Value);
        }
#if UNITY_EDITOR
        EditorUtility.SetDirty(proxy);
#endif
    }

    private void AllowScriptsLogs() {
        for (var i = 0; i < proxy.keys.Count; i++) {
            var key = proxy.keys[i];
            Debug.classLogger[key] = proxy.values[i];
        }
    }
}