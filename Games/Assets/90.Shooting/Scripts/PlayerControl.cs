using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour {

    public float Speed = 20.0f;
    private Transform myTransform = null;
    public GameObject myBullet = null;          //myBullet이란 이름의 GameObject 변수 생성
    public GameObject explosionEffect = null;
    public float speedTime = 5.0f;

    // Use this for initialization
    void Start () {

        myTransform = GetComponent<Transform>();
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 moveAmount = Speed * Vector3.left * Input.GetAxis("Horizontal") * Time.deltaTime;
        // 내가 원하는 속도 * 내가 원하는 방향 * 입력된 방향 * 지난 프레임이 완료된 시간
        myTransform.Translate(moveAmount);

        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            Instantiate(myBullet, myTransform.position, Quaternion.identity);
            //만약 space key가 입력된다면, myBullet이란 GameObject를 myTransform의 position에 생성
        }
        

        //한 쪽 화면 밖으로 나가면 반대쪽으로 플레이어 이동
        if (myTransform.position.x > 7.0f)
        {
            myTransform.position = new Vector3(-7.0f, -4.0f, 0);
        }
        else if(myTransform.position.x < -7.0f)
        {
            myTransform.position = new Vector3(7.0f, -4.0f, 0);
        }
     

    }

    //트리거가 내 이벤트 함수에 닿는 순간 자동으로 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "enemy")
        {
            MainControl.Life -= 1;
            Instantiate(explosionEffect, myTransform.position, Quaternion.identity);
            //public으로 정의한 explosionEffect GameObject를 내가 생성한 위치에 생성
            //InitPosition();
            //Destroy(other.gameObject);

        }


    }

}
