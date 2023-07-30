using System.Collections.Generic;
using ItemsConstants = Items.ItemConstants;

namespace AgentSicarioConstants {
    static class AgentSicarioActions {
        public const string ACTION_REST_IN_THE_COMMUNE = "RestInTheCommune";
        public const string ACTION_STUDY_LOCATIONS = "StudyLocations";
        public const string ACTION_STEAL_MONEY = "StealMoney";
        public const string ACTION_KILL_GUARD_GUN = "KillGuardGun";
        public const string ACTION_KILL_GUARD_SNIPER = "KillGuardSniper";
        public const string ACTION_KILL_TARGET_GUN = "KillTargetGun";
        public const string ACTION_KILL_TARGET_SNIPER = "KillTargetSniper";
        public const string ACTION_BUY_GUN = "BuyGun";
        public const string ACTION_BUY_SNIPER = "BuySniper";
    }

    static class AgentSicarioStates {
        public const string IDLE = "Idle";
        public const string PURSUIT = "Pursuit";
        public const string KILL = "Kill";
        public const string DEAD = "Dead";
        public const string EVADE = "Evade";
        public const string GO_TO = "GoTo";
        public const string PICK_UP = "pickUp";
        public const string PLAN_STEP = "PlanStep";
        public const string PLAN_FAIL = "PlanFail";
        public const string PLAN_SUCCESS = "Success";
    }

    static class AgentSicarioGoapVariables {
        public const string VAR_GUARDS_AMOUNT = "guardsAmount";
        public const string VAR_TOTAL_GUARDS = "totalGuards";
        public const string VAR_VISIBILITY = "visibility";
        public const string VAR_HP = "hp";
        public const string VAR_TARGET_LOCATION = "targetLocation";
        public const string VAR_STUDY_LOCATION = "studyLocation";
        public const string VAR_TOTAL_LOCATIONS = "totalLocations";
        public const string VAR_DEAD_TARGET = "deadTarget";
        public const string VAR_WEAPON = "weapon";
        public const string VAR_MONEY = "money";
    }

    static class AgentSicarioDictionaries {
        public static readonly Dictionary<string, string> ActionToItem = new() {
            { AgentSicarioActions.ACTION_REST_IN_THE_COMMUNE, ItemsConstants.ITEM_TYPE_COMMUNE },
            { AgentSicarioActions.ACTION_STUDY_LOCATIONS, ItemsConstants.ITEM_TYPE_LOCATION },
            { AgentSicarioActions.ACTION_STEAL_MONEY, ItemsConstants.ITEM_TYPE_MONEY },
            { AgentSicarioActions.ACTION_KILL_GUARD_GUN, ItemsConstants.ITEM_TYPE_GUARD },
            { AgentSicarioActions.ACTION_KILL_GUARD_SNIPER, ItemsConstants.ITEM_TYPE_GUARD },
            { AgentSicarioActions.ACTION_KILL_TARGET_GUN, ItemsConstants.ITEM_TYPE_TARGET },
            { AgentSicarioActions.ACTION_KILL_TARGET_SNIPER, ItemsConstants.ITEM_TYPE_TARGET },
            { AgentSicarioActions.ACTION_BUY_GUN, ItemsConstants.ITEM_TYPE_STORE },
            { AgentSicarioActions.ACTION_BUY_SNIPER, ItemsConstants.ITEM_TYPE_STORE },
        };

        public static readonly Dictionary<string, string> ActionToState = new() {
            { AgentSicarioActions.ACTION_REST_IN_THE_COMMUNE, AgentSicarioStates.GO_TO },
            { AgentSicarioActions.ACTION_STUDY_LOCATIONS, AgentSicarioStates.GO_TO },
            { AgentSicarioActions.ACTION_STEAL_MONEY, AgentSicarioStates.PURSUIT },
            { AgentSicarioActions.ACTION_KILL_GUARD_GUN, AgentSicarioStates.KILL },
            { AgentSicarioActions.ACTION_KILL_GUARD_SNIPER, AgentSicarioStates.KILL },
            { AgentSicarioActions.ACTION_KILL_TARGET_GUN, AgentSicarioStates.KILL },
            { AgentSicarioActions.ACTION_KILL_TARGET_SNIPER, AgentSicarioStates.KILL },
            { AgentSicarioActions.ACTION_BUY_GUN, AgentSicarioStates.GO_TO },
            { AgentSicarioActions.ACTION_BUY_SNIPER, AgentSicarioStates.GO_TO },
        };
    }
}
