using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightControl : MonoBehaviour {
    //이동관련.
    public float MoveSpeed = 2.0f;
    public float RunSpeed = 3.0f;
    public float RotateSpeed = 100.0f;
    [Range(0.0f, 1.0f)]
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
    
    //스테이트 관련.
    public enum KnightState { None, Idle, Walk, Run, Attack, Skill, Die }
    public KnightState myState = KnightState.None;

	// Use this for initialization
	void Start () {
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

	}
	
	// Update is called once per frame
	void Update () {
        //이동기능.
        Move();
        //캐릭터가 바라보는 방향을 이동방향으로 돌려주는 기능.
        BodyDirection();
        //상태에 따라 애니메이션을 재생시켜주는 기능.
        AnimationControl();
        //조건에 따라 상태를 변경시켜주는 기능.
        CheckState();
	}
    //캐릭터 이동함수.
    void Move()
    {
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
        if(myState == KnightState.Run)
        {
            speed = RunSpeed;
        }
        Vector3 moveAmount = MoveDirection * speed * Time.deltaTime;
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

        //이동을 하고있다면.
        if(myCharacterController.velocity != Vector3.zero)
        {
            GUILayout.Label("Velocity: " + myCharacterController.velocity.ToString()
                + " / Magnitude:" + myCharacterController.velocity.magnitude.ToString());
        }
    }
    //내 캐릭터의 현재 이동 속도.(값으로 반환)
    private float GetVelocitySpeed()
    {
        //멈춰있다면
        if(myCharacterController.velocity == Vector3.zero)
        {
            Velocity = Vector3.zero;
        }else
        {   //이동하고 있다면.
            Velocity = Vector3.Lerp(Velocity, myCharacterController.velocity,
                VelocitySpeed);//velocityspeed = 0.1f
        }
        return Velocity.magnitude;
    }

    void BodyDirection()
    {   //이동하고 있다면.
        if(GetVelocitySpeed() > 0.0f)
        {
            Vector3 newForward = myCharacterController.velocity;
            newForward.y = 0.0f;
            //현재 내가 바라보고 있는 방향을 가고자 하는 방향으로 조금씩 돌려줍니다.
            transform.forward = Vector3.Lerp(transform.forward, newForward,0.5f);
        }
    }
    //애니메이션을 재생하는 함수.
    void AnimationPlay(AnimationClip clip)
    {
        myAnimation.clip = clip;//기본 애니메이션 클립 변경.
        myAnimation.CrossFade(clip.name);//애니메이션 재생.
    }
    void AnimationControl()
    {
        switch(myState)
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
                break;
            case KnightState.Skill:
                break;
        }
    }

    void CheckState()
    {
        switch(myState)
        {
            case KnightState.Idle:
                //이동을 시작했다면.
                if(GetVelocitySpeed() > 0.1f)
                {
                    myState = KnightState.Walk;
                }
                break;
            case KnightState.Walk:
                if(GetVelocitySpeed() > 1.8f)
                {
                    myState = KnightState.Run;
                }
                if(GetVelocitySpeed() < 0.1f)
                {
                    myState = KnightState.Idle;
                }
                break;
            case KnightState.Run:
                if(GetVelocitySpeed() < 1.8f)
                {
                    myState = KnightState.Walk;
                }
                if(GetVelocitySpeed() < 0.1f)
                {
                    myState = KnightState.Idle;
                }
                break;
            case KnightState.Attack:
                break;
            case KnightState.Skill:
                break;
        }
    }
}
