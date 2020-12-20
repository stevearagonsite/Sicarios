using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AStarNormal<Node> where Node : class{
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
    public static IEnumerable<Node> Run
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

            // TODO: Comment this
            // DebugGoap(state);

            if (satisfies(candidate)) {
                Utils.Log("SATISFIED"); // TODO: Comment this
                state.finished = true; // Has been found the goal!!!
            }

            state.open.Remove(candidate);
            state.closed.Add(candidate);
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

        if (!state.finished) return null;


        var seq = Utils.GeneratePath(state.current, n => state.previous[n])
            .TakeWhile(n => n != null)
            .Reverse();

        return seq;
    }
}