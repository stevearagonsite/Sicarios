using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEditor.Experimental.GraphView;

/*
Modo psicopata = Menos costo matar
	Se puede "patchear" in game!
*/

public class GoapMiniTest : MonoBehaviour {
    public static IEnumerable<GOAPAction> GoapRun(GOAPState from, GOAPState to, IEnumerable<GOAPAction> actions) {
        int watchdog = 200;
        Func<GOAPState, GOAPState, float> heuristic = (curr, goal) => goal.values.Count(kv => !kv.In(curr.values));

        var seq = AStarNormal<GOAPState>.Run(
            from,
            to,
            heuristic,
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

        Logger.Debug.LogColor("PLAN", "green");
        if (seq == null) {
            Logger.Debug.LogError("impossible to plan");
            return null;
        }

        var costSoFar = 0f;
        foreach (var act in seq.Skip(1)) {
            var heuristicValue = heuristic(act, to);
            var dijkstraValue = act.generatingAction.cost;
            costSoFar += dijkstraValue + heuristicValue;
            var stringValues = "\n" + "(CostSoFar: " + costSoFar + ")" + "\n" +
                               "(Heuristic: " + heuristicValue + ")" + "\n" +
                               "(Dijkstra: " + dijkstraValue + ")" + "\n" +
                               "(Step: " + act.step + ")";
            Logger.Debug.Log(act + "----------------------------" + stringValues);
        }

        Logger.Debug.LogColor("WATCHDOG " + watchdog, "green");

        return seq.Skip(1).Select(x => x.generatingAction);
    }

    void Start() {
        Logger.Debug.LogColor("START", "green");
        // ExampleGOAP01(out var actions,out var from,out var to);
        ExampleGOAP02(out var actions, out var from, out var to);
        var plan = GoapRun(from, to, actions);
    }

    void ExampleGOAP01(out List<GOAPAction> actions, out GOAPState from, out GOAPState to) {
        actions = new List<GOAPAction>() {
            new GOAPAction("BuildHouse")
                .Pre("hasWood", true)
                .Pre("hasAxe", true)
                .Pre("backPain", false)
                .Pre("hasHammer", true)
                .Effect("houseBuilt", true),

            new GOAPAction("CollectHammer")
                .Pre("hasWood", true)
                .Effect("hasHammer", true),

            new GOAPAction("CollectAxe")
                .Pre("hasAxe", false)
                .Effect("hasAxe", true)
                .Cost(10f),

            new GOAPAction("CollectCheapAxe")
                .Pre("hasAxe", false)
                .Effect("hasAxe", true)
                .Effect("backPain", true)
                .Cost(2f),

            new GOAPAction("ChopWood")
                .Pre("hasAxe", true)
                .Effect("hasWood", true),

            new GOAPAction("UseMedicine")
                .Pre("backPain", true)
                .Effect("backPain", false)
                .Cost(4f)
        };
        from = new GOAPState();
        from.values["houseBuilt"] = false;
        from.values["hasAxe"] = false;
        from.values["hasHammer"] = false;
        from.values["backPain"] = false;

        to = new GOAPState();
        to.values["houseBuilt"] = true;
        to.values["hasAxe"] = true;
        to.values["hasHammer"] = true;
        to.values["backPain"] = false;
    }

    void ExampleGOAP02(out List<GOAPAction> actions, out GOAPState from, out GOAPState to) {
        const string VAR_ALERT = "alert";
        const string VAR_AMMON = "ammon";
        const string VAR_COSTUME = "costume";
        const string VAR_CASH = "cash";
        const string VAR_INJURED = "injured";
        const string VAR_OBJECTIVE_DELETED = "objectiveDeleted";

        actions = new List<GOAPAction>() {
            new GOAPAction("LetTheTimePass")
                .Pre(VAR_ALERT, true)
                .Effect(VAR_ALERT, false)
                .Effect(VAR_INJURED, false),

            new GOAPAction("Attack")
                .Pre(VAR_AMMON, true)
                .Pre(VAR_INJURED, false)
                .Effect(VAR_AMMON, false)
                .Effect(VAR_COSTUME, false)
                .Effect(VAR_OBJECTIVE_DELETED, true)
                .Effect(VAR_ALERT, true)
                .Effect(VAR_INJURED, true)
                .Cost(6),

            new GOAPAction("BuyAmmon")
                .Pre(VAR_AMMON, false)
                .Pre(VAR_CASH, true)
                .Effect(VAR_AMMON, true)
                .Effect(VAR_CASH, false),

            new GOAPAction("KillGuard")
                .Pre(VAR_COSTUME, false)
                .Effect(VAR_COSTUME, true)
                .Effect(VAR_ALERT, true)
                .Effect(VAR_AMMON, true)
                .Cost(4),

            new GOAPAction("BribeGuard")
                .Pre(VAR_CASH, true)
                .Pre(VAR_COSTUME, false)
                .Pre(VAR_ALERT, false)
                .Effect(VAR_COSTUME, true)
                .Effect(VAR_CASH, false)
                .Cost(2),

            new GOAPAction("Infiltrate")
                .Pre(VAR_COSTUME, true)
                .Pre(VAR_ALERT, false)
                .Effect(VAR_OBJECTIVE_DELETED, true)
                .Effect(VAR_ALERT, true)
                .Cost(2),

            new GOAPAction("useSniper")
                .Pre(VAR_AMMON, true)
                .Pre(VAR_ALERT, false)
                .Effect(VAR_AMMON, false)
                .Effect(VAR_OBJECTIVE_DELETED, true)
                .Cost(7),
        };

        from = new GOAPState();
        from.values[VAR_ALERT] = false;
        from.values[VAR_AMMON] = true;
        from.values[VAR_COSTUME] = false;
        from.values[VAR_CASH] = true;
        from.values[VAR_INJURED] = false;
        from.values[VAR_OBJECTIVE_DELETED] = false;

        to = new GOAPState();
        to.values[VAR_ALERT] = false;
        to.values[VAR_INJURED] = false;
        to.values[VAR_OBJECTIVE_DELETED] = true;
    }
}