using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using MyContent.Scripts;
using UnityEngine;

[RequireComponent(typeof(Item))]
public class AgentTarget : BaseAgent{
    private void Start() {
        _meshRenderer.material.color = Consts.AGENT_TARGET_COLOR;
        gameObject.name = "Target";
        var item = GetComponent<Item>();
        item.type = ItemConstants.ITEM_TYPE_TARGET;
    }

    protected override IEnumerable<GOAPActionDelegate> goapPlan {
        set => throw new System.NotImplementedException();
    }

    protected override void ExecutePlan(List<Tuple<string, Item>> plan) {
        throw new System.NotImplementedException();
    }
}