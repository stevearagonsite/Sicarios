﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = Logger.Debug;

/*
Modo psicopata = Menos costo matar
	Se puede "patchear" in game!
*/

public class GoapMiniTest : MonoBehaviour {
    const string VAR_GUARDS_AMOUNT = "guardsAmount";
    const string VAR_TOTAL_GUARDS = "totalGuards";
    const string VAR_VISIBILITY = "visibility";
    const string VAR_HP = "hp";
    const string VAR_TARGET_LOCATION = "targetLocation";
    const string VAR_STUDY_LOCATION = "studyLocation";
    const string VAR_TOTAL_LOCATIONS = "totalLocations";
    const string VAR_WEAPON = "weapon";
    const string VAR_MONEY = "money";
    
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

        Debug.LogColor("PLAN", "green");
        if (seq == null) {
            Debug.LogError("impossible to plan");
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
            Debug.Log(act + "----------------------------" + stringValues);
        }

        Debug.LogColor("WATCHDOG " + watchdog, "green");

        return seq.Skip(1).Select(x => x.generatingAction);
    }


    public static IEnumerable<GOAPActionDelegate> GoapRunDelegate(GOAPStateDelegate from, GOAPStateDelegate to,
        IEnumerable<GOAPActionDelegate> actions) {
        int watchdog = 300;
        Func<GOAPStateDelegate, GOAPStateDelegate, float> heuristic = (curr, goal) => {
            return goal.values.Count(goalKv => goalKv.Value != curr.values[goalKv.Key]);
        };

        var seq = AStarNormal<GOAPStateDelegate>.Run(
            from,
            to,
            heuristic,
            curr => to.values.All(kv => kv.In(curr.values)),
            curr => {
                if (watchdog == 0) return Enumerable.Empty<AStarNormal<GOAPStateDelegate>.Arc>();

                watchdog--;

                return actions
                    .Where(action => action.ValidatePreconditions(curr))
                    .Aggregate(new FList<AStarNormal<GOAPStateDelegate>.Arc>(), (possibleList, action) => {
                        var newState = new GOAPStateDelegate(curr);
                        newState.ApplyEffects(action.effects);
                        newState.generatingAction = action;
                        newState.step = curr.step + 1;
                        return possibleList + new AStarNormal<GOAPStateDelegate>.Arc(newState, action.cost);
                    });
            });

        Debug.LogColor("PLAN", "green");
        Debug.LogColor("watchdog: " + watchdog, "green");
        if (seq == null) {
            Debug.LogError("impossible to plan");
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
            Debug.Log(act + "----------------------------" + stringValues);
        }

        Debug.LogColor("WATCHDOG " + watchdog, "green");

        return seq.Skip(1).Select(x => x.generatingAction);
    }

    void Start() {
        Debug.LogColor("START", "green");
        // ExampleGOAP01(out var actions,out var from,out var to);
        // ExampleGOAP02(out var actions, out var from, out var to);
        // var plan = GoapRun(from, to, actions);


        ExampleGOAP03(out var actions, out var from, out var to);
        var plan = GoapRunDelegate(from, to, actions);
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

    void ExampleGOAP03(out List<GOAPActionDelegate> actions, out GOAPStateDelegate from, out GOAPStateDelegate to) {
        from = new GOAPStateDelegate();
        from.SetValue(VAR_GUARDS_AMOUNT, 20);
        from.SetValue(VAR_TOTAL_GUARDS, 20);
        from.SetValue(VAR_VISIBILITY, 0f);
        from.SetValue(VAR_HP, 100f);
        from.SetValue(VAR_TARGET_LOCATION, false);
        from.SetValue(VAR_STUDY_LOCATION, 0);
        from.SetValue(VAR_TOTAL_LOCATIONS, 5);
        from.SetValue(VAR_WEAPON, "none");
        from.SetValue(VAR_MONEY, 0);

        actions = new List<GOAPActionDelegate>() {
            new GOAPActionDelegate("RestInTheCommune")
                .Pre(VAR_VISIBILITY,gs => gs.GetValueFloat(VAR_VISIBILITY) > 0f)
                .Effect(VAR_VISIBILITY, gs => gs.SetValueFloat(VAR_VISIBILITY, 0f))
                .Effect(VAR_HP, gs => gs.SetValueFloat(VAR_HP, 100F))
                .Cost(2),

            new GOAPActionDelegate("StudyLocations")
                .Pre(VAR_TARGET_LOCATION, gs => gs.GetValueBool(VAR_TARGET_LOCATION))
                .Effect(VAR_STUDY_LOCATION, gs => {
                    gs = gs.SetValueInt(VAR_STUDY_LOCATION, 1 + gs.GetValueInt(VAR_STUDY_LOCATION));
                    return gs.GetValueInt(VAR_STUDY_LOCATION) >= 1 ? gs.SetValueBool(VAR_TARGET_LOCATION, true) : gs;
                }),

            new GOAPActionDelegate("StealMoney")
                .Pre(VAR_VISIBILITY, gs => gs.GetValueFloat(VAR_VISIBILITY) < 60)
                .Effect(VAR_MONEY, gs => gs.SetValueInt(VAR_MONEY, 100 + gs.GetValueInt(VAR_MONEY)))
                .Effect(VAR_VISIBILITY, gs => gs.SetValue(VAR_VISIBILITY, 10f + gs.GetValueFloat(VAR_VISIBILITY)))
                .Cost(2),

            new GOAPActionDelegate("KillGuardGun")
                .Pre(VAR_VISIBILITY,gs => gs.GetValueFloat(VAR_VISIBILITY) < 100)
                .Pre(VAR_WEAPON,gs => gs.GetValueString(VAR_WEAPON) == "gun")
                .Pre(VAR_HP,gs => gs.GetValueFloat(VAR_HP) > 40f)
                .Effect(VAR_HP, gs => gs.SetValueFloat(VAR_HP, gs.GetValueFloat(VAR_HP) - 1f))
                .Effect(VAR_VISIBILITY, gs => gs.SetValueFloat(VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(VAR_VISIBILITY) + 10)))
                .Effect(VAR_GUARDS_AMOUNT, gs => gs.SetValueInt(VAR_GUARDS_AMOUNT, gs.GetValueInt(VAR_GUARDS_AMOUNT) - 1))
                .Cost(3),

            new GOAPActionDelegate("KillGuardSniper")
                .Pre(VAR_VISIBILITY,gs => gs.GetValueFloat(VAR_VISIBILITY) < 50)
                .Pre(VAR_WEAPON,gs => gs.GetValueString(VAR_WEAPON) == "sniper")
                .Pre(VAR_HP,gs => gs.GetValueFloat(VAR_HP) > 80f)
                .Effect(VAR_VISIBILITY, gs => gs.SetValueFloat(VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(VAR_VISIBILITY) + 10)))
                .Effect(VAR_GUARDS_AMOUNT, gs => gs.SetValueInt(VAR_GUARDS_AMOUNT, gs.GetValueInt(VAR_GUARDS_AMOUNT) - 1))
                .Cost(6),

            new GOAPActionDelegate("KillTargetGun")
                .Pre(VAR_GUARDS_AMOUNT,gs => gs.GetValueInt(VAR_TOTAL_GUARDS) * 0.5f < gs.GetValueInt(VAR_GUARDS_AMOUNT))
                .Pre(VAR_VISIBILITY,gs => gs.GetValueFloat(VAR_VISIBILITY) < 30)
                .Pre(VAR_WEAPON,gs => gs.GetValueString(VAR_WEAPON) == "gun")
                .Pre(VAR_HP,gs => gs.GetValueFloat(VAR_HP) > 99)
                .Effect(VAR_HP, gs => gs.SetValueFloat(VAR_HP, gs.GetValueFloat(VAR_HP) - 5f))
                .Effect(VAR_VISIBILITY, gs => gs.SetValueFloat(VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(VAR_VISIBILITY) + 50)))
                .Cost(3),

            new GOAPActionDelegate("KillTargetSniper")
                .Pre(VAR_GUARDS_AMOUNT,gs => gs.GetValueInt(VAR_TOTAL_GUARDS) * 0.5f < gs.GetValueInt(VAR_GUARDS_AMOUNT))
                .Pre(VAR_VISIBILITY,gs => gs.GetValueFloat(VAR_VISIBILITY) < 10)
                .Pre(VAR_WEAPON,gs => gs.GetValueString(VAR_WEAPON) == "sniper")
                .Pre(VAR_HP,gs => gs.GetValueFloat(VAR_HP) > 99)
                .Effect(VAR_VISIBILITY, gs => gs.SetValueFloat(VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(VAR_VISIBILITY) + 10)))
                .Cost(3),

            new GOAPActionDelegate("BuyGun")
                .Pre(VAR_VISIBILITY,gs => gs.GetValueFloat(VAR_VISIBILITY) < 100)
                .Pre(VAR_WEAPON,gs => gs.GetValueString(VAR_WEAPON) != "gun")
                .Pre(VAR_MONEY,gs => gs.GetValueInt(VAR_MONEY) >= 100)
                .Effect(VAR_MONEY, gs => gs.SetValueInt(VAR_MONEY, gs.GetValueInt(VAR_MONEY) - 100))
                .Effect(VAR_WEAPON, gs => gs.SetValueString(VAR_WEAPON, "gun"))
                .Cost(3),

            new GOAPActionDelegate("BuySniper")
                .Pre(VAR_VISIBILITY,gs => gs.GetValueFloat(VAR_VISIBILITY) < 100)
                .Pre(VAR_WEAPON,gs => gs.GetValueString(VAR_WEAPON) != "sniper")
                .Pre(VAR_MONEY,gs => gs.GetValueInt(VAR_MONEY) >= 200)
                .Effect(VAR_MONEY, gs => gs.SetValueInt(VAR_MONEY, gs.GetValueInt(VAR_MONEY) - 200))
                .Effect(VAR_WEAPON, gs => gs.SetValueString(VAR_WEAPON, "sniper"))
                .Cost(3),
        };

        to = new GOAPStateDelegate();
        to.SetValue(VAR_VISIBILITY, 0f);
        to.SetValue(VAR_HP, 100f);
        // to.SetValue(VAR_TARGET_LOCATION, true);
        to.SetValue(VAR_WEAPON, "sniper");
        to.SetValue(VAR_MONEY, 300);
    }
}