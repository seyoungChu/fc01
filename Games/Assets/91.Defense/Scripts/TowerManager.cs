using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerManager : SingletonMonobehaviour<TowerManager> {
    //타워컨트롤 스크립트를 담아놓는다. TowerControls[10].
    public List<TowerControl> towerControls = new List<TowerControl>();
	// Use this for initialization
	void Start () {
        //차일드로 붙어있는 게임오브젝트의 갯수반큼 반복하고.
		for(int i = 0; i < transform.childCount; i++)
        {   //i번째(0~갯수)에있는 자식 게임오브젝트로부터 TowerControl컴포넌트를 얻어오게됩니다.
            TowerControl control = transform.GetChild(i).GetComponent<TowerControl>();
            control.myID = i; //myID = 0 ~ child갯수.
            towerControls.Add(control);//towerControls리스트에 담아놓습니다.
            //0번째라면 towerControls[0], 1번째라면 towerControls[1]
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
