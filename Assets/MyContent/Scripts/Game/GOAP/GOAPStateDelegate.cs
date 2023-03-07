using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GOAPStateDelegate{
    public Dictionary<string, object> values = new Dictionary<string, object>();
    public GOAPActionDelegate generatingAction = null;
    public int step = 0;

    #region CONSTRUCTOR

    public GOAPStateDelegate(GOAPActionDelegate gen = null) {
        generatingAction = gen;
    }

    public GOAPStateDelegate(GOAPStateDelegate source, GOAPActionDelegate gen = null) {
        foreach (var elem in source.values) {
            if (values.ContainsKey(elem.Key))
                values[elem.Key] = elem.Value;
            else
                values.Add(elem.Key, elem.Value);
        }

        generatingAction = gen;
    }
    
    public GOAPStateDelegate SetValue(string key, object value){
        values[key] = value;
        return this;
    }
    
    public GOAPStateDelegate SetValueInt(string key, int value){
        values[key] = value;
        return this;
    }
    
    public GOAPStateDelegate SetValueFloat(string key, float value){
        values[key] = value;
        return this;
    }
    
    public GOAPStateDelegate SetValueString(string key, string value){
        values[key] = value;
        return this;
    }
    
    public GOAPStateDelegate SetValueBool(string key, bool value){
        values[key] = value;
        return this;
    }

    #endregion

    public object GetValue(string key) {
        if (!values.ContainsKey(key)) throw new Exception($"Value {key} not found!");
        return values[key];
    }
    
    public int GetValueInt(string key) {
        if (!values.ContainsKey(key)) throw new Exception($"Value {key} not found!");
        if (values[key] is not int value) throw new Exception($"Value {key} is not an int!");
        return value;
    }
    
    public float GetValueFloat(string key) {
        if (!values.ContainsKey(key)) throw new Exception($"Value {key} not found!");
        if (values[key] is not float value) throw new Exception($"Value {key} is not an float!");
        return value;
    }
    
    public string GetValueString(string key) {
        if (!values.ContainsKey(key)) throw new Exception($"Value {key} not found!");
        if (values[key] is not string value) throw new Exception($"Value {key} is not an string!");
        return value;
    }
    
    public bool GetValueBool(string key) {
        if (!values.ContainsKey(key)) throw new Exception($"Value {key} not found!");
        if (values[key] is not bool value) throw new Exception($"Value {key} is not an bool!");
        return value;
    }
    
    public override bool Equals(object obj) {
        var other = obj as GOAPStateDelegate;
        var result =
            other != null
            && other.generatingAction == generatingAction //Very important to keep! TODO: REVIEW
            && other.values.Count == values.Count
            && other.values.All(kv => values[kv.Key] == kv.Value);
        return result;
    }
    
    public void ApplyEffects(Dictionary<string, Func<GOAPStateDelegate, GOAPStateDelegate>> effects) {
        foreach (var effect in effects) {
            effect.Value(this);
        }
    }

    public override int GetHashCode() {
        //Better hashing but slow.
        // var x = 31;
        // var hashCode = 0;
        // foreach(var kv in values) {
        // 	hashCode += (x*(kv.Key + ":" + kv.Value).GetHashCode);
        // 	x*=31;
        // }
        // return hashCode;

        //Heuristic count+first value hash multiplied by polynomial primes
        return values.Count == 0 ? 0 : 31 * values.Count + 31 * 31 * values.First().GetHashCode();
    }
    
    // public static bool operator ==(GOAPStateDelegate obj1, GOAPStateDelegate obj2) {
    //     return obj1 == null && obj2 == null || 
    //             obj1 != null && obj2 != null && obj1.Equals(obj2);
    // }
    //
    // public static bool operator !=(GOAPStateDelegate obj1, GOAPStateDelegate obj2) {
    //     return !(obj1 == obj2);
    // }
    
    // this is third one 'Equals'

    public override string ToString() {
        var str = "";
        foreach (var kv in values.OrderBy(x => x.Key)) {
            str += string.Format("{0:12} : {1}\n", kv.Key, kv.Value);
        }

        var response = generatingAction != null ? generatingAction.name : "NULL";

        return response + "\n" + "----------------------------" + "\n" + str;
    }
}