using System;
using System.Collections.Generic;
using UnityEngine;

public class BoxQuerier : MonoBehaviour {
	public Grid targetGrid;
	public Color gizmoColor = Color.red;
	public float width = 3;
	public float height = 5;

	public IEnumerable<GridEntity> Query() {
		return targetGrid.Query(
			transform.position + new Vector3(-width * 0.5f, 0, -height * 0.5f),
			transform.position + new Vector3(width * 0.5f, 0, height * 0.5f),
			position => true
		);
	}

	void OnDrawGizmos() {
		if (targetGrid == null)
			return;

		//Flatten the sphere we're going to draw
		Gizmos.color = gizmoColor;
		Gizmos.DrawWireCube(transform.position, new Vector3(width, 0, height));

		if(Application.isPlaying) {
			foreach(var entity in Query())
				Gizmos.DrawWireSphere(entity.transform.position, 3);
		}
	}
}

