using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnersControl : MonoBehaviour {

    public GameObject[] Spawners = null;
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

            for (int i = 0; i < Spawners.Length; i++ ) {
                Beetle.SendMessage("SetTargetPosition", Spawners[i].transform.position);
            }
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
