using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour {
    // 타워의 식별 ID
    public int myID = -1;

    public enum TowerState { None = 0, Idle, Attack };
    // 타워의 상태 설정
    public TowerState myState = TowerState.None;
    // 타워의 현재 상태

    public GameObject currentTarget = null;
    public GameObject nextTarget = null;
    public TextMesh stateText = null;

    public Transform AttackDummy = null;
    public float AttackDelay = 1.0f;
    public float LastAttackTime = 0.0f;

    public int Level = 1;
    public int Damage = 10;
    public int HitCount = 0;

	// Use this for initialization
	void Start () {
        myState = TowerState.Idle;
        //상태를 보여줄 텍스트 메쉬를 가져온다
        stateText = transform.Find("StateTextMesh").GetComponent<TextMesh>();
        AttackDummy = transform.Find("AttackDummy");
	}
	
    void Attack()
    {
        if(currentTarget != null) {
            if(Time.time > LastAttackTime + AttackDelay)
            {
                LastAttackTime = Time.time;
                GameObject missile = (GameObject)Instantiate(Resources.Load("Prefabs/Missile"), 
                                                             AttackDummy.position, AttackDummy.rotation);
                missile.transform.localScale = new Vector3(0.15f, 0.15f, 0.1f);
                MissileControl control = missile.AddComponent<MissileControl>();
                control.SetTarget(currentTarget);

                // 데미지정보를 갖고 있는 데미지컴포넌트를 붙어줌
                DamageComponent dam = missile.AddComponent<DamageComponent>();
                // 몬스테에게 입힐 데미지를 넘겨줍니다
                dam.SetDamage(Level * Damage);
                // 이 데미지를 발생시킨건 나의 ID를 저장
                dam.OwnerID = myID;
            }
        } else {
            myState = TowerState.Idle;
        }
    }

	// Update is called once per frame
	void Update () {
        //stateText.text = myState.ToString();
        stateText.text = "[Lv."+Level.ToString() + "]: " + HitCount.ToString();
        switch(myState){
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
                myState = TowerState.Attack;
                //현재 목표를 방금 사정거리에 들어온 타켓으로 지정
                currentTarget = target;
                break;
            case TowerState.Attack:
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
                if (currentTarget == target)
                {
                    currentTarget = nextTarget;
                    nextTarget = null;
                }
                // 나간 타켓이 현재 타켓이면 현재타켓을 다음 타켓으로 변경하고
                // 다음타켓은 초기화한다
                break;
        }
    }
}
