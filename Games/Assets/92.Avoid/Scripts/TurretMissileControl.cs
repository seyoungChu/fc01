using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretMissileControl : MonoBehaviour {
    public float Speed = 5.0f;
    private Transform myTransform = null;
    public Vector3 TargetDirection = Vector3.zero;//0,0,0

    void SetTargetDirection(Vector3 dir)
    {
        TargetDirection = dir;
    }
	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if(TargetDirection != Vector3.zero)
        {
            myTransform.Translate(TargetDirection * Speed * Time.deltaTime,
                Space.World);
        }
	}
    //더이상화면에 그려지지않는다면 호출되는 이벤트함수.
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
