using System.Collections;
using System.Collections.Generic;
using MyContent.Scripts;
using UnityEngine;

public class AgentTarget : BaseAgent{
    private void Start() {
        _meshRenderer.material.color = Consts.AGENT_TARGET_COLOR;
    }
}