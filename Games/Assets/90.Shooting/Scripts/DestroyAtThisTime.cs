using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAtThisTime : MonoBehaviour {

    public float Time = 1.0f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, Time);
	}
}
