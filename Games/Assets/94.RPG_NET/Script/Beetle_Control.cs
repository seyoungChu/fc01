using UnityEngine;
using System.Collections;

public class Beetle_Control : MonoBehaviour {
	
	private Vector3 TargetPos;
	public float BeetleMoveSpeed = 10.0f;
	
	
	public Animation AnimationController;
	public AnimationClip MoveAnim;
	public AnimationClip DieAnim;
	public AnimationClip AttackAnim;
	
	public float MoveAnimSpeed = 1.0f;
	public float DieAnimSpeed = 1.0f;
	public float AttackAnimSpeed = 1.0f;
	
	public GameObject DeathPrefab;
	public GameObject DamagePrefab;
	private GameObject damage;
	
	
	public int HP = 100;
	

	// Use this for initialization
	void Start () {
		
		TargetPos = new Vector3(0.0f, 0.0f,0.0f);
		
		AnimationController.wrapMode = WrapMode.Loop;
		AnimationController[DieAnim.name].wrapMode = WrapMode.Clamp;
		
		AnimationController.CrossFade(MoveAnim.name);
		
	
	}
	
	void SearchTargetPos()
	{
		GameObject player = GameObject.FindWithTag("Player");
		
		if(player)
		{
			TargetPos = player.transform.position;
		}else
		{
			//0925
			//Debug.LogError("Can't Find The Player~~!!!!!!!!!");
		}
		
	}
	
	
	// Update is called once per frame
	void Update () {
		
		bool isdead = IsDead();
		if(isdead == true)
		{
			return;
		}
		
		SearchTargetPos();
		
		Vector3 DiffVector = TargetPos - transform.position;
		if(DiffVector.magnitude < 2.0f)
		{
			return;
		}
		
		Vector3 Direction = DiffVector.normalized;
		
		transform.Translate(Direction * BeetleMoveSpeed * Time.deltaTime, Space.World);
		
		transform.LookAt(TargetPos);
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log("[Called OnTrigger Enter] : " + other.gameObject.name);
		if(other.tag == "AttackCollider")
		{
			damage = GameObject.Instantiate(DamagePrefab, transform.position, transform.rotation) as GameObject;
			damage.SendMessage("SetText", "50");
			
			this.HP -= 50;
		}
	}
	
	bool IsDead()
	{
		if(this.HP <= 0)
		{
			AnimationController.CrossFade(DieAnim.name);
			if(AnimationController[DieAnim.name].normalizedTime > 0.9f)
			{
				AnimationController[DieAnim.name].normalizedTime = 0.91f;
				GameObject.Instantiate(DeathPrefab, gameObject.transform.position , gameObject.transform.rotation);
				//GameObject.Destroy(gameObject);
				Network.Destroy(gameObject);
			}
			
			return true;
		}
		
		return false;
	}
}

























