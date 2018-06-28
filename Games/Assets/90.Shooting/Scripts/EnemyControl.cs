using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour {

    public float Speed = 3.0f;
    private Transform myTransform = null;
    public GameObject explosionEffect = null;
	
	void Start () {

        myTransform = GetComponent<Transform>();
       
	}
	
	
	void Update () {

        Vector3 moveAmount = Vector3.back * Speed * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if(myTransform.position.y < -5.0f)
        {
            InitPosition();
            MainControl.Score -= 56;
        }

        if(PlayerControl.collisionCheck == true)
        {
            Instantiate(explosionEffect, myTransform.position, Quaternion.identity);
            InitPosition();
            PlayerControl.collisionCheck = false;

        }

	}

    void InitPosition()
    {
        float PositionX = Random.Range(-4.0f, 4.0f);
        myTransform.position = new Vector3(PositionX, 5.0f, 0.0f); 
    
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet") 
        {   
            Instantiate(explosionEffect, myTransform.position, Quaternion.identity);
            InitPosition();
            Destroy(other.gameObject);
            MainControl.Score += 132;
        }
    }
}
