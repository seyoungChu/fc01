using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour {
    //타워의 상태 열거형, 대기상태와 공격상태를 정의하고 있다.
    public enum TowerState { None = 0, Idle, Attack }
    //타워의 현재 상태.
    public TowerState myState = TowerState.None;
    //현재타겟.
    public GameObject currentTarget = null;
    //다음타겟.
    public GameObject nextTarget = null;
    //상태를 보여줄 3D TextMesh 컴포넌트.
    public TextMesh stateText = null;


	// Use this for initialization
	void Start () {
        //내 상태를 대기상태로 지정.
        myState = TowerState.Idle;
        //상태를 보여줄 텍스트메쉬를 얻어옵니다.
        stateText = transform.Find("StateTextMesh").GetComponent<TextMesh>();
	}
    /// <summary>
    /// 타워가 공격(Attack) 상태일때의 동작.
    /// </summary>
    void Attack()
    {
        if(currentTarget != null)
        {
            //attack
        }else
        {   //대기 상태로 돌아간다.
            myState = TowerState.Idle;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //상태를 출력.
        stateText.text = myState.ToString();
        //상태별 동작을 구분.
		switch(myState)
        {
            case TowerState.Idle:
                break;
            case TowerState.Attack:
                Attack();
                break;
        }
	}

    void EnterEnemy(GameObject target)
    {
        switch(myState)
        {
            case TowerState.Idle:
                //타워의상태를 공격상태로 변경.
                myState = TowerState.Attack;
                //현재목표를 방금 사정거리에 들어온 타겟으로 지정.
                currentTarget = target;
                break;
            case TowerState.Attack:
                //다음 공격 타겟으로 지정.
                nextTarget = target;
                break;
        }
    }
    void ExitEnemy(GameObject target)
    {
        switch (myState)
        {
            case TowerState.Idle:
                break;
            case TowerState.Attack:
                //빠져나간 타겟이 현재 타겟이면.
                if(currentTarget == target)
                {   //다음 타겟을 지정하고.
                    currentTarget = nextTarget;
                    //다음 타겟은 비워둔다.
                    nextTarget = null;
                }
                break;
        }
    }
}
