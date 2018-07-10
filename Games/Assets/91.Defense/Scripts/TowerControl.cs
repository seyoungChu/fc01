using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour {
    public enum TowerState { None = 0, Idle, Attack } // 대기상태, 공격상태
    public TowerState myState = TowerState.None; // 타워의 현재 상태.

    public GameObject currentTarget = null;
    public GameObject nextTarget = null;
    public TextMesh stateText = null;


	// Use this for initialization
	void Start () {
        // 내 상태를 대기상태로 지정
        myState = TowerState.Idle;
        //상태를 보여줄 텍스트메쉬를 얻어옴
        stateText = transform.Find("StateTextMesh").GetComponent<TextMesh>();

	}

    /// <summary>
    /// 타워가 Attack 상태일때의 동작
    /// </summary>
    void Attack()
    {
        if(currentTarget != null)
        {
            // 공격
        }
        else
        {
            // 대기 상태로 돌아감
            myState = TowerState.Idle;
        }
    }

	// Update is called once per frame
	void Update () {
        stateText.text = myState.ToString();
        switch (myState)
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
        switch (myState)
        {
            case TowerState.Idle:
                //타원의 상태를 공격상태로 변경
                myState = TowerState.Attack;
                //현재목표를 방금 사정거리에 들어온 타겟으로 지정
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
                //빠져나간 타겟이 현재 타겟이면
                if(currentTarget == target)
                {
                    //다음 타겟을 현재타겟으로
                    currentTarget = nextTarget;
                    
                    nextTarget = null;
                }
                break;
        }
    }
}
