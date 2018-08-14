using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour {

    public string TagName = string.Empty;

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == TagName)
        {   //상위 게임오브젝트에게 SetTarget함수가 호출되도록 메세지를 보내고
            //만약 SetTarget이라는 함수가 없다면 무시하도록 옵션을 설정합니다.
            gameObject.SendMessageUpwards("SetTarget",
                other.gameObject, SendMessageOptions.DontRequireReceiver);
        }
    }
}
