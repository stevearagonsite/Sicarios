using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utils{
    public static bool In<T>(this T x, HashSet<T> set) {
        return set.Contains(x);
    }

    public static bool In<K, V>(this KeyValuePair<K, V> x, Dictionary<K, V> dict) {
        return dict.Contains(x);
    }

    public static IEnumerable<T> GeneratePath<T>(T seed, Func<T, T> mutate) {
        var accum = seed;
        while (true) {
            yield return accum;
            accum = mutate(accum);
        }
    }

    public static ValueDefault DefaultGet<Key, ValueDefault>(
        this Dictionary<Key, ValueDefault> dict,
        Key key,
        Func<ValueDefault> defaultFactory
    ) {
        ValueDefault value;
        if (!dict.TryGetValue(key, out value))
            dict[key] = value = defaultFactory();
        return value;
    }

    public static void UpdateWith<K, V>(this Dictionary<K, V> a, Dictionary<K, V> b) {
        foreach (var kvp in b) {
            a[kvp.Key] = kvp.Value;
        }
    }
}