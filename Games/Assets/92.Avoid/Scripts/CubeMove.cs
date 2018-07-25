using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour {

    private Transform myTransform = null;
    public Vector3 NextPosition = Vector3.zero; //0,0,0
    [Range(0.1f,1.0f)]
    public float Speed = 0.2f;

    void SetNextPosition(Vector3 next)
    {
        NextPosition = next;
    }

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
		if(NextPosition != Vector3.zero)
        {
            //lerp는 선형보간입니다.
            myTransform.position = Vector3.Lerp(transform.position,
                NextPosition, Speed);
        }
        //휠을 스크롤했다면.
        if(Input.mouseScrollDelta != Vector2.zero)
        {
            Debug.Log("Scroll:" + Input.mouseScrollDelta.ToString());
            //Speed에 마우스 스크롤 휠의 y값을 이용해서 변경합니다.
            Speed += Input.mouseScrollDelta.y * Time.deltaTime;
            //Speed의 값은 0.1과 1.0 사이에서 정해집니다.
            Speed = Mathf.Clamp(Speed, 0.1f, 1.0f);
        }
        
	}
}
