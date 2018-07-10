using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

	public float Speed = 1.0f;
	private Transform myTransform = null;
	public GameObject myBullet = null;
	public GameObject explosionEffect = null;

	// Use this for initialization
	void Start () {
		myTransform = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 moveAmount = Speed * Vector3.left * Input.GetAxis ("Horizontal") * Time.deltaTime;
		myTransform.Translate (moveAmount);
		if (myTransform.position.x < -6.0f)
			myTransform.position = new Vector3 (-6.0f, myTransform.position.y, myTransform.position.z);
		else if (myTransform.position.x > 6.0f)
			myTransform.position = new Vector3 (6.0f, myTransform.position.y, myTransform.position.z);

		if (Input.GetKeyDown (KeyCode.Space)) {
			Instantiate (myBullet, myTransform.position, Quaternion.identity);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "enemy") {
			MainControl.Life -= 1;
			Instantiate (explosionEffect, myTransform.position, Quaternion.identity);
			Destroy (gameObject);
		}
	}
}
