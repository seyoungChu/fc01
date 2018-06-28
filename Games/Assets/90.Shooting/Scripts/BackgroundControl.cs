using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundControl : MonoBehaviour {

    public float Speed = 0.1f;
    public Renderer myRenderer = null;
	// Use this for initialization
	void Start () {
        myRenderer = GetComponent<MeshRenderer>();
	}
	// Update is called once per frame
	void Update () {
        myRenderer.material.SetTextureOffset("_MainTex",
            new Vector2(0.0f, -Time.time * Speed));
            
    }
}
