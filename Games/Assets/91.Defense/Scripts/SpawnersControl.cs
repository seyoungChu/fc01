using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnersControl : MonoBehaviour {

    //배열 선언
    public GameObject[] Spawners = null;

    //스폰을 위한 선언
    public GameObject SpawnObject = null;
    private float SpawnDelay = 1.0f;
    private float LastSpawnTime = 0.0f;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        //만약 현재 시간이 마지막으로 스폰한 시간으로부터 딜레이 시간만큼 지났다면,
		if(Time.time > LastSpawnTime + SpawnDelay)
        {
            //이벤트 발생 시간을 현재 시간으로 세팅
            LastSpawnTime = Time.time;
            
            //원하는 오브젝트를 생성하고, 오브젝트를 Spawners의 위치에 생성하겠다. 
            GameObject Beetle = (GameObject)Instantiate(SpawnObject, Spawners[0].transform.position, Spawners[0].transform.rotation); 

            //비틀에게 현재 
            for(int i = 1; i < Spawners.Length; i++)
            {
                Beetle.SendMessage("SetTargetPosition", Spawners[i].transform.position);
            }
        }
	}

    //선택되었을 때만 기즈모를 그리는 함수
    private void OnDrawGizmosSelected()
    {
        //지름 1.0f의 Sphere 기즈모를 그린다.
        Gizmos.DrawSphere(transform.position, 1.0f);
    }

    //예외처리
    private void OnDrawGizmos()
    {
        //Spawnes가 비어있거나, 배열 값이 0이면 아무것도 안한다.
        if(Spawners == null || Spawners.Length == 0)
        {
            return;
        }

        //디펜스 라인을 만들기 위해 게임씬에서는 보이지 않는 가상의 선을 만들기 위한 작업
        //기획 또는 레벨디자이너가 시각적으로 확인할 수 있도록
        //Gizmo에 SpawnersControl이라고 생성됨 (check on/off)
        //빌드하면 Gizmo는 아무런 동작도 하지 않음

        //Gizmo의 컬러를 빨강으로
        Gizmos.color = Color.red;

        //Spawners WireCube를 배열로 넣는다.
        for (int i = 0; i<Spawners.Length; i++)
        {
            //Vector3.one = (1.0f, 1.0f, 1.0f)
            Gizmos.DrawWireCube(Spawners[i].transform.position, Vector3.one);
        }

        //Gizmo 색상 변경
        Gizmos.color = Color.blue;


        for(int i = 0; i < Spawners.Length; i++)
        {
            Gizmos.DrawWireSphere(Spawners[i].transform.position, 1.0f);
            if(i < Spawners.Length -1)
            {
                //[0, 1], [1, 2] ... [4, 5] 좌표로 선을 그림
                Gizmos.DrawLine(Spawners[i].transform.position, Spawners[i+1].transform.position);
            }

        }



    }

}
