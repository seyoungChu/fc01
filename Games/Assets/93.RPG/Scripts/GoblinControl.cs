using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;
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
    //피격 이펙트 트위닝용.
    private Tweener effectTweener = null;
    public SkinnedMeshRenderer skinBody = null;

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

        skinBody = myTransform.Find("body").GetComponent<SkinnedMeshRenderer>();
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
    {   //상태 제어 함수 호출.
        CheckState();
        //애니메이션 제어 함수 호출.
        AnimationControl();
    }
    //대기 상태일 경우의 동작
    void OnIdle()
    {
        //타겟이 없다면.내 시야에 플레이어가 들어오지 않았다면.
        if(TargetPlayer == null)
        {   //이동하고 싶은 위치를 임의의 점으로 구하되 y축값은 공중으로 지정.
            TargetPosition =
                new Vector3(
                    myTransform.position.x + Random.Range(-10.0f, 10.0f),
                    myTransform.position.y + 1000.0f,
                    myTransform.position.z + Random.Range(-10.0f, 10.0f));
            //TargetPosition위치에서 아랫방향을 향하는 레이.
            Ray ray = new Ray(TargetPosition, Vector3.down);
            RaycastHit raycastHit = new RaycastHit();
            if(Physics.Raycast(ray,out raycastHit, Mathf.Infinity) == true)
            {   //레이가 충돌한 위치의 높이값을 얻어옵니다.
                TargetPosition.y = raycastHit.point.y;
            }
            //고블린의 상태를 순찰 상태로 변경합니다.
            myState = GoblinState.Patrol;

        }else //타겟이 있다면.
        {
            myState = GoblinState.MoveToTarget;
        }
    }
    //대기 상태.
    IEnumerator OnWait()
    {   //대기 상태로 전환.
        myState = GoblinState.Wait;
        //대기 시간을 랜덤하게 1~3초 사이의 시간을 구하고.
        float waitTime = Random.Range(1.0f, 3.0f);
        //대기 시간만큼 기다렸다가.
        yield return new WaitForSeconds(waitTime);
        //다시 Idle상태로 전환.
        myState = GoblinState.Idle;
    }


    //이동 상태일 경우의 동작 (moveToTarget,Patrol)
    void OnMove()
    {
       switch(myState)
        {   //정찰 상태인 경우
            case GoblinState.Patrol:
                //이동할 임의의 위치가 지정이 되었다면.
                if(TargetPosition != Vector3.zero)
                {   //목표위치와 나와의 위치를 계산.
                    Vector3 Diff = TargetPosition - myTransform.position;
                    if(Diff.magnitude < 1.0f)
                    {
                        StartCoroutine(OnWait());
                        return;
                    }
                    //목표위치로 이동.
                    Vector3 Direction = Diff.normalized;
                    Vector3 moveAmount = Direction * MoveSpeed * Time.deltaTime;
                    myTransform.Translate(moveAmount, Space.World);
                    myTransform.LookAt(TargetPosition);
                }
                break;
                //타겟으로 이동하는 상태일 경우.
            case GoblinState.MoveToTarget:
                if(TargetPlayer != null)
                {
                    Vector3 Diff = TargetPlayer.transform.position - myTransform.position;
                    if(Diff.magnitude < AttackRange)
                    {
                        myState = GoblinState.Attack;
                        return;
                    }
                    Vector3 Direction = Diff.normalized;
                    Vector3 moveAmount = Direction * MoveSpeed * Time.deltaTime;
                    myTransform.Translate(moveAmount, Space.World);
                    myTransform.LookAt(TargetPlayer.transform);
                }
                break;
        }
    }
    //공격 상태일 경우의 동작
    void OnAttack()
    {
        Vector3 Diff = TargetPlayer.transform.position - myTransform.position;
        if(Diff.magnitude > AttackRange + 1.0f)
        {
            myState = GoblinState.MoveToTarget;
        }
    }
    //고블린의 상태에 따라 동작을 구분합니다.
    void CheckState()
    {
        switch(myState)
        {
            case GoblinState.Idle:
                OnIdle();
                break;
            case GoblinState.MoveToTarget:
            case GoblinState.Patrol:
                OnMove();
                break;
            case GoblinState.Attack:
                OnAttack();
                break;
        }
    }
    //애니메이션 컨트롤
    void AnimationControl()
    {
        switch(myState)
        {
            case GoblinState.Wait:
            case GoblinState.Idle:
                myAnimation.CrossFade(IdleAnimation.name);
                break;
            case GoblinState.Patrol:
            case GoblinState.MoveToTarget:
                myAnimation.CrossFade(MoveAnimation.name);
                break;
            case GoblinState.Attack:
                myAnimation.CrossFade(AttackAnimation.name);
                break;
            case GoblinState.Die:
                myAnimation.CrossFade(DieAnimation.name);
                break;
        }
    }
    //충돌 체크 처리.
    private void OnTriggerEnter(Collider other)
    {   //나를 때린 게 플레이어의 칼이라면
        if(other.tag == "sword")
        {   //데미지 이펙트를 생성하고.
            Instantiate(DamageEffect, other.transform.position, Quaternion.identity);
            //피격 애니메이션을 재생합니다.
            myAnimation.CrossFade(DamageAnimation.name);
            //피격 트위닝 재생.
            DamageTweenEffect();
        }
    }
    //TriggerDetector에서 Player 태그를 가진 게임오브젝트가 충돌되었다면 호출.
    public void SetTarget(GameObject target)
    {
        TargetPlayer = target;
        myState = GoblinState.MoveToTarget;
    }
    //피격 이펙트 트위닝
    void DamageTweenEffect()
    {   //이미 이펙트 트위닝 중이라면 다시 호출할 필요가 없습니다.
        if(effectTweener != null && effectTweener.isComplete == false)
        {
            return;
        }
        Color colorTo = Color.red;
        effectTweener = HOTween.To(skinBody.material, 0.2f, new TweenParms()
            .Prop("color", colorTo)
            .Loops(1, LoopType.Yoyo)
            .OnStepComplete(DamageFinished));
    }
    void DamageFinished()
    {
        skinBody.material.color = Color.white;
    }
}
