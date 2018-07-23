using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnersControl : MonoBehaviour {

    //몬스터들의 이동겨로 위치들
    public GameObject[] Spawners = null;
    //소환할 몬스터 프리랩
    public GameObject SpawnObject = null;

    public float SpawnDelay = 1.0f; // 이벤트 발생 간격
    private float LastSpawenTime = 0.0f; // 마지막 이벤트 발생 시간

	// Use this for initialization
	void Start () {
	    	
	}
	
	// Update is called once per frame
	void Update () {
        //현재 시간이 마지막으로 스폰한 시간으로부터 딜레이 시간이 지났으면
        if(Time.time > LastSpawenTime + SpawnDelay)
        {
            LastSpawenTime = Time.time; // 이벤트 발생시간을 현재 시간으로 셋팅, 할당 
            GameObject Beetle = (GameObject)Instantiate(SpawnObject, Spawners[0].transform.position, Spawners[0].transform.rotation);
            // 스폰하는 새로운 몬스터의 위치 = Spawners의 첫번째 위, 각도의 값을 갖는다
            for (int i = 0; i < Spawners.Length; i++ ) {
                Beetle.SendMessage("SetTargetPosition", Spawners[i].transform.position);
                // 몬스터의 경로를 알려주는 코드
            }

            HPBarControl hPBarControl = Beetle.AddComponent<HPBarControl>();
            // HPBarControl 스크립트를 몬스터에 붙임
            hPBarControl.SetHP(100);
            // 초기 HP을 100으로 셋팅
        }
	}

    // 해당 게임 오브젝트를 선택했을 때에만 기즈모를 보여주는 이벤트 함수
    private void OnDrawGizmosSelected()
    {
        // 지름 1.0f 구의 기즈모를 그려라!!!
        Gizmos.DrawSphere(transform.position, 1.0f);
    }


    private void OnDrawGizmos()
    {
        if(Spawners == null || Spawners.Length == 0) 
        {
            return;
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < Spawners.Length; i++) 
        {
            Gizmos.DrawWireCube(Spawners[i].transform.position, Vector3.one);
        }

        Gizmos.color = Color.blue;
        for (int i = 0; i < Spawners.Length; i++)
        {
            Gizmos.DrawWireSphere(Spawners[i].transform.position, 1.0f);
            if (i < Spawners.Length - 1) 
            {
                Gizmos.DrawLine(Spawners[i].transform.position, Spawners[i + 1].transform.position);
            }
        }

    }
}
