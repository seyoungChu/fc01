using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAtThisTime : MonoBehaviour {

    public float Time = 1.0f;

	// Use this for initialization
	void Start () {

        Destroy(gameObject, Time);
        //Time이 되면 이 script가 붙어 있는 gameObject를 Destroy 해라!
		
	}
}
