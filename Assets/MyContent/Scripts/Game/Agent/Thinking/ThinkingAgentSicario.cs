using System;
using System.Collections.Generic;
using MyContent.Scripts;
using UnityEngine;
using Random = UnityEngine.Random;

public delegate void ThinkDelegateSicario(BaseAgent a);

public static class ThinkingSicario {

    public static void RandomWalk(BaseAgent agent) {
        agent.MovementSpeed = Random.Range(
            Consts.AGENT_PERSON_MIN_MOVEMENT_SPEED,
            Consts.AGENT_PERSON_MAX_MOVEMENT_SPEED
        );
        agent.transform.eulerAngles += new Vector3(0, Random.value, 0) * Random.Range(-180f, 180f);
    }
    
    public static void GetLocation(BaseAgent agent) {
    }
}