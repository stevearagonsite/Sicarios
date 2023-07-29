using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Logger;

public partial class AgentSicario{
    const string ACTION_REST_IN_THE_COMMUNE = "RestInTheCommune";
    const string ActionStudyLocations = "RestInTheCommune";
    private void CreatePlan(out List<GOAPActionDelegate> actions, out GOAPStateDelegate from, out GOAPStateDelegate to) {
        const string VAR_GUARDS_AMOUNT = "guardsAmount";
        const string VAR_TOTAL_GUARDS = "totalGuards";
        const string VAR_VISIBILITY = "visibility";
        const string VAR_HP = "hp";
        const string VAR_TARGET_LOCATION = "targetLocation";
        const string VAR_STUDY_LOCATION = "studyLocation";
        const string VAR_TOTAL_LOCATIONS = "totalLocations";
        const string VAR_DEAD_TARGET = "deadTarget";
        const string VAR_WEAPON = "weapon";
        const string VAR_MONEY = "money";

        from = new GOAPStateDelegate();
        from.SetValue(VAR_GUARDS_AMOUNT, 20);
        from.SetValue(VAR_TOTAL_GUARDS, 20);
        from.SetValue(VAR_VISIBILITY, 0f);
        from.SetValue(VAR_HP, 100f);
        from.SetValue(VAR_TARGET_LOCATION, false);
        from.SetValue(VAR_DEAD_TARGET, false);
        from.SetValue(VAR_STUDY_LOCATION, 0);
        from.SetValue(VAR_TOTAL_LOCATIONS, 5);
        from.SetValue(VAR_WEAPON, "none");
        from.SetValue(VAR_MONEY, 0);

        const int VALUE_SHIPPER = 100;
        const int VALUE_GUN = 100;
        const int VALUE_STUDY = 1;

        actions = new List<GOAPActionDelegate>() {
            new GOAPActionDelegate("RestInTheCommune")
                .Pre(VAR_VISIBILITY, gs => gs.GetValueFloat(VAR_VISIBILITY) > 0f)
                .Effect(VAR_VISIBILITY, gs => gs.SetValueFloat(VAR_VISIBILITY, 0f))
                .Effect(VAR_HP, gs => gs.SetValueFloat(VAR_HP, 100F)),

            new GOAPActionDelegate("StudyLocations")
                .Pre(VAR_TARGET_LOCATION, gs => !gs.GetValueBool(VAR_TARGET_LOCATION))
                .Effect(VAR_STUDY_LOCATION, gs => {
                    // gs = gs.SetValueInt(VAR_STUDY_LOCATION, 1 + gs.GetValueInt(VAR_STUDY_LOCATION));
                    target = GameManager.instance.locations[0];
                    // gs = gs.SetValueBool(VAR_TARGET_LOCATION, true);
                    return gs.SetValueInt(VAR_STUDY_LOCATION, 0);
                }),

            new GOAPActionDelegate("StealMoney")
                .Pre(VAR_VISIBILITY, gs => gs.GetValueFloat(VAR_VISIBILITY) < 60)
                .Effect(VAR_MONEY, gs => gs.SetValueInt(VAR_MONEY, 1000 + gs.GetValueInt(VAR_MONEY)))
                .Effect(VAR_VISIBILITY, gs => gs.SetValue(VAR_VISIBILITY, 10f + gs.GetValueFloat(VAR_VISIBILITY)))
                .Cost(2),

            new GOAPActionDelegate("KillGuardGun")
                .Pre(VAR_VISIBILITY, gs => gs.GetValueFloat(VAR_VISIBILITY) < 100)
                .Pre(VAR_WEAPON, gs => gs.GetValueString(VAR_WEAPON) == "gun")
                .Pre(VAR_HP, gs => gs.GetValueFloat(VAR_HP) > 40f)
                .Effect(VAR_HP, gs => gs.SetValueFloat(VAR_HP, gs.GetValueFloat(VAR_HP) - 1f))
                .Effect(VAR_VISIBILITY,
                    gs => gs.SetValueFloat(VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(VAR_VISIBILITY) + 10)))
                .Effect(VAR_GUARDS_AMOUNT,
                    gs => gs.SetValueInt(VAR_GUARDS_AMOUNT, gs.GetValueInt(VAR_GUARDS_AMOUNT) - 1))
                .Cost(3),

            new GOAPActionDelegate("KillGuardSniper")
                .Pre(VAR_WEAPON, gs => gs.GetValueString(VAR_WEAPON) == "sniper")
                .Pre(VAR_HP, gs => gs.GetValueFloat(VAR_HP) > 10f)
                .Effect(VAR_VISIBILITY,
                    gs => gs.SetValueFloat(VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(VAR_VISIBILITY) + 10)))
                .Effect(VAR_GUARDS_AMOUNT,
                    gs => gs.SetValueInt(VAR_GUARDS_AMOUNT, gs.GetValueInt(VAR_GUARDS_AMOUNT) - 1))
                .Cost(6),

