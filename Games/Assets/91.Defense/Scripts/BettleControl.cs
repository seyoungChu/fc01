using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BettleControl : MonoBehaviour {

    public float Speed = 1.0f;
    private Transform myTransform = null;
    public Vector3[] TargetPositions = new Vector3[10];
    private int addIndex = 0;
    private int currentIndex = 0;


	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>(); //cashing!!!
        // 애니메이션 재생모드를 반복 모드로 세팅
        GetComponent<Animation>().wrapMode = WrapMode.Loop;

	}
	
	// Update is called once per frame
	void Update () {
        // 방향 + 크기 벡터 = 목표 위치 - 내 위치.
        Vector3 Diff = TargetPositions[currentIndex] - myTransform.position;

        // 거리를 구하는 함수 Distance(목표위치, 내위치)
        float Distance = Vector3.Distance(TargetPositions[currentIndex], myTransform.position);

        //가깝다면 인덱스 1증가
        if (Distance < 0.1f)
        {
            currentIndex++;
            // 다음 목표 위치가 0,0,0 이면,
            if(TargetPositions[currentIndex] == Vector3.zero)
            {
                Destroy(gameObject);
            }
        }

        // 방향 벡터 = 방향 + 크기 벡터 - 크기.
        Vector3 Direction = Diff.normalized;

        //이동
        myTransform.Translate(Speed * Direction * Time.deltaTime,Space.World); // Space.World : 카메라에서 바라보는 절대방향으로
        // 정면을 보게 함
        myTransform.LookAt(TargetPositions[currentIndex]);
	}//update end

    /// <summary>
    /// 비틀의 목표 위치들을 저장하는 함수.
    /// </summary>
    /// <param name="targetPos"></param>
    void SetTargetPosition(Vector3 targetPos)
    {
        TargetPositions[addIndex] = targetPos;
        addIndex++;
    }
}
