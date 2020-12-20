using UnityEngine;
using System.Collections;

public class LineOfSight : MonoBehaviour {

    public GameObject target;
    public float viewAngle;
    public float viewDistance;
    
	const int wallLayer = 9;
	bool targetInSight;
	int layerMask;

	public Transform Target { get { return target.transform; } }
	public bool TargetInSight { get { return targetInSight; } }

	public void Configure(float viewAngle, float viewDistance) {
		this.viewAngle = viewAngle;
		this.viewDistance = viewDistance;
	}

	void Awake() {
		layerMask = (1 << wallLayer);
	}

	void Execute () {
		// Primero calculamos que cumpla con los requisitos de distancia y ángulo.
		// Es decir, que este dentro del campo de visión sin contar obstáculos.

		// Siempre la dirección desde un punto a otro es: Posición Final - Posición Inicial
		var dirToTarget = target.transform.position - transform.position; 
        
		// Vector3.Angle nos da el ángulo entre dos direcciones
		var angleToTarget = Vector3.Angle(transform.forward, dirToTarget);

		// Vector3.Distance nos da la distancia entre dos posiciones
		var sqrDistanceToTarget = (transform.position - target.transform.position).sqrMagnitude;

		RaycastHit rch;

		targetInSight = 
			// Verifica el angulo de vision
			angleToTarget <= viewAngle &&
			// Verifica la distancia de vision
			sqrDistanceToTarget <= viewDistance * viewDistance &&
			// Verifica si hay obstaculos en el medio
			!Physics.Raycast(
				transform.position,
				dirToTarget,
				out rch,
				Mathf.Sqrt(sqrDistanceToTarget),
				layerMask
			);
	}
}