            new GOAPActionDelegate("KillTargetGun")
                .Pre(VAR_GUARDS_AMOUNT,
                    gs => gs.GetValueInt(VAR_TOTAL_GUARDS) * 0.5f < gs.GetValueInt(VAR_GUARDS_AMOUNT))
                .Pre(VAR_VISIBILITY, gs => gs.GetValueFloat(VAR_VISIBILITY) < 30)
                .Pre(VAR_WEAPON, gs => gs.GetValueString(VAR_WEAPON) == "gun")
                .Pre(VAR_HP, gs => gs.GetValueFloat(VAR_HP) > 99)
                .Pre(VAR_TARGET_LOCATION, gs => gs.GetValueBool(VAR_TARGET_LOCATION))
                .Effect(VAR_HP, gs => gs.SetValueFloat(VAR_HP, gs.GetValueFloat(VAR_HP) - 5f))
                .Effect(VAR_VISIBILITY,
                    gs => gs.SetValueFloat(VAR_VISIBILITY, Math.Abs(gs.GetValueFloat(VAR_VISIBILITY) + 50)))
                .Effect(VAR_DEAD_TARGET, gs => gs.SetValueBool(VAR_DEAD_TARGET, true))
                .Cost(3),

            new GOAPActionDelegate("KillTargetSniper")
                .Pre(VAR_GUARDS_AMOUNT,
                    gs => gs.GetValueInt(VAR_TOTAL_GUARDS) * 0.5f < gs.GetValueInt(VAR_GUARDS_AMOUNT))
                .Pre(VAR_WEAPON, gs => gs.GetValueString(VAR_WEAPON) == "sniper")
                .Pre(VAR_HP, gs => gs.GetValueFloat(VAR_HP) > 10)
                .Pre(VAR_TARGET_LOCATION, gs => gs.GetValueBool(VAR_TARGET_LOCATION))
                .Effect(VAR_DEAD_TARGET, gs => gs.SetValueBool(VAR_DEAD_TARGET, true))
                .Cost(3),

            new GOAPActionDelegate("BuyGun")
                .Pre(VAR_WEAPON, gs => gs.GetValueString(VAR_WEAPON) != "gun")
                .Pre(VAR_MONEY, gs => gs.GetValueInt(VAR_MONEY) >= VALUE_GUN)
                .Effect(VAR_MONEY, gs => gs.SetValueInt(VAR_MONEY, gs.GetValueInt(VAR_MONEY) - VALUE_GUN))
                .Effect(VAR_WEAPON, gs => gs.SetValueString(VAR_WEAPON, "gun"))
                .Cost(3),

            new GOAPActionDelegate("BuySniper")
                .Pre(VAR_WEAPON, gs => gs.GetValueString(VAR_WEAPON) != "sniper")
                .Pre(VAR_MONEY, gs => gs.GetValueInt(VAR_MONEY) >= VALUE_SHIPPER)
                .Effect(VAR_MONEY, gs => gs.SetValueInt(VAR_MONEY, gs.GetValueInt(VAR_MONEY) - VALUE_SHIPPER))
                .Effect(VAR_WEAPON, gs => gs.SetValueString(VAR_WEAPON, "sniper"))
                .Cost(3),
        };

        to = new GOAPStateDelegate();
        to.SetCaseFromValue(VAR_HP, (gs) => gs.GetValueFloat(VAR_HP) >= 100f);
        to.SetCaseFromValue(VAR_VISIBILITY, (gs) => gs.GetValueFloat(VAR_VISIBILITY) == 0f);
        to.SetCaseFromValue(VAR_MONEY, (gs) => gs.GetValueInt(VAR_MONEY) >= 500);
        to.SetCaseFromValue(VAR_WEAPON, (gs) => gs.GetValueString(VAR_WEAPON) == "sniper");
        to.SetCaseFromValue(VAR_TARGET_LOCATION, (gs) => gs.GetValueBool(VAR_TARGET_LOCATION));
        to.SetCaseFromValue(VAR_DEAD_TARGET, (gs) => gs.GetValueBool(VAR_DEAD_TARGET));
    }

}
