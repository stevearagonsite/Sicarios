using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Items;
using LINQExtension;
using MyContent.Scripts;
using UnityEngine;
using Debug = Logger.Debug;

public delegate IEnumerable<Item> ThinkGetItemByView(BaseAgent owner, IEnumerable<Item> everything);

public static class ThinkingAgent{
    public static void RandomWalk(BaseAgent agent) {
        agent.MovementSpeed = Random.Range(
            Consts.AGENT_MIN_MOVEMENT_SPEED, 
            Consts.AGENT_MAX_MOVEMENT_SPEED
            );
        agent.transform.eulerAngles += new Vector3(0, Random.value, 0) * Random.Range(0f, 360f);
    }
    
    public static IEnumerable<Item> Targets(BaseAgent owner, IEnumerable<Item> possibleTargets) {
        if (possibleTargets == null) return null;
#if UNITY_EDITOR
        Debug.Log("ThinkingAgent Count", $"Targets: {possibleTargets.Count()}");
#endif
        var viewAngle = owner.lineOfSight.viewAngle;
        var viewDistance = owner.lineOfSight.viewDistance;
        var itemsByView = possibleTargets
            .Where(i => TargetInSight.InSight(owner.transform, i.transform, viewAngle, viewDistance));
      
        if (itemsByView?.Count() <= 0) return null;
        return itemsByView
            .Where(i => !i.insideInventory)
            .Where(i => i.name != owner.name)
            .OrderBy(d => Vector3.Distance(owner.transform.position, d.transform.position));
    }

    public static void GetLocation(BaseAgent agent) {
    }
}