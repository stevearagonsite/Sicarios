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

	void Update () {
		if (target == null) return;
		var dirToTarget = target.transform.position - transform.position; 
        
		var angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
		var sqrDistanceToTarget = (transform.position - target.transform.position).sqrMagnitude;

		RaycastHit rch;

		targetInSight = 
			angleToTarget <= viewAngle &&
			sqrDistanceToTarget <= viewDistance * viewDistance &&
			!Physics.Raycast(
				transform.position,
				dirToTarget,
				out rch,
				Mathf.Sqrt(sqrDistanceToTarget),
				layerMask
			);
	}
	
	void OnDrawGizmosSelected() {
		if (target != null) {
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, target.transform.position);
		}
		
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(transform.position, viewDistance);
    
		Vector3 viewAngleA = DirectionFromAngle(-viewAngle / 2, false);
		Vector3 viewAngleB = DirectionFromAngle(viewAngle / 2, false);
    
		Gizmos.DrawLine(transform.position, transform.position + viewAngleA * viewDistance);
		Gizmos.DrawLine(transform.position, transform.position + viewAngleB * viewDistance);
	}

	Vector3 DirectionFromAngle(float angleInDegrees, bool angleIsGlobal) {
		if (!angleIsGlobal) {
			angleInDegrees += transform.eulerAngles.y;
		}
    
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

}
