using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleControl : MonoBehaviour {

    public float Speed = 1.0f;
    private Transform myTransform = null;
    public Vector3[] TargetPositions = new Vector3[10];
    public int[] TargetIndex = new int[10];
    private int AddIndex = 0;
    private int CurrentIndex = 0;
    
	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>(); //cashing.
        //애니메이션 재생모드를 반복 모드로 세팅.
        GetComponent<Animation>().wrapMode = WrapMode.Loop;
	}
	
	// Update is called once per frame
	void Update () {
        //방향+크기 벡터 = 목표 위치 - 내 위치.
        Vector3 Diff = TargetPositions[CurrentIndex] - myTransform.position;
        //거리를 구하는 함수 Distance (목표위치, 내위치)
        float Distance = Vector3.Distance(TargetPositions[CurrentIndex],
                                            myTransform.position);
        if (Distance < 0.1f) //가깝다면.
        {
            CurrentIndex++;//CurrentIndex를 1 증가합니다.
            //다음 목표 위치가 0,0,0이라면.
            if(TargetPositions[CurrentIndex] == Vector3.zero)
            //if(CurrentIndex == AddIndex)
            {
                Destroy(gameObject);
            }
        }
        //방향 벡터 = 방향+크기 벡터 - 크기.
        Vector3 Direction = Diff.normalized;

        //이동
        myTransform.Translate(Speed * Direction * Time.deltaTime, Space.World);
        myTransform.LookAt(TargetPositions[CurrentIndex]);
	}
    /// <summary>
    /// 비틀의 목표 위치들을 저장하는 함수.
    /// </summary>
    /// <param name="targetPos"></param>
    void SetTargetPosition(Vector3 targetPos)
    {
        TargetPositions[AddIndex] = targetPos;
        AddIndex++; //Addindex = AddIndex + 1;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "AttackCollider")
        {
            Debug.Log("Damage Hitted!!");
        }
    }
}
