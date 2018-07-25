using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//이 스크립트는 에디터가 실행하지 않아도 Update, OnGUI함수가 호출됩니다.
[ExecuteInEditMode]
//이 스크립트는 rigidbody컴포넌트가 반드시 필요합니다.
[RequireComponent(typeof(Rigidbody))]
public class ScriptAdvance : MonoBehaviour {
    [Header("헤더입니다.")]

    //scale의 값을 0~10까지 제한합니다.
    [Range(0.0f,10.0f)]
    public float Scale = 1.0f;
    [Tooltip("툴팁입니다.")]
    public string myString = string.Empty;

    [Header("컨텍스트메뉴입니다.")]

    [ContextMenuItem("리셋","Init")]
    public bool IsInit = false;
    private void Init()
    {
        Scale = 1.0f;
        myString = string.Empty;
        transform.localScale = new Vector3(Scale, Scale, Scale);
    }

    // Use this for initialization
    void Start () {
        //코루틴을 시작합니다.
        StartCoroutine(CoroutineTest());
	}
    //반환값은 반드시 IEnumerator
    IEnumerator CoroutineTest()
    {
        Debug.Log("Time:" + Time.time.ToString());
        yield return new WaitForSeconds(1);//1초기다린후 이어서 동작
        Debug.Log("Time:" + Time.time.ToString());
        yield return new WaitForSeconds(3);//3초 기다린후 이어서 동작
        Debug.Log("Time:" + Time.time.ToString());
        yield return null;//한 프레임 쉬고
        Debug.Log("Time:" + Time.time.ToString());
        yield return null;//한 프레임 쉬고
        Debug.Log("Time:" + Time.time.ToString());
    }

	
	// Update is called once per frame
	void Update () {
        transform.localScale = new Vector3(Scale, Scale, Scale);
	}
}
