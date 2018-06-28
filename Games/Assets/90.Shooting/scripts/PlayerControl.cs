using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float Speed = 1.0f;
    private Transform myTransform = null;
    public GameObject myBullet = null;

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveAmount = Speed * Vector3.left * Input.GetAxis("Horizontal") * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if(Input.GetKeyDown(KeyCode.Space) == true )
        {
            Instantiate(myBullet, myTransform.position, Quaternion.identity);
            
        }
	}
}
