using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BeetleControl : MonoBehaviour {

    //외부에 노출할 것들은 public으로 정의
    public float speed = 1.0f;
    private Transform myTransform = null;
    //위치를 잡아둘 배열 생성
    public Vector3[] TargetPositions = new Vector3[10];
    private int AddIndex = 0;                           // 목표
    private int CurrentIndex = 0;                       // 현재 어디를 타겟으로 할 것인가?



	// Use this for initialization
	void Start () {

        myTransform = GetComponent<Transform>();        //cashing
        GetComponent<Animation>().wrapMode = WrapMode.Loop;     //애니메이션 반복을 위한 세팅. wrapMode = 재생 모드
        //이렇게 쓰면 모든 애니메이션 모드가 반복으로 되기에 실전에서는 쓰면 안된다. 임시 코드
		
	}
	
	// Update is called once per frame
	void Update () {
        //내가 가고자 하는 위치 - 현재 내 위치 = 그 위치를 가리키고 있는 Vector가 나옴 (크기 포함)
        Vector3 Diff = TargetPositions[CurrentIndex] - myTransform.position;                        // 방향 + 크기 벡터 = 목표 위치 - 내 위치
        float Distance = Vector3.Distance(TargetPositions[CurrentIndex], myTransform.position);     //거리를 구하는 함수 (목표 위치, 내 위치)

        if (Distance < 0.1f)                                                                        //만약 Distance가 0.1보다 작다면, CurrentIndex를 1 증가
        {
            CurrentIndex++;

            //만약 다음 TargetPositions이 0, 0, 0이라면
            if(TargetPositions[CurrentIndex] == Vector3.zero)
            {
                Destroy(gameObject);
            }

        }
        // 방향 벡터 = 방향 + 크기 벡터 - 크기
        //정규화를 통해 크기를 빼고 방향 값만 남김
        Vector3 Direction = Diff.normalized;

        //이동, Space.World = 너의 방향이 어떻든 간에 카메라가 바라보는 방향의 좌표계로 움직여라!
        myTransform.Translate(speed * Direction * Time.deltaTime, Space.World);
        myTransform.LookAt(TargetPositions[CurrentIndex]);      // 가고자 하는 위치로 object의 front를 돌려준다. (LookAt 함수)

		
	}
    
    //비틀의 목표 위치를 저장하는 함수 (targetPos)
    void SetTargetPosition(Vector3 targetPos)
    {
        TargetPositions[AddIndex] = targetPos;
        AddIndex++;                                 //TargetPosition 함수를 호출할 때마다 AddIndex 값을 1씩 증가
    }
}
