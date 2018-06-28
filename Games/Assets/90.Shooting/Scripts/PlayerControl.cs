using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float Speed = 5.0f;
    public GameObject myBullet = null;
    private Transform myTransform = null;

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 moveAmount = Speed * Vector3.left * Input.GetAxis ("Horizontal") * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if (Input.GetKeyDown(KeyCode.Space) == true) {
            // 총알 생성
            Instantiate(myBullet, myTransform.position, Quaternion.identity);
        }
	}
}
