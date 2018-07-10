using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundControl : MonoBehaviour {

    public float Speed = 0.1f;
    public Renderer myRenderer = null;

	// Use this for initialization
	void Start () {

        myRenderer = GetComponent<MeshRenderer>();
		//이 스크립트가 붙어있는 랜더러를 가져온다.
	}
	
	// Update is called once per frame
	void Update () {

        myRenderer.material.SetTextureOffset("_MainTex", new Vector2(0.0f, -Time.time * Speed));
        //myRenderer.material.SetTextureOffset("_MainTex", new Vector2(-Time.deltaTime * Speed, 0.0f));

        //myRenderer의 메테리얼을 SetTextureOffset 함수를 이용해 offset 해주겠다.
        // Texture의 Properties 이름이 _MainTex

    }
}
