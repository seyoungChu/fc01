using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorControl : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "enemy")
        {
            // 타겟이 들어옴
            Debug.Log("Enter Enemy : " + other.name);
            
            // 자식이 부모 게임오브젝트에게 SendMessage를 보내는 경우 : SendMessageUpwards
            // TriggerDetector -> Tower
            gameObject.SendMessageUpwards("EnterEnemy", other.gameObject);
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        // 충돌체를 빠져나간 게임오브젝트의 태그가 Enemy 라면
        if(other.tag == "enemy")
        {
            // 로그를 출력
            Debug.LogWarning("Exit Enemy : " + other.name);
            // 부모 게임오브젝트에게 메세지를 보냄
            // TriggerDetector -> Tower
            gameObject.SendMessageUpwards("ExitEnemy", other.gameObject);
        }
    }
}
