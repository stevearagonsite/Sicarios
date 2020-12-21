using System;
using System.Collections;
using System.Collections.Generic;
using MyContent.Scripts;
using UnityEngine;

public class TerrainChecker : MonoBehaviour{
    public bool isTerrain { get; set; }


    private void OnTriggerEnter(Collider c) {
        if (c.gameObject.layer == Layers.TERRAIN_NUM_LAYER || c.gameObject.layer == Layers.AGENT_NUM_LAYER) {
            isTerrain = true;
        }
    }
    
    private void OnTriggerExit(Collider c) {
        if (c.gameObject.layer == Layers.TERRAIN_NUM_LAYER) {
            isTerrain = false;
        }
    }

    void OnDrawGizmos() {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(Vector3.zero, Vector3.one);
    }
}