﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletControl : MonoBehaviour {
    
    public float Speed = 5.0f;
    private Transform myTransform = null;

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveAmount = Speed * Vector3.up * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if (myTransform.position.y > 5.0f) {
            Destroy(gameObject);
        }
	}
}
