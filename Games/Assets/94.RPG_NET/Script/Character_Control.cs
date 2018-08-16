using UnityEngine;
using System.Collections;

public class Character_Control : MonoBehaviour
{

	public float MoveSpeed = 10.0f;

	private float Gravity = 9.8f;

	public float RotateSpeed = 500.0f;

	private Vector3 MoveDirection = Vector3.zero;

	private float VerticalSpeed = 0.0f;

	private CharacterController controller;

	public Animation AnimationController;
	

	public GameObject AttackCollider;
	// Skill Effect GameObject 
	public GameObject SkillPrefab;
	

	public AnimationClip IdleAnim;		
	public AnimationClip WalkAnim;	
	public AnimationClip RunAnim;		
	public AnimationClip AttackAnim;	
	public AnimationClip SkillAnim; 
	
	public float IdleAnimSpeed = 1.0f;	
	public float WalkAnimSpeed = 1.0f;	
	public float RunAnimSpeed = 1.0f;		
	public float AttackAnimSpeed = 1.0f;
	public float SkillAnimSpeed = 1.0f;
	
	private Vector3 LastPosition = Vector3.zero;
	private bool IsMove = false;
    //private float MoveDirSpeed = 0.0f;
	
	enum CharacterState {
		Idle = 0,
		Walk = 1,
		Run = 2,
		Attack = 3,
		Skill = 4,
		Size,
	}
	

	private CharacterState CharState;
	
	private CollisionFlags collisionflags = CollisionFlags.CollidedBelow;
	
	public NetworkPlayer owner;

	[RPC]
	void SetPlayer(NetworkPlayer player){

		Debug.Log("SetPlayer Called");	
		// player setting
		owner = player;
		
		if(Network.isServer)
		{
			gameObject.AddComponent<PlayerLocal>();
		}else
		{
			gameObject.AddComponent<PlayerRemote>();
		}
	
	}

	
	void Awake()
	{
		DontDestroyOnLoad(gameObject);
	}
	
