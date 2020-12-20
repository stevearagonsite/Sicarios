using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public delegate bool InSightDelegate(Transform a , Transform b, float va , float vd);

public static class TargetInSight 
{
    public static bool InSight(Transform a, Transform b, float va, float vd)
    {
		//1. Calculamos el vector direccional hacia el target.
		var _dirToTarget = b.transform.position - a.position;

		//2. Calculamos el angulo entre el forward del Npc y el del target.
		var _angleToTarget = Vector3.Angle(a.forward, _dirToTarget);

		//3. Calculamos la distancia entre el Npc y el target.
        var _distanceToTarget = Vector3.Distance(a.position, b.transform.position);

        RaycastHit rch;
        bool obstaclesBetween = false;
        if (Physics.Raycast(a.position, _dirToTarget, out rch, _distanceToTarget))
        {
            //Debug.Log(rch.collider.gameObject.name);
            if (rch.collider.gameObject.layer == 8)
            {
                obstaclesBetween = true;
            }
        }

        //4. Si el angulo y la distancia hacia el enemigo son menores o iguales 
        //que el angulo y la distancia de la vision, el objetivo esta a la vista.
        if (_angleToTarget <= va && _distanceToTarget <= vd && !obstaclesBetween)
        {
            //Vemos al enemigo
            return  true;
        }
        else
        {
            //Debug.Log("Clear...");	//No vemos al enemigo
            return  false;
        }

    }
}
