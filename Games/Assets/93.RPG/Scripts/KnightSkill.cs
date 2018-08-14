using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightSkill : MonoBehaviour {

    //검출 영역범위 
    public float radius = 5.0f;
    //물리 시뮬레이션에서 적용시킬 힘의 값.
    public float power = 200.0f;
    
	// Use this for initialization
	void Start () {
        Vector3 explosionPos = transform.position;
        //내 위치를 중심으로 radius반경 위치에 있는 모든 충돌체를 얻어오고 
        Collider[] colliders = Physics.OverlapSphere(explosionPos, radius);
        foreach(Collider hit in colliders)
        {   //그 중에 sword, player라는 태그를 가지고 있는 충돌체는 무시하고.
            if(hit.tag == "sword" || hit.tag == "Player")
            {
                continue;
            }
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if(rb != null)
            {   //내가 원하는 힘만큼 원하는 위치와 반경에서 폭파 시뮬레이션 
                //Force를 적용합니다.
                rb.AddExplosionForce(power, explosionPos, radius, 3.0f);
            }

        }
	}
	
}
