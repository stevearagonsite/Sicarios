#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LoggerSetup))]
public class LoggerSetupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LoggerSetup proxy = (LoggerSetup)target;

        // Empieza a hacer cambios al objeto
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("CLASS TYPE & KEY", new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            wordWrap = true,
        }, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.75f));
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("ON/OFF", new GUIStyle(GUI.skin.label)
        {
            fontSize = 12,
            wordWrap = true,
            alignment = TextAnchor.MiddleCenter
        }, GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.15f));
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.Space(10);
        EditorGUILayout.EndHorizontal();
        for (int i = 0; i < proxy.keys.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();
            proxy.keys[i] = EditorGUILayout.TextField(
                proxy.keys[i], 
                GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.75f)
            );
            EditorGUILayout.Space(10);
            proxy.values[i] = EditorGUILayout.Toggle(
                proxy.values[i], 
                new GUIStyle(EditorStyles.toggle),
                GUILayout.Width(EditorGUIUtility.currentViewWidth * 0.1f)
            );
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space(10);
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add new entry"))
        {
            proxy.keys.Add("");
            proxy.values.Add(false);
        }

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Turn all off")) {
            for (int i = proxy.values.Count - 1; i >= 0; i--) {
                proxy.values[i] = false;
            }
        }
        if (GUILayout.Button("Remove all")) {
            proxy.keys.Clear();
            proxy.values.Clear();
        }
        EditorGUILayout.EndHorizontal();

        // Si hay cambios, marca el objeto como sucio para que Unity lo guarde
        if (EditorGUI.EndChangeCheck())
        {
            EditorUtility.SetDirty(proxy);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif