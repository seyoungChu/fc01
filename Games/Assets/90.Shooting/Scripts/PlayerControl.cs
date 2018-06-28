using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {
    
    public float speed = 1.0f;
    private Transform myTransform = null;
    public GameObject myBullet = null;
    static public bool collisionCheck = false;

	void Start () {
        myTransform = GetComponent<Transform>();
	}
   
	void Update () {
        Vector3 moveAmount = speed * Vector3.left * Input.GetAxis("Horizontal") * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if(Input.GetKeyDown(KeyCode.Space) == true ) {
            Instantiate(myBullet, myTransform.position, Quaternion.identity);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "enemy") 
        {
            MainControl.Life--;
            MainControl.Score -= 256;
            collisionCheck = true;

        }
    }
}
