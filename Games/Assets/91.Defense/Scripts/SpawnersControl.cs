using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnersControl : MonoBehaviour {

    public GameObject[] Spawners = null;

    // Use this for initialization
    void Start() {

    }

    // Update is called once per frame
    void Update() {

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
        Gizmos.color = Color.red;
        for(int i = 0; i < Spawners.Length; i++)//0,1,2,3,4,5
        {
            //vector3.one = (1.0f,1.0f,1.0f)
            Gizmos.DrawWireCube(Spawners[i].transform.position, Vector3.one);
        }
        Gizmos.color = Color.blue;
        for(int i = 0; i < Spawners.Length; i++) //0,1,2,3,4,5
        {
            Gizmos.DrawWireSphere(Spawners[i].transform.position, 1.0f);
            if( i < Spawners.Length - 1) // 0,1,2,3,4
            {   //[0,1] ,[1,2], [2,3] , [3,4], [4,5]
                Gizmos.DrawLine(Spawners[i].transform.position,
                    Spawners[i + 1].transform.position);
            }
        }
    }
    
}