 	void Move()
	{
				
		//0925
		if(this.owner != null && Network.player == owner)
		{

			Transform cameraTransform = Camera.main.transform;		

			Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);

			forward.y = 0.0f;

			forward = forward.normalized;

			Vector3 right = new Vector3(forward.z , 0.0f ,-forward.x);

			float v = Input.GetAxisRaw("Vertical");
			float h = Input.GetAxisRaw("Horizontal");

			Vector3 targetDirection = h * right + v * forward;

            if (targetDirection.magnitude > 0.0f)
            {
                GetComponent<NetworkView>().RPC("SendCharacterMoveInput", RPCMode.OthersBuffered, targetDirection);
            }
            

            this.CharacterMove(targetDirection);
		
		}
		
		
		
	}

    void CharacterMove(Vector3 targetDirection)
    {
        if (controller == null) return;
        
        MoveDirection = Vector3.RotateTowards(MoveDirection, targetDirection, RotateSpeed * Mathf.Deg2Rad * Time.deltaTime, 1000);

        MoveDirection = MoveDirection.normalized;

        Vector3 grav = new Vector3(0.0f, VerticalSpeed, 0.0f);

        Vector3 movement = (MoveDirection * MoveSpeed * Time.deltaTime) + grav;

        collisionflags = controller.Move(movement);

        //MoveDirSpeed = targetDirection.magnitude;

    }

    [RPC]
    void SendCharacterMoveInput(Vector3 targetDirection)
    {
        this.CharacterMove(targetDirection);
    }
	
	void BodyDirection()
	{
		
		Vector3 horizontalVelocity = controller.velocity;
		
		horizontalVelocity.y = 0.0f;
		
		if( horizontalVelocity.magnitude > 0 )
		{    
			Vector3 trans = horizontalVelocity.normalized;
			
			transform.forward = Vector3.Lerp(transform.forward,trans, 0.5f);
		}
	}
	
	
	
	// Use this for initialization.
	void Start () { 
		
		
		WalkAnim.wrapMode = WrapMode.Loop;
		IdleAnim.wrapMode = WrapMode.Loop;
		RunAnim.wrapMode = WrapMode.Loop;
		
		AttackAnim.wrapMode = WrapMode.Clamp;
		SkillAnim.wrapMode = WrapMode.Clamp;
		
		
		controller = gameObject.GetComponent<CharacterController>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		Move();
		
		BodyDirection();
		
		AnimationPlay();
		
		CheckState();
		
		ApplyGravity();
		// Input Key~
		InputCommand();
		
		StateChangeAttackCollider();
		
		LastPosition = transform.position;
	}
	
	void AnimationPlay() 
	{    
		if(AnimationController)
		{   
			switch(CharState)
			{
			case CharacterState.Idle:
				AnimationController.CrossFade(IdleAnim.name);
				break;
				
			case CharacterState.Walk:
				
				AnimationController.CrossFade(WalkAnim.name);
				break;
				
			case CharacterState.Attack:
				
				AnimationController.CrossFade(AttackAnim.name);
				
				if(AnimationController[AttackAnim.name].normalizedTime > 0.9f)
				{
					AnimationController[AttackAnim.name].normalizedTime = 0.0f;
					CharState = Character_Control.CharacterState.Idle;
				}
				
				break;
			case CharacterState.Skill:
				
				AnimationController[SkillAnim.name].speed = SkillAnimSpeed;
				AnimationController.CrossFade(SkillAnim.name);
				if(AnimationController[SkillAnim.name].normalizedTime > 0.9f)
				{
					AnimationController[SkillAnim.name].normalizedTime = 0.0f;
					CharState = Character_Control.CharacterState.Idle;
				}
				
				break;
			}
		}
	}
	
	void CheckState()
	{
        if (CharState == Character_Control.CharacterState.Skill)
        {
            return;
        }


        if (CharState == Character_Control.CharacterState.Attack)
        {
            return;
        }
		
		if(this.owner != null && Network.player == owner)
		{
	        if (controller.velocity.magnitude < 0.1f)
	        {
	            CharState = Character_Control.CharacterState.Idle;
	        }
	        else
	        {
	            CharState = Character_Control.CharacterState.Walk;
	        }
			 
		}else
		{
			if(LastPosition != Vector3.zero && Vector3.Distance(transform.position, LastPosition) > 0.0f)
			{
				CharState = Character_Control.CharacterState.Walk;
			}else
			{
				CharState = Character_Control.CharacterState.Idle;
			}
		}
		
		//0925
		if(this.owner != null && Network.player == owner)
		{
            

			if(Input.GetMouseButtonDown(0))
			{
                GetComponent<NetworkView>().RPC("SendInputButton", RPCMode.AllBuffered, 0);
				//CharState = Character_Control.CharacterState.Attack;
			}
			
			if(Input.GetMouseButtonDown(1))
			{
                GetComponent<NetworkView>().RPC("SendInputButton", RPCMode.AllBuffered, 1);
				//GameObject.Instantiate(SkillPrefab, transform.position, transform.rotation);
				//CharState = Character_Control.CharacterState.Skill;
			}
		}
	}

    [RPC]
    void SendInputButton(int button)
    {
        if (button == 0)
        {
            CharState = Character_Control.CharacterState.Attack;
        }
        else if (button == 1)
        {
            GameObject.Instantiate(SkillPrefab, transform.position, transform.rotation);
            CharState = Character_Control.CharacterState.Skill;
        }
    }
	
	
	// Debug 창용 GUI.
	void OnGUI()
	{	
		GUI.color = Color.red;
		
		GUI.Label(new Rect(10,10,200,100), CharState.ToString());
		
		GUI.Label(new Rect(10,30,200,100), transform.position.ToString());
		
	}
	
	bool IsGrounded ()
	{	
		
		return ( collisionflags & CollisionFlags.CollidedBelow ) != 0;
	}
	
	void ApplyGravity ()
	{	
		if(IsGrounded())
		{	
			VerticalSpeed = 0.0f;
		}else
		{	
			VerticalSpeed = VerticalSpeed - Gravity * Time.deltaTime;
		}
	}
	
	void InputCommand()
	{
		Camera main = Camera.main;
		if(Input.GetKeyDown("q"))
		{
			main.transform.gameObject.SendMessage("ChangeCameraState", Camera_Control.CameraViewPointState.FIRST);
		}
		if(Input.GetKeyDown("w"))
		{
			main.transform.gameObject.SendMessage("ChangeCameraState", Camera_Control.CameraViewPointState.SECOND);
		}
		if(Input.GetKeyDown("e"))
		{
			main.transform.gameObject.SendMessage("ChangeCameraState", Camera_Control.CameraViewPointState.THIRD);
		}
		
		
		
	}
	
	void StateChangeAttackCollider()
	{
		if(AttackCollider == null)
		{
			Debug.LogError("Attack Collider is null~");
		}
		
		switch(CharState)
		{
		case CharacterState.Idle:
		case CharacterState.Run:
		case CharacterState.Walk:
			
			AttackCollider.active = false;
			break;
		case CharacterState.Attack:
		case CharacterState.Skill:
			
			AttackCollider.active = true;
			break;
		}
	}
}























