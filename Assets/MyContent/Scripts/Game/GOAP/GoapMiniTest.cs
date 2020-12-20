using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

/*
Modo psicopata = Menos costo matar
	Se puede "patchear" in game!
*/

public class GoapMiniTest : MonoBehaviour{
    
    public static IEnumerable<GOAPAction> GoapRun(GOAPState from, GOAPState to, IEnumerable<GOAPAction> actions) {
        int watchdog = 200;

        var seq = AStarNormal<GOAPState>.Run(
            from,
            to,
            (curr, goal) => goal.values.Count(kv => !kv.In(curr.values)),
            curr => to.values.All(kv => kv.In(curr.values)),
            curr => {
                
                if (watchdog == 0) return Enumerable.Empty<AStarNormal<GOAPState>.Arc>();

                watchdog--;

                return actions
                    .Where(action => action.preconditions.All(kv => kv.In(curr.values)))
                    .Aggregate(new FList<AStarNormal<GOAPState>.Arc>(), (possibleList, action) => {
                        var newState = new GOAPState(curr);
                        newState.values.UpdateWith(action.effects);
                        newState.generatingAction = action;
                        newState.step = curr.step + 1;
                        return possibleList + new AStarNormal<GOAPState>.Arc(newState, action.cost);
                    });
            });

        if (seq == null) {
            Debug.Log("Imposible planear");
            return null;
        }

        foreach (var act in seq.Skip(1)) {
            Debug.Log(act);
        }

        Debug.Log("WATCHDOG " + watchdog);

        return seq.Skip(1).Select(x => x.generatingAction);
    }

    void Start() {
        var actions = new List<GOAPAction>() {
            new GOAPAction("BuildHouse")
                .Pre("hasWood", true)
                .Pre("hasHammer", true)
                .Effect("houseBuilt", true),

            new GOAPAction("CollectHammer")
                .Effect("hasHammer", true),

            new GOAPAction("CollectAxe")
                .Effect("hasAxe", true)
                .Cost(10f),

            new GOAPAction("CollectCheapAxe")
                .Effect("hasAxe", true)
                .Effect("backPain", true)
                .Cost(2f),

            new GOAPAction("ChopWood")
                .Pre("hasAxe", true)
                .Effect("hasWood", true),

            new GOAPAction("UseMedicine")
                .Effect("backPain", false)
                .Cost(100f)
        };
        var from = new GOAPState();
        from.values["backPain"] = false;

        var to = new GOAPState();
        to.values["houseBuilt"] = true;
        to.values["backPain"] = false;

        GoapRun(from, to, actions);
    }
}