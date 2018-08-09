using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinControl : MonoBehaviour
{
    public enum GoblinState { None, Idle, Patrol, Wait, MoveToTarget, Attack, Damage ,Die}
    public GoblinState myState = GoblinState.None;
    //상태를 원래대로 돌려주기위해 이전 상태를 저장해 놓습니다.
    public GoblinState lastState = GoblinState.None;

    public float MoveSpeed = 1.0f;//이동 속도.
    public float RotateSpeed = 100.0f;//회전 속도.
    public GameObject TargetPlayer = null;//타겟 플레이어.
    public Vector3 TargetPosition = Vector3.zero;//(0,0,0) 패트롤시 이동할 목표위치.

    private Animation myAnimation = null;//캐싱.
    private Transform myTransform = null;//캐싱.
    //사용할 애니메이션.
    public AnimationClip IdleAnimation = null;//대기 애니메이션
    public AnimationClip MoveAnimation = null;//이동 애니메이션
    public AnimationClip AttackAnimation = null;//공격 애니메이션
    public AnimationClip DamageAnimation = null;//피격 애니메이션
    public AnimationClip DieAnimation = null;//죽는 애니메이션.
    //
    public int HP = 100; //
    public float AttackRange = 1.5f; //공격 거리
    public GameObject DamageEffect = null; //피격 이펙트.


    // Use this for initialization
    void Start()
    {
        //내 기본상태는 대기
        myState = GoblinState.Idle;
        myAnimation = GetComponent<Animation>();
        myTransform = GetComponent<Transform>();
        //애니메이션 세팅.
        myAnimation[IdleAnimation.name].wrapMode = WrapMode.Loop;
        myAnimation[MoveAnimation.name].wrapMode = WrapMode.Loop;
        myAnimation[AttackAnimation.name].wrapMode = WrapMode.Once;
        myAnimation[DamageAnimation.name].wrapMode = WrapMode.Once;
        myAnimation[DamageAnimation.name].layer = 10; //더 중요하게 재생됩니다.
        myAnimation[DieAnimation.name].wrapMode = WrapMode.Once;
        myAnimation[DieAnimation.name].layer = 10;
        //애니메이션 이벤트 세팅.
        AddAnimationEvent("AttackAnimFinish", AttackAnimation);
        AddAnimationEvent("DamageAnimFinish", DamageAnimation);
        AddAnimationEvent("DieAnimFinish", DieAnimation);
    }
    #region 애니메이션이벤트
    //애니메이션 클립에 애니메이션 이벤트를 추가합니다.
    void AddAnimationEvent(string FuncName, AnimationClip clip)
    {
        AnimationEvent newEvent = new AnimationEvent();
        newEvent.functionName = FuncName;
        newEvent.time = clip.length - 0.1f;
        clip.AddEvent(newEvent);
    }
    //공격 애니메이션이 끝나면 호출됩니다.
    void AttackAnimFinish()
    {
    }
    void DamageAnimFinish()
    {
    }
    void DieAnimFinish()
    {

    }
    #endregion
    // Update is called once per frame
    void Update()
    {

    }
}
