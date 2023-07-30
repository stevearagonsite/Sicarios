using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logger;
using GoapA = AgentSicarioConstants.AgentSicarioActions;
using GoapV = AgentSicarioConstants.AgentSicarioGoapVariables;

public partial class AgentSicario {
    private void CreatePlan(out List<GOAPActionDelegate> actions, out GOAPStateDelegate from,
        out GOAPStateDelegate to) {
        from = new GOAPStateDelegate();
        from.SetValue(GoapV.VAR_GUARDS_AMOUNT, 20);
        from.SetValue(GoapV.VAR_TOTAL_GUARDS, 20);
        from.SetValue(GoapV.VAR_VISIBILITY, 0f);
        from.SetValue(GoapV.VAR_HP, 100f);
        from.SetValue(GoapV.VAR_TARGET_LOCATION, false);
        from.SetValue(GoapV.VAR_DEAD_TARGET, false);
        from.SetValue(GoapV.VAR_STUDY_LOCATION, 0);
        from.SetValue(GoapV.VAR_TOTAL_LOCATIONS, 5);
        from.SetValue(GoapV.VAR_WEAPON, "none");
        from.SetValue(GoapV.VAR_MONEY, 0);

        const int VALUE_SHIPPER = 100;
        const int VALUE_GUN = 100;
        const int VALUE_STUDY = 1;
        const string WEAPON_GUN = "gun";
        const string WEAPON_SNIPER = "sniper";

        actions = new List<GOAPActionDelegate>() {
            new GOAPActionDelegate(GoapA.ACTION_REST_IN_THE_COMMUNE)
                .Pre(GoapV.VAR_VISIBILITY, gs => gs.GetValueFloat(GoapV.VAR_VISIBILITY) > 0f)
                .Effect(GoapV.VAR_VISIBILITY, gs => gs.SetValueFloat(GoapV.VAR_VISIBILITY, 0f))
                .Effect(GoapV.VAR_HP, gs => gs.SetValueFloat(GoapV.VAR_HP, 100F)),

            new GOAPActionDelegate(GoapA.ACTION_STUDY_LOCATIONS)
                .Pre(GoapV.VAR_TARGET_LOCATION, gs => !gs.GetValueBool(GoapV.VAR_TARGET_LOCATION))
                .Effect(GoapV.VAR_STUDY_LOCATION, gs => {
                    gs = gs.SetValueInt(GoapV. VAR_STUDY_LOCATION, 1 + gs.GetValueInt(GoapV. VAR_STUDY_LOCATION));
                    if (gs.GetValueInt(GoapV. VAR_STUDY_LOCATION) < VALUE_STUDY) return gs;
                    gs = gs.SetValueBool(GoapV.VAR_TARGET_LOCATION, true);
                    return gs.SetValueInt(GoapV. VAR_STUDY_LOCATION, 0);
                }),

            new GOAPActionDelegate(GoapA.ACTION_STEAL_MONEY)
                .Pre(GoapV.VAR_VISIBILITY, gs => gs.GetValueFloat(GoapV.VAR_VISIBILITY) < 60)
                .Effect(GoapV.VAR_MONEY, gs => gs.SetValueInt(GoapV.VAR_MONEY, 1000 + gs.GetValueInt(GoapV.VAR_MONEY)))
                .Effect(GoapV.VAR_VISIBILITY,
                    gs => gs.SetValue(GoapV.VAR_VISIBILITY, 10f + gs.GetValueFloat(GoapV.VAR_VISIBILITY)))
                .Cost(2),

            new GOAPActionDelegate(GoapA.ACTION_KILL_GUARD_GUN)
                .Pre(GoapV.VAR_VISIBILITY, gs => gs.GetValueFloat(GoapV.VAR_VISIBILITY) < 100)
                .Pre(GoapV.VAR_WEAPON, gs => gs.GetValueString(GoapV.VAR_WEAPON) == WEAPON_GUN)
                .Pre(GoapV.VAR_HP, gs => gs.GetValueFloat(GoapV.VAR_HP) > 40f)
                .Effect(GoapV.VAR_HP, gs => gs.SetValueFloat(GoapV.VAR_HP, gs.GetValueFloat(GoapV.VAR_HP) - 1f))
                .Effect(GoapV.VAR_VISIBILITY,
                    gs => gs.SetValueFloat(GoapV.VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(GoapV.VAR_VISIBILITY) + 10)))
                .Effect(GoapV.VAR_GUARDS_AMOUNT,
                    gs => gs.SetValueInt(GoapV.VAR_GUARDS_AMOUNT, gs.GetValueInt(GoapV.VAR_GUARDS_AMOUNT) - 1))
                .Cost(3),

            new GOAPActionDelegate(GoapA.ACTION_KILL_GUARD_SNIPER)
                .Pre(GoapV.VAR_WEAPON, gs => gs.GetValueString(GoapV.VAR_WEAPON) == WEAPON_SNIPER)
                .Pre(GoapV.VAR_HP, gs => gs.GetValueFloat(GoapV.VAR_HP) > 10f)
                .Effect(GoapV.VAR_VISIBILITY,
                    gs => gs.SetValueFloat(GoapV.VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(GoapV.VAR_VISIBILITY) + 10)))
                .Effect(GoapV.VAR_GUARDS_AMOUNT,
                    gs => gs.SetValueInt(GoapV.VAR_GUARDS_AMOUNT, gs.GetValueInt(GoapV.VAR_GUARDS_AMOUNT) - 1))
                .Cost(6),

