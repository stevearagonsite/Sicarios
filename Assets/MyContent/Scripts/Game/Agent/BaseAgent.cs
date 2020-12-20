using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(GridEntity))]
public abstract class BaseAgent : MonoBehaviour{
    protected CharacterController _controller;
    protected MeshRenderer _meshRenderer;
    protected float _life = 100;
    
    public virtual float MovementSpeed { get; set; }
    protected void Awake() {
        _controller = GetComponent<CharacterController>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }
}