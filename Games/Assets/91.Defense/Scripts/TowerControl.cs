using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerControl : MonoBehaviour {
    //타워의 식별ID.
    public int myID = -1;
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
    public Transform AttackDummy = null;//미사일이 생성될 위치를 얻어올 트랜스폼.
    public float AttackDelay = 1.0f;//타워의 공격속도 1초마다 미사일을 생성하게 됩니다.
    private float LastAttackTime = 0.0f;//마지막으로 공격한 시간을 기록합니다.

    public int Level = 1;//타워의 레벨.
    public int Damage = 10;//타워의 공격력.
    public int HitCount = 0;//몬스터를 공격해서 맞춘 횟수.
	// Use this for initialization
	void Start () {
        //내 상태를 대기상태로 지정.
        myState = TowerState.Idle;
        //상태를 보여줄 텍스트메쉬를 얻어옵니다.
        stateText = transform.Find("StateTextMesh").GetComponent<TextMesh>();
        //공격더미를 얻어옵니다.
        AttackDummy = transform.Find("AttackDummy");
	}
    /// <summary>
    /// 타워가 공격(Attack) 상태일때의 동작.
    /// </summary>
    void Attack()
    {
        //타겟이 없으면 동작하지 않습니다.
        if(currentTarget != null)
        {
            //공격딜레이만큼 기다렸다가 이벤트를 발생.
            if(Time.time > LastAttackTime + AttackDelay)
            {
                LastAttackTime = Time.time;//마지막 이벤트 발생시간을 현재시간으로 지정.
                //Prefabs폴더 안에 있는 Missile이라는 프리팹을 생성.
                GameObject missile = (GameObject)Instantiate(
                    Resources.Load("Prefabs/Missile"),
                    AttackDummy.position,
                    AttackDummy.rotation);
                //초기 사이즈는 0.15,0.15,0.1 지정.
                missile.transform.localScale = new Vector3(0.15f, 0.15f, 0.1f);
                //미사일컨트롤 스크립트를 붙여줍니다.
                MissileControl control = missile.AddComponent<MissileControl>();
                //현재 타겟을 미사일의 타겟으로 지정해줍니다.
                control.SetTarget(currentTarget);
                //데미지정보를 갖고 있는 데미지컴포넌트를 붙여주고.
                DamageComponent dam = missile.AddComponent<DamageComponent>();
                //몬스터에게 입힐 데미지를 넘겨줍니다.
                dam.SetDamage(Level * Damage);// 1 X 10
                //이 데미지를 발생시킨건 나의 ID를 넘겨줍니다.
                dam.OwnerID = myID;
            }

        }else
        {   //대기 상태로 돌아간다.
            myState = TowerState.Idle;
        }
    }
	
	// Update is called once per frame
	void Update () {
        //상태를 출력. -> [레벨] : 맞춘횟수
        stateText.text = "[Lv."+Level.ToString() + "]: " + HitCount.ToString();
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
