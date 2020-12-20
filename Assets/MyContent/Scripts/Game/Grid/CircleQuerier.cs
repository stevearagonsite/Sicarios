using System;
using System.Collections.Generic;
using UnityEngine;

public class CircleQuerier : MonoBehaviour {
	public Grid targetGrid;
	public Color gizmoColor = Color.red;
	public float radius = 15;

	public IEnumerable<GridEntity> Query() {
		return targetGrid.Query(
			transform.position + new Vector3(-radius, 0, -radius),
			transform.position + new Vector3(radius, 0, radius),
			position => {
				var position2d = position - transform.position;
				position2d.y = 0;
				return position2d.sqrMagnitude < radius * radius;
			}
		);
	}

	void OnDrawGizmos() {
		if (targetGrid == null)
			return;

		//Flatten the sphere we're going to draw
		Gizmos.matrix *= Matrix4x4.Scale(Vector3.forward + Vector3.right);
		Gizmos.color = gizmoColor;

		Gizmos.DrawWireSphere(transform.position, radius);

		if(Application.isPlaying) {
			foreach(var entity in Query())
				Gizmos.DrawWireSphere(entity.transform.position, 3);
		}
	}
}

