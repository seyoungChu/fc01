﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour {

    private Transform myTransform = null;
    public Vector3 NextPosition = Vector3.zero;

    [Range(0.0f, 1.0f)]
    public float Speed = 0.1f;

    void SetNextPosition(Vector3 next){
        NextPosition = next;    
    }
	// Use this for initialization
	void Start () {
        myTransform.GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        if(NextPosition != Vector3.zero)
        {
            myTransform.position = Vector3.Lerp(transform.position, NextPosition, Speed);
        }
	}
}
