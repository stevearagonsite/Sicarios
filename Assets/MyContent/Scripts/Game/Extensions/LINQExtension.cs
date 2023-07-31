using System;
using System.Collections.Generic;
using System.Linq;
using Items;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Debug = Logger.Debug;

namespace LINQExtension {
    public static class DoubleOperations {
        public static double Median(this IEnumerable<double> source) {
            if (source.Count() == 0) {
                Debug.LogError("Cannot compute median for an empty set.");
            }

            var sortedList = from number in source
                orderby number
                select number;

            var itemIndex = (int)sortedList.Count() / 2;

            if (sortedList.Count() % 2 == 0) {
                // Even number of items.
                return (sortedList.ElementAt(itemIndex) + sortedList.ElementAt(itemIndex - 1)) / 2;
            }
            else {
                // Odd number of items.
                return sortedList.ElementAt(itemIndex);
            }
        }
    }

    public static class DebugExtension {
        public static IEnumerable<BaseAgent> Log(this IEnumerable<BaseAgent> source, Action<BaseAgent> predicate) {
            foreach (var item in source) {
                predicate(item);
            }

            return source;
        }
        
        public static IEnumerable<Item> Log(this IEnumerable<Item> source, Action<Item> predicate) {
            foreach (var item in source) {
                predicate(item);
            }

            return source;
        }
        public static IEnumerable<JToken> Log(this IEnumerable<JToken> source) {
            foreach (var item in source) {
                Debug.Log(item);
            }

            return source;
        }
    }
    
    public static class EnumerableExtension {
        public static IEnumerable<T> ToAdd<T>(this IEnumerable<T> source, T item) {
            foreach (var i in source) {
                yield return i;
            }

            yield return item;
        }
        
        public static IEnumerable<T> ToAddFirst<T>(this IEnumerable<T> source, T item) {
            yield return item;

            foreach (var i in source) {
                yield return i;
            }
        }
        
        public static IEnumerable<Vector3> DuplicateAndSetOffset(this IEnumerable<Transform> source, float distance = 10f) {
            foreach (var t in source) {
                var position = t.position;
                yield return position;
                // Calculate projected position and Add a 10 meter offset
                var projectedPosition = position + (t.forward * distance);
                yield return projectedPosition;
            }
        }
        
        public static IEnumerable<T> ToAddList<T>(this IEnumerable<T> source, IEnumerable<T> items) {
            foreach (var i in source) {
                yield return i;
            }

            foreach (var i in items) {
                yield return i;
            }
        }
        
        public static IEnumerable<T> ToAddFirstList<T>(this IEnumerable<T> source, IEnumerable<T> items) {
            foreach (var i in items) {
                yield return i;
            }
            
            foreach (var i in source) {
                yield return i;
            }
        }
    }
}