using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BettleControl : MonoBehaviour {

    public float Speed = 1.0f;
    private Transform myTransform = null;
    public Vector3[] TargetPositions = new Vector3[10];
    private int AddIndex = 0;
    private int CurrentIndex = 0; 

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>(); //cashing
        GetComponent<Animation>().wrapMode = WrapMode.Loop;
	}
	
	// Update is called once per frame
	void Update () {
        
        //가고자하는 위치 - 현재 위치 = 이동할 크기와 방향
        Vector3 Diff = TargetPositions[CurrentIndex] - myTransform.position;
        float Distance = Vector3.Distance(TargetPositions[CurrentIndex], myTransform.position);

        if(Distance < 0.1f) // 가깝다면
        {
            CurrentIndex++; // CurrentIndex 1 증가시킴
            if(TargetPositions[CurrentIndex] == Vector3.zero)
            {
                Destroy(gameObject);
            }
        }

        Vector3 Direction = Diff.normalized; //정규화 방향 벡터 = 방행 + 크기 벡터 - 크기

        //이동
        myTransform.Translate(Speed * Direction * Time.deltaTime, Space.World);
        myTransform.LookAt(TargetPositions[CurrentIndex]); // 가고자하는 위치로 오브젝트 돌려줌
	}

    //비틀의 목표 위치들을 저장하는 함수
    void SetTargetPosition(Vector3 taregetPos) {
        TargetPositions[AddIndex] = taregetPos;
        AddIndex++;
    }
}
