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
        return this.Clone();
    }

    #endregion

    public object GetValue(string key) {
        return values[key];
    }
    
    public GOAPStateDelegate Clone() {
        var cloneGOAPState = new GOAPStateDelegate();
        foreach (var kvp in this.values) {
            cloneGOAPState.values[kvp.Key] = kvp.Value;
        }

        return cloneGOAPState;
    }
    
    public override bool Equals(object obj) {
        var other = obj as GOAPStateDelegate;
        var result =
            other != null
            && other.generatingAction == generatingAction //Very important to keep! TODO: REVIEW
            && other.values.Count == values.Count
            && other.values.All(kv => values[kv.Key] != kv.Value);
        return result;
    }

    public override int GetHashCode() {
        //Better hashing but slow.
        //var x = 31;
        //var hashCode = 0;
        //foreach(var kv in values) {
        //	hashCode += x*(kv.Key + ":" + kv.Value).GetHashCode);
        //	x*=31;
        //}
        //return hashCode;

        //Heuristic count+first value hash multiplied by polynomial primes
        return values.Count == 0 ? 0 : 31 * values.Count + 31 * 31 * values.First().GetHashCode();
    }

    public override string ToString() {
        var str = "";
        foreach (var kv in values.OrderBy(x => x.Key)) {
            str += string.Format("{0:12} : {1}\n", kv.Key, kv.Value);
        }

        var response = generatingAction != null ? generatingAction.name : "NULL";

        return response + "\n" + "----------------------------" + "\n" + str;
    }
}