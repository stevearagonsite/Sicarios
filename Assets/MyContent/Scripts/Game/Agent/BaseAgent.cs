using System;
using System.Collections;
using System.Collections.Generic;
using MyContent.Scripts;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GridEntity))]
public abstract class BaseAgent : MonoBehaviour{
    protected MeshRenderer _meshRenderer;
    protected float _life = 100;
    
    public virtual float MovementSpeed { get; set; }
    protected void Awake() {
        _meshRenderer = GetComponent<MeshRenderer>();
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