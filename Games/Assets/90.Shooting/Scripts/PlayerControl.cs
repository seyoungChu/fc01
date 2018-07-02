using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float Speed = 1.0f;
    private Transform myTransform = null;
    // 객체를 담아둘 변수 선언
    public GameObject myBullet = null;

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
        //이 스크립트를 가진 오브젝트에 해당 컴포넌트 추가한다 - 초기화 ( 캐싱 ) 
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveAmount = Speed * Vector3.left * Input.GetAxis("Horizontal") * Time.deltaTime;
        myTransform.Translate(moveAmount);
        //객체의 기능 중 이동시키는데 이동의 양은 moveAmount에서 받는다

        if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            Instantiate(myBullet, myTransform.position, Quaternion.identity);
            // 총알 오브젝트를 현재 나(비행기a)의 위치에 기본 각도로 생성하겠다
        }
	}
}
