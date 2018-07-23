using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectorControl : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "enemy")
        {
            // 타겟이 들어옴
            Debug.Log("Enter Enemy : " + other.name);
            // 자식이 부모 게임 오브젝트에게 SendMessage를 보내는 경우에 사용
            // TriggerDetector > Tower 
            gameObject.SendMessageUpwards("EnterEnemy", other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "enemy") 
        {
            Debug.Log("Exit Enemy : " + other.name);
            gameObject.SendMessageUpwards("ExitEnemy", other.gameObject);
        }
    }
}
