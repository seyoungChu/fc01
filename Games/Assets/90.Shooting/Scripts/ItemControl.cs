using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemControl : MonoBehaviour {

    private Transform myTransform = null;
    public float moveAmount = 1.0f;
    public float Speed = 5.0f;

    // Use this for initialization
    void Start () {
        myTransform = GetComponent<Transform>();
        //< > Type을 의미, () 함수를 의미
    }

    // Update is called once per frame
    void Update () {

        Vector3 moveAmount = Vector3.down * Speed * Time.deltaTime;
        myTransform.Translate(moveAmount);

        if (myTransform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }

    }
    
    //트리거가 내 이벤트 함수에 닿는 순간 자동으로 호출
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "player")
        {
            MainControl.Score += 250;
            Destroy(gameObject);
            //나와 부딪힌 other의 gameObject 제거

        }

    }
}
