using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Items;
using MyContent.Scripts;
using UnityEngine;
using Debug = Logger.Debug;
using AgentConstants = AgentSicarioConstants.AgentSicarioStates;

[RequireComponent(typeof(LineOfSight))]
public partial class AgentSicario : BaseAgent{

    protected override void Awake() {
        base.Awake();
        _meshRenderer.material.color = Consts.AGENT_SICARIO_COLOR;
        gameObject.name = "Sicario";
        MovementSpeed = Consts.AGENT_MAX_MOVEMENT_SPEED;
    }
}
