using System.Collections;
using System.Linq;
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
    }
    
    public virtual IEnumerator SicarioDecisions(float time) {
        while (true) {
            _fsm.Feed(AgentConstants.IDLE);
            if (_plan != null) {
                GOAPActionDelegate action = _plan.First();
            }

            yield return new WaitForSeconds(time);
        }
    }
}
