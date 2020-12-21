using MyContent.Scripts;
using UnityEngine;

public delegate void ThinkDelegatePerson(BaseAgent a);

public static class ThinkingPerson{
    public static void RandomWalk(BaseAgent agent) {
        agent.MovementSpeed = Random.Range(
            Consts.AGENT_PERSON_MIN_MOVEMENT_SPEED, 
            Consts.AGENT_PERSON_MAX_MOVEMENT_SPEED
            );
        agent.transform.eulerAngles += new Vector3(0, Random.value, 0) * Random.Range(0f, 360f);
    }
}