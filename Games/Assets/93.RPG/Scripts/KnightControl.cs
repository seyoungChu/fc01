using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightControl : MonoBehaviour
{
    //이동관련.
    public float MoveSpeed = 2.0f;
    public float RunSpeed = 3.0f;
    public float RotateSpeed = 100.0f;
    public float VelocitySpeed = 0.1f;
    public Vector3 MoveDirection = Vector3.zero;
    private CharacterController myCharacterController = null;
    private CollisionFlags collisionFlags = CollisionFlags.None;
    private Vector3 Velocity = Vector3.zero;
    //애니메이션 관련.
    private Animation myAnimation = null;
    public AnimationClip IdleAnimation = null;//대기 애니메이션클립.
    public AnimationClip WalkAnimation = null;//이동 애니메이션 클립.
    public AnimationClip RunAnimation = null;//달리기 애니메이션 클립.
    public AnimationClip Attack1Animation = null;//공격모션 1
    public AnimationClip Attack2Animation = null;//공격모션 2
    public AnimationClip Attack3Animation = null;//공격모션 3
    public AnimationClip Attack4Animation = null;//공격모션 4
    //스킬 애니메이션 관련.
    public AnimationClip SkillAnimation = null;//스킬 애니메이션.
    public GameObject SkillEffect = null; //스킬에 사용할 이펙트.

    //스테이트 관련.
    public enum KnightState { None, Idle, Walk, Run, Attack, Skill, Die }
    public KnightState myState = KnightState.None;

    //공격 관련 스테이트.
    public enum KnightAttack { Attack1, Attack2, Attack3, Attack4 }
    public KnightAttack attackState = KnightAttack.Attack1;
    public bool NextAttack = false;
    //중력 관련.
    private float Gravity = 9.8f;
    private float VerticalSpeed = 0.0f;
    //공격중에는 이동하지 않습니다.
    private bool DisableMove = false;
    //공격이펙트 관련.
    public TrailRenderer AttackTrailRenderer = null;
    public CapsuleCollider AttackCapsuleCollider = null;

    // Use this for initialization
    void Start()
    {
        //30프레임 고정.
        //Application.targetFrameRate = 30;
        //vSync를 꺼줍니다.
        //QualitySettings.vSyncCount = 0;


        myCharacterController = GetComponent<CharacterController>();
        myState = KnightState.Idle;
        //기본 애니메이션 세팅.
        myAnimation = GetComponent<Animation>();
        myAnimation.playAutomatically = false;
        myAnimation.Stop();
        myAnimation[IdleAnimation.name].wrapMode = WrapMode.Loop;
        myAnimation[WalkAnimation.name].wrapMode = WrapMode.Loop;
        myAnimation[RunAnimation.name].wrapMode = WrapMode.Loop;
        myAnimation[Attack1Animation.name].wrapMode = WrapMode.Once;
        myAnimation[Attack2Animation.name].wrapMode = WrapMode.Once;
        myAnimation[Attack3Animation.name].wrapMode = WrapMode.Once;
        myAnimation[Attack4Animation.name].wrapMode = WrapMode.Once;
        //공격애니메이션 클립에 애니메이션 종료 이벤트를 추가합니다.
        AddAnimationEvent("AttackAnimationFinish", Attack1Animation);
        AddAnimationEvent("AttackAnimationFinish", Attack2Animation);
        AddAnimationEvent("AttackAnimationFinish", Attack3Animation);
        AddAnimationEvent("AttackAnimationFinish", Attack4Animation);
        //스킬애니메이션 클립에 애니메이션 종료이벤트를 추가.
        myAnimation[SkillAnimation.name].wrapMode = WrapMode.Once;//한번만 재생.
        AddAnimationEvent("SkillAnimationFinish", SkillAnimation);//스킬 애니메이션이벤트

    }
    //애니메이션 클립에 새로운 애니메이션 이벤트를 추가하는 함수입니다.
    void AddAnimationEvent(string funcName, AnimationClip clip)
    {
        //추가될 새로운 애니메이션 이벤트를 생성하고.
        AnimationEvent newEvent = new AnimationEvent();
        //애니메이션이벤트에서 호출할 함수 이름.
        newEvent.functionName = funcName;
        //애니메이션이벤트가 발생될 시간.
        newEvent.time = clip.length - 0.1f;
        //애니메이션 클립에 원하는 애니메이션이벤트를 추가합니다.
        clip.AddEvent(newEvent);
    }
    //스킬애니메이션 재생이 완료되었을때 호출되는 함수.
    void SkillAnimationFinish()
    {
        //스킬애니메이션이 끝나면 다시 이동시켜줍니다.
        DisableMove = false;
        //현재 내위치에서
        Vector3 position = transform.position;
        //2 유닛정도 정면에 위치한 곳을 구해서.
        position += transform.forward * 2.0f;
        //이펙트를 생성합니다.
        Instantiate(SkillEffect, position, Quaternion.identity);
        //스킬이 끝나면 내 상태를 대기상태로 돌려놓습니다.
        myState = KnightState.Idle;
    }

    //공격애니메이션의 재생이 완료되었을때 호출되는 함수.
    void AttackAnimationFinish()
    {   //공격중에 마우스 왼쪽버튼을 클릭한 적이 없다면.
        if(NextAttack == false)
        {
            DisableMove = false;
            myState = KnightState.Idle;
            attackState = KnightAttack.Attack1;
        }else
        {
            //공격중에 마우스 왼쪽버튼을 클릭했었다면.
            NextAttack = false;
            switch(attackState)
            {
                case KnightAttack.Attack1:
                    attackState = KnightAttack.Attack2;
                    break;
                case KnightAttack.Attack2:
                    attackState = KnightAttack.Attack3;
                    break;
                case KnightAttack.Attack3:
                    attackState = KnightAttack.Attack4;
                    break;
                case KnightAttack.Attack4:
                    attackState = KnightAttack.Attack1;
                    break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //이동기능.
        Move();
        //캐릭터가 바라보는 방향을 이동방향으로 돌려주는 기능.
        BodyDirection();
        //상태에 따라 애니메이션을 재생시켜주는 기능.
        AnimationControl();
        //조건에 따라 상태를 변경시켜주는 기능.
        CheckState();
        //마우스 입력을 받아 공격 상태를 제어합니다.
        InputControl();
        //캐릭터에 중력을 적용시켜 줍니다.
        ApplyGravity();
        //공격 상태일때만 이펙트와 충돌체를 제어.
        AttackEffectControl();
    }
    //캐릭터 이동함수.
    void Move()
    {
        //이동이 막혀있다면 이동하지 않습니다.
        if(DisableMove == true)
        {
            return;
        }
        //메인 카메라의 트랜스폼 컴포넌트
        Transform cameraTransform = Camera.main.transform;
        //메인 카메라의 Forward 방향의 World 벡터를 얻어서.
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        //Y축 값과 크기는 사용하지 않습니다.
        forward.y = 0.0f;
        forward = forward.normalized;
        //내적의 직교의 성질을 이용하여 right벡터를 구합니다.
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

        float vertical = Input.GetAxisRaw("Vertical"); //키보드의 위,아래키 값.
        float horizontal = Input.GetAxisRaw("Horizontal");//키보드의 좌,우 방향키 값.
        //이동 방향
        Vector3 targetDirection = horizontal * right + vertical * forward;
        //이전에 이동했던 이동방향과 가고자하는 이동방향을 보간합니다.
        MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection,
            RotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000.0f);
        MoveDirection = MoveDirection.normalized;//크기는 제거.방향만 사용.
        float speed = MoveSpeed;
        if (myState == KnightState.Run)
        {
            speed = RunSpeed;
        }
        Vector3 grav = new Vector3(0.0f, VerticalSpeed, 0.0f);
        Vector3 moveAmount = (MoveDirection * speed * Time.deltaTime) + grav;
        //캐릭터 컨트롤러의 Move함수로 캐릭터를 이동시키고 이동시키면서 
        //충돌된 정보를 collisionFlag를 통해 알 수 있습니다.
        collisionFlags = myCharacterController.Move(moveAmount);

    }

    private void OnGUI()
    {
        //현재 상태를 출력.
        GUILayout.Label("State :" + myState.ToString());
        //현재 재생되고 있는 애니메이션 이름과 재생 타임을 출력.
        GUILayout.Label(myAnimation.clip.name + " : "
            + myAnimation[myAnimation.clip.name].normalizedTime.ToString());
        //충돌 플래그 확인용
        GUILayout.Label("Collsion Flag:" + collisionFlags.ToString());
        //이동을 하고있다면.
        if (myCharacterController.velocity != Vector3.zero)
        {
            GUILayout.Label("Velocity: " + myCharacterController.velocity.ToString()
                + " / Magnitude:" + myCharacterController.velocity.magnitude.ToString());
        }
    }
    //내 캐릭터의 현재 이동 속도.(값으로 반환)
    private float GetVelocitySpeed()
    {
        //멈춰있다면
        if (myCharacterController.velocity == Vector3.zero)
        {
            Velocity = Vector3.zero;
        }
        else
        {   //이동하고 있다면.
            Vector3 currentVelocity = myCharacterController.velocity;
            currentVelocity.y = 0.0f;
            Velocity = Vector3.Lerp(Velocity, currentVelocity,
                VelocitySpeed * Time.fixedDeltaTime);//velocityspeed = 0.1f
            //fixedDeltaTime
        }
        return Velocity.magnitude;
    }

    void BodyDirection()
    {   //이동하고 있다면.
        Vector3 newForward = myCharacterController.velocity;
        newForward.y = 0.0f;
        //현재 내가 바라보고 있는 방향을 가고자 하는 방향으로 조금씩 돌려줍니다.
        transform.forward = Vector3.Lerp(transform.forward, newForward, 0.5f);

    }
    //애니메이션을 재생하는 함수.
    void AnimationPlay(AnimationClip clip)
    {
        myAnimation.clip = clip;//기본 애니메이션 클립 변경.
        myAnimation.CrossFade(clip.name);//애니메이션 재생.
    }
    //애니메이션을 재생시켜줍니다.
    void AnimationControl()
    {
        switch (myState)
        {
            case KnightState.Idle:
                AnimationPlay(IdleAnimation);
                break;
            case KnightState.Walk:
                AnimationPlay(WalkAnimation);
                break;
            case KnightState.Run:
                AnimationPlay(RunAnimation);
                break;
            case KnightState.Attack:
                //현재 나의 공격상태에따라 해당하는 공격 애니메이션을 재생.
                AttackAnimationControl();
                break;
            case KnightState.Skill:
                AnimationPlay(SkillAnimation);
                break;
        }
    }

    void CheckState()
    {
        float velocitySpeed = GetVelocitySpeed();
        switch (myState)
        {
            case KnightState.Idle:
                //이동을 시작했다면.
                if (velocitySpeed > 0.1f)
                {
                    myState = KnightState.Walk;
                }
                break;
            case KnightState.Walk:
                if (velocitySpeed > 1.8f)
                {
                    myState = KnightState.Run;
                }
                if (velocitySpeed < 0.1f)
                {
                    myState = KnightState.Idle;
                }
                break;
            case KnightState.Run:
                if (velocitySpeed < 1.8f)
                {
                    myState = KnightState.Walk;
                }
                if (velocitySpeed < 0.1f)
                {
                    myState = KnightState.Idle;
                }
                break;
            case KnightState.Attack:
                DisableMove = true;
                break;
            case KnightState.Skill:
                DisableMove = true;
                break;
        }
    }

    void InputControl()
    {
        //마우스 왼쪽버튼을 클릭했다면.
        if(Input.GetMouseButtonDown(0) == true)
        {
            //현재 상태가 공격상태가 아니라면.
            if(myState != KnightState.Attack)
            {   //공격 상태로 변경.
                myState = KnightState.Attack;
                //연속 공격의 상태도 첫번째 공격 스테이트로 변경.
                attackState = KnightAttack.Attack1;
            }else
            {
                //현재 상태가 공격 상태라면.
                switch(attackState)
                {
                    case KnightAttack.Attack1:
                        //첫번째 공격애니메이션이 10%이상 재생되었다면.
                        if(myAnimation[Attack1Animation.name].normalizedTime > 0.1f)
                        {
                            NextAttack = true;
                        }
                        break;
                    case KnightAttack.Attack2:
                        if(myAnimation[Attack2Animation.name].normalizedTime > 0.1f)
                        {
                            NextAttack = true;
                        }
                        break;
                    case KnightAttack.Attack3:
                        if(myAnimation[Attack3Animation.name].normalizedTime > 0.1f)
                        {
                            NextAttack = true;
                        }
                        break;
                    case KnightAttack.Attack4:
                        if(myAnimation[Attack4Animation.name].normalizedTime > 0.1f)
                        {
                            NextAttack = true;
                        }
                        break;

                }
            }
        }
        //마우스 오른쪽 버튼을 클릭했다면.
        if(Input.GetMouseButtonDown(1) == true)
        {   //현재 공격중이라면 연속공격을 취소시키고.
            if(myState == KnightState.Attack)
            {
                attackState = KnightAttack.Attack1;
                NextAttack = false;
            }
            //내 상태를 스킬 상태로 변경합니다.
            myState = KnightState.Skill;
        }
    }
    //공격용 애니메이션 재생
    void AttackAnimationControl()
    {   //현재 나의 공격 상태에 따라 애니메이션을 재생시켜줍니다.
        switch(attackState)
        {
            case KnightAttack.Attack1:
                AnimationPlay(Attack1Animation);
                break;
            case KnightAttack.Attack2:
                AnimationPlay(Attack2Animation);
                break;
            case KnightAttack.Attack3:
                AnimationPlay(Attack3Animation);
                break;
            case KnightAttack.Attack4:
                AnimationPlay(Attack4Animation);
                break;
        }
    }
    //캐릭터에 중력을 적용하는 함수.
    void ApplyGravity()
    {
        //현재 바닥과 충돌했다면.
        if ((collisionFlags & CollisionFlags.CollidedBelow) != 0)
        {
            VerticalSpeed = 0.0f;
        }else
        {
            //공중에 떠있다면.
            VerticalSpeed = VerticalSpeed - Gravity * Time.deltaTime;
        }
    }
    //공격시에 트레일과 충돌체를 제어합니다.
    void AttackEffectControl()
    {
        switch(myState)
        {
            case KnightState.Attack:
            case KnightState.Skill:
                AttackTrailRenderer.enabled = true;
                AttackCapsuleCollider.enabled = true;
                break;
            default:
                AttackTrailRenderer.enabled = false;
                AttackCapsuleCollider.enabled = false;
                break;

        }
    }
}
