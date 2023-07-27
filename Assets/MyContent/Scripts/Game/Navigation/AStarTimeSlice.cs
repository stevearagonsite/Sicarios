using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using Debug = Logger.Debug;

public class AStarTimeSlice<Node> where Node : class{
    private static int _timeSlice = 10;
    private static bool _planIsFinished = false;
    private static IEnumerable<Node> _seq = null;
    public class Arc{
        public Node endpoint;
        public float cost;

        //Could save the both in a tuple, but creating a custom class give more readability below
        public Arc(Node ep, float c) {
            endpoint = ep;
            cost = c;
        }
    }

    // Expand can return null as "no neighbours"
    public static IEnumerator Run
    (
        Node from,
        Node to,
        // Heuristic cost
        Func<Node, Node, float> heuristic,
        // Satisfies
        Func<Node, bool> satisfies,
        // Endpoints costs
        Func<Node, IEnumerable<Arc>> expand
    ) {
        var initialState = new AStarState<Node>();
        initialState.open.Add(from);
        initialState.gs[from] = 0;
        initialState.fs[from] = heuristic(from, to);
        initialState.previous[from] = null;
        initialState.current = from;

        var state = initialState;
        while (state.open.Count > 0 && !state.finished) {
            // Debugger gets buggy af with this, can't watch variable:
            state = state.Clone();

            var candidate = state.open.OrderBy(x => state.fs[x]).First();
            state.current = candidate;

            UpdateUIGoap.actionsCounter[candidate.ToString()] = 
                UpdateUIGoap.actionsCounter.DefaultGet(candidate.ToString(), () => 0) + 1;
            
            if (satisfies(candidate)) {
                state.finished = true; // Has been found the goal!!!
            }

            state.open.Remove(candidate);
            state.closed.Add(candidate);
            _timeSlice--;
            if (_timeSlice <= 0) {
                _timeSlice = 10;
                yield return new WaitForEndOfFrame();
            }
            var neighbours = expand(candidate);

            // has neighbours ?
            if (neighbours == null || !neighbours.Any()) continue;

            var gCandidate = state.gs[candidate];

            foreach (var ne in neighbours) {
                if (ne.endpoint.In(state.closed)) continue;

                var gNeighbour = gCandidate + ne.cost;
                state.open.Add(ne.endpoint);

                if (gNeighbour > state.gs.DefaultGet(ne.endpoint, () => gNeighbour)) continue;

                state.previous[ne.endpoint] = candidate;
                state.gs[ne.endpoint] = gNeighbour;
                state.fs[ne.endpoint] = gNeighbour + heuristic(ne.endpoint, to);
            }
        }
        
        if (!state.finished) {
            _planIsFinished = true;
            _seq = null;
            yield break;
        }


        _planIsFinished = true;
        _seq = Utils.GeneratePath(state.current, n => state.previous[n])
            .TakeWhile(n => n != null)
            .Reverse();
    }
    
    public static void resetValues(){
        _timeSlice = 10;
        _planIsFinished = false;
        _seq = null;
    }

    public static IEnumerable<Node> GetSequence() {
        return _seq;
    }
    
    public static bool IsPlanFinished() {
        return _planIsFinished;
    }
}