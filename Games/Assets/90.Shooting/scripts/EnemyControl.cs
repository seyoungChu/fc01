﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour {

    public float Speed = 1.0f;
    private Transform myTransform = null;
    public GameObject explosionEffect = null;

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveAmount = Vector3.back * Speed * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if (myTransform.position.y < -5.0)
        {
            this.InitPosition();
        }
	}

    private void InitPosition()
    {
        float PositionX = Random.Range(-4.0f, 4.0f);
        myTransform.position = new Vector3(PositionX, 5.0f, 0.0f);
    }

	private void OnTriggerEnter(Collider other)
	{
        if(other.tag == "Bullet"){
            MainControl.Score += 100;
            Instantiate(explosionEffect, myTransform.position, Quaternion.identity);
            this.InitPosition();
            Destroy(other.gameObject);
        }
	}
}