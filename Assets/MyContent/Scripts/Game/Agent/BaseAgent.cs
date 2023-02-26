using System;
using System.Collections;
using System.Collections.Generic;
using MyContent.Scripts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GridEntity))]
public abstract class BaseAgent : MonoBehaviour{
    protected TerrainChecker _terrainChecker;
    protected MeshRenderer _meshRenderer;
    protected float _life = 100;
    public CircleQuerier radiusQuerier;
    public GridEntity _gridEntity;
    
    public virtual float MovementSpeed { get; set; }
    
    protected void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
        _terrainChecker = GetComponentInChildren<TerrainChecker>();
    }
    
    public void WalkForward() {
        if (_terrainChecker.isTerrain) {
            // TODO: Pending create evade obstacles
            // transform.eulerAngles += new Vector3(0, Random.Range(90, -90), 0);
            // _terrainChecker.isTerrain = false;
            return;
        }

        transform.position += transform.forward * Time.deltaTime * MovementSpeed;
    }
    
    public void OnDead() {
        // Hotfix
        if (_life > 0) return;

        _meshRenderer.material.color = Consts.AGENT_DEAD_COLOR;
        Destroy(GetComponent<BoxCollider>());
        StartCoroutine("DestroyAgent");
    }

    private IEnumerator DestroyAgent() {
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}