using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyControl : MonoBehaviour {

    public float Speed = 3.0f;
    private Transform myTransform = null;
    public GameObject explosionEffect = null;
    public GameObject itemBox = null;

    // Use this for initialization
    void Start () {

        myTransform = GetComponent<Transform>();
		
	}
	
	// Update is called once per frame
	void Update () {

        Vector3 moveAmount = Vector3.back * Speed * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if (myTransform.position.y < -5.0f)
        {   
            //만약 enemy를 격추시키지 못했다면 -50점. 단, 50점 이상일 때만
            if(MainControl.Score >= 50)
            {
                MainControl.Score -= 50;
            }

            InitPosition();
            Speed = InitSpeed();
            //myTransform.position = new Vector3(PositionX, 5.0f, 0);
            //myTransform의 position의 위치를 new를 통해 새로운 위치로 옮긴다. 이 때 X의 위치는 랜덤하게 생성한 PositionX의 값을 통해 결정
            // 자주 사용하는 코드를 함수로 선언하여 InitPosition을 호출 받는 argument가 없기에 ()로
        }
        
    }

    //중복되는 코드를 호출하기 위해 함수 선언
    void InitPosition()
    {
        float PositionX = Random.Range(-4.0f, 4.0f);
        myTransform.position = new Vector3(PositionX, 5.0f, 0);

    }

    //적 생성 시 랜덤 스피드
    static float InitSpeed()
    {
        float rndSpeed = Random.Range(4.0f, 10.0f);

        return rndSpeed;
    }

    //아이템 생성 확률
    static int ItemDropProb()
    {
        int rndItemProb = Random.Range(1, 100);
        

        return rndItemProb;
    }

    //아이템 생성 위치
    static int ItemRndPosition()
    {
        int rndItemPosX = Random.Range(-3, 3);

        return rndItemPosX;
    }

    //트리거가 내 이벤트 함수에 닿는 순간 자동으로 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "bullet")
        {
            MainControl.Score += 100;
            Instantiate(explosionEffect, myTransform.position, Quaternion.identity);
            //public으로 정의한 explosionEffect GameObject를 내가 생성한 위치에 생성
            InitPosition();
            Speed = InitSpeed();
            Destroy(other.gameObject);
            //나와 부딪힌 other의 gameObject 제거

            
            //30% 확률로 아이템 생성
            if(ItemDropProb() > 70)
            {
                Instantiate(itemBox, new Vector3(ItemRndPosition(), 4, 0), Quaternion.identity);
                //public으로 정의한 explosionEffect GameObject를 내가 생성한 위치에 생성
            }


        }

    }

}
