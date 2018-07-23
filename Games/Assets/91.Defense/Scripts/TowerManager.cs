using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : SingletonMonobehaviour<TowerManager> {
    // Bettle 죽인 타워를 추적하는 기능

    // 타워컨트롤 스크립트를 담아놓는다 TowersControls[10];
    public List<TowerControl> towerControls = new List<TowerControl>();

	// Use this for initialization
	void Start () {
        //자식으로 붙어있는 게임오브젝트 개수만큼 반복하고 
        for (int i = 0; i < transform.childCount; i++)   
        {
            TowerControl control = transform.GetChild(i).GetComponent<TowerControl>();
            // i번째에 있는 자식 게임오브젝트로부터 TowerControl 컴포넌트를 얻어오게 됨
            control.myID = i; // myID = 0 ~ child 갯수 
            towerControls.Add(control); // Towercontrol 리스트에 담는다
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
