using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryAtThisTime : MonoBehaviour {
    
    public float Time = 1.0f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject,Time);
        // 이 스크립트를 가지고 있는 게임 오브젝트를 Time만큼 지난 뒤 Destory해라
	}
}
