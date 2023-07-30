﻿using System.Collections;
using System.Collections.Generic;
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
}