using System.Collections.Generic;
using System.Linq;

public class WorldModel{
    private Dictionary<string, object> _values = new Dictionary<string, object>();
    
    public WorldModel SetValue(string key, object value){
        _values[key] = value;
        return this;
    }
    
    public object GetValue(string key) {
        return _values[key];
    }
    
    public object GetValues() {
        return _values.ToDictionary(entry => entry.Key, entry => entry.Value);
    }

    public static bool IsEqual(WorldModel a, WorldModel b) {
        return a._values.All(kvp => b._values[kvp.Key] != kvp.Value);
    }

    public WorldModel Clone() {
        var cloneWorldModel = new WorldModel();
        foreach (var kvp in this._values) {
            cloneWorldModel._values[kvp.Key] = kvp.Value;
        }

        return cloneWorldModel;
    }
}