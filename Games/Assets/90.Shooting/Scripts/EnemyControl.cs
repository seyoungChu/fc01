using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControl : MonoBehaviour {

    public float speed = 3.0f;
    private Transform myTransform = null;
    public GameObject explosionEffect = null;

	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 moveAmount = speed * Vector3.back * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if(myTransform.position.y < - 5.0f) 
        {
            InitPosition();
        }
	}

    void InitPosition() 
    {
        float PositionX = Random.Range(-4.0f, 4.0f);
        myTransform.position = new Vector3(PositionX, 5.0f, 0.0f);
        // 위에서 얻은 X의 랜덤값을 에너미의 위치에 넣는다 
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Bullet")
        {
            MainControl.Score = MainControl.Score + 100;
            Instantiate(explosionEffect, myTransform.position, Quaternion.identity);
            InitPosition();
            Destroy(other.gameObject);
        }
    }
    //트리거가 collider에 닿았을 때 실행되고 충돌체의 정보를 넘겨준다


}
