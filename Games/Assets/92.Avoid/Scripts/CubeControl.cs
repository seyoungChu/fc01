﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour {
    //내가 조종할 큐브 플레이어 게임오브젝트.
    public GameObject Player = null;
    //현재 픽킹한 위치를 담아놓습니다.
    public Vector3 PickPosition = Vector3.zero;
    //나의 생명력
    public int Lives = 5;
    public int Score = 0; //내가 획득한 점수.
    public GUISkin mySkin = null;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        //마우스 왼쪽 버튼을 클릭했는데.
		if(Input.GetMouseButtonDown(0) == true)
        {
            //플레이어가 현재 없다면.
            if(Player == null)
            {
                Player = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Player.transform.position = PickPosition;
                Player.AddComponent<CubeMove>();
                Player.tag = "Player"; //Tag를 Player로 지정.
            }
        }
	}

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit = new RaycastHit();
        int BackgroundLayer = LayerMask.NameToLayer("Background");
        int PickLayer = 1 << BackgroundLayer;
        if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity, PickLayer) == true)
        {
            PickPosition = raycastHit.point;
            if(Player != null)
            {
                Player.SendMessage("SetNextPosition", PickPosition);
            }
        }
    }

    private void OnGUI()
    {
        GUI.skin = mySkin;
        Score = (int)Time.time;//0.0001 -> 0 , 1.00123 -> 1
        GUILayout.Label("Score:" + Score);
        GUILayout.Label("Lives:" + Lives);
    }


}