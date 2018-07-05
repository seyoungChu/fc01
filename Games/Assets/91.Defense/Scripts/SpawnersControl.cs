using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnersControl : MonoBehaviour {

    public GameObject[] Spawners = null;
    public GameObject SpawnObject = null;

    public float SpawnDelay = 1.0f;
    private float LastSpawnTime = 0.0f;
    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        //현재시간이 마지막으로 스폰한 시간으로부터 딜레이 시간만큼 지났으면.
        if(Time.time > LastSpawnTime + SpawnDelay)
        {
            LastSpawnTime = Time.time;//이벤트 발생시간 현재시간으로 세팅.
            GameObject Beetle = (GameObject)Instantiate(SpawnObject,
                Spawners[0].transform.position,
                Spawners[0].transform.rotation);
            for(int i = 1; i < Spawners.Length; i++)
            {
                Beetle.SendMessage("SetTargetPosition", Spawners[i].transform.position);
            }

        }
    }
    /// <summary>
    /// 선택되었을때만 기즈모를 그리는 이벤트 함수.
    /// </summary>
    private void OnDrawGizmosSelected()
    {   //지름 1.0의 구(sphere)기즈모를 그리게됩니다.
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
    
    private void OnDrawGizmos()
    {
        if (Spawners == null || Spawners.Length == 0)
        {
            return;
        }
        
        for(int i = 0; i < Spawners.Length; i++) //0,1,2,3,4,5
        {
            if(Spawners[i] != null)
            {
                Gizmos.color = Color.red;
                //vector3.one = (1.0f,1.0f,1.0f)
                Gizmos.DrawWireCube(Spawners[i].transform.position, Vector3.one);
                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(Spawners[i].transform.position, 1.0f);
                if (i < Spawners.Length - 1 && Spawners[i+1] != null) // 0,1,2,3,4
                {   //[0,1] ,[1,2], [2,3] , [3,4], [4,5]
                    Gizmos.DrawLine(Spawners[i].transform.position,
                        Spawners[i + 1].transform.position);
                }
            }
            
        }
    }
    
}