            new GOAPActionDelegate(GoapA.ACTION_KILL_TARGET_GUN)
                .Pre(GoapV.VAR_GUARDS_AMOUNT,
                    gs => gs.GetValueInt(GoapV.VAR_TOTAL_GUARDS) * 0.5f < gs.GetValueInt(GoapV.VAR_GUARDS_AMOUNT))
                .Pre(GoapV.VAR_VISIBILITY, gs => gs.GetValueFloat(GoapV.VAR_VISIBILITY) < 30)
                .Pre(GoapV.VAR_WEAPON, gs => gs.GetValueString(GoapV.VAR_WEAPON) == WEAPON_GUN)
                .Pre(GoapV.VAR_HP, gs => gs.GetValueFloat(GoapV.VAR_HP) > 99)
                .Pre(GoapV.VAR_TARGET_LOCATION, gs => gs.GetValueBool(GoapV.VAR_TARGET_LOCATION))
                .Effect(GoapV.VAR_HP, gs => gs.SetValueFloat(GoapV.VAR_HP, gs.GetValueFloat(GoapV.VAR_HP) - 5f))
                .Effect(GoapV.VAR_VISIBILITY,
                    gs => gs.SetValueFloat(GoapV.VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(GoapV.VAR_VISIBILITY) + 50)))
                .Effect(GoapV.VAR_DEAD_TARGET, gs => gs.SetValueBool(GoapV.VAR_DEAD_TARGET, true))
                .Cost(3),

            new GOAPActionDelegate(GoapA.ACTION_KILL_TARGET_SNIPER)
                .Pre(GoapV.VAR_GUARDS_AMOUNT,
                    gs => gs.GetValueInt(GoapV.VAR_TOTAL_GUARDS) * 0.5f < gs.GetValueInt(GoapV.VAR_GUARDS_AMOUNT))
                .Pre(GoapV.VAR_WEAPON, gs => gs.GetValueString(GoapV.VAR_WEAPON) == WEAPON_SNIPER)
                .Pre(GoapV.VAR_HP, gs => gs.GetValueFloat(GoapV.VAR_HP) > 10)
                .Pre(GoapV.VAR_TARGET_LOCATION, gs => gs.GetValueBool(GoapV.VAR_TARGET_LOCATION))
                .Effect(GoapV.VAR_DEAD_TARGET, gs => gs.SetValueBool(GoapV.VAR_DEAD_TARGET, true))
                .Cost(3),

            new GOAPActionDelegate(GoapA.ACTION_BUY_GUN)
                .Pre(GoapV.VAR_WEAPON, gs => gs.GetValueString(GoapV.VAR_WEAPON) != WEAPON_GUN)
                .Pre(GoapV.VAR_MONEY, gs => gs.GetValueInt(GoapV.VAR_MONEY) >= VALUE_GUN)
                .Effect(GoapV.VAR_MONEY,
                    gs => gs.SetValueInt(GoapV.VAR_MONEY, gs.GetValueInt(GoapV.VAR_MONEY) - VALUE_GUN))
                .Effect(GoapV.VAR_WEAPON, gs => gs.SetValueString(GoapV.VAR_WEAPON, WEAPON_GUN))
                .Cost(3),

            new GOAPActionDelegate(GoapA.ACTION_BUY_SNIPER)
                .Pre(GoapV.VAR_WEAPON, gs => gs.GetValueString(GoapV.VAR_WEAPON) != WEAPON_SNIPER)
                .Pre(GoapV.VAR_MONEY, gs => gs.GetValueInt(GoapV.VAR_MONEY) >= VALUE_SHIPPER)
                .Effect(GoapV.VAR_MONEY,
                    gs => gs.SetValueInt(GoapV.VAR_MONEY, gs.GetValueInt(GoapV.VAR_MONEY) - VALUE_SHIPPER))
                .Effect(GoapV.VAR_WEAPON, gs => gs.SetValueString(GoapV.VAR_WEAPON, WEAPON_SNIPER))
                .Cost(3),
        };

        to = new GOAPStateDelegate();
        to.SetCaseFromValue(GoapV.VAR_HP, (gs) => gs.GetValueFloat(GoapV.VAR_HP) >= 100f);
        to.SetCaseFromValue(GoapV.VAR_VISIBILITY, (gs) => gs.GetValueFloat(GoapV.VAR_VISIBILITY) == 0f);
        to.SetCaseFromValue(GoapV.VAR_MONEY, (gs) => gs.GetValueInt(GoapV.VAR_MONEY) >= 500);
        to.SetCaseFromValue(GoapV.VAR_WEAPON, (gs) => gs.GetValueString(GoapV.VAR_WEAPON) == WEAPON_SNIPER);
        to.SetCaseFromValue(GoapV.VAR_TARGET_LOCATION, (gs) => gs.GetValueBool(GoapV.VAR_TARGET_LOCATION));
        to.SetCaseFromValue(GoapV.VAR_DEAD_TARGET, (gs) => gs.GetValueBool(GoapV.VAR_DEAD_TARGET));
    }
}