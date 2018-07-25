using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretControl : MonoBehaviour {
    public enum TurretState { None = 0, Idle, Move, Rotate, Attack, Wait }
    public TurretState myState = TurretState.None;

    public float MoveSpeed = 5.0f;
    private Transform myTransform = null;

    public GameObject TargetPlayer = null; //타겟 플레이어
    public Vector3 GoalPosition = Vector3.zero; //내가 이동하고 싶은 지점.
    public float RotateSpeed = 100.0f; //회전 속도.
    public GameObject TurretMissile = null; // 총알 프리팹.

    private TextMesh DebugText = null; //State출력용 3D Text 컴포넌트.
    private Transform AttackSocket = null; // 실제 총알이 발사될 위치의 트랜스폼.


	// Use this for initialization
	void Start () {
        myState = TurretState.Idle;
        myTransform = GetComponent<Transform>();
        DebugText = transform.Find("Text").GetComponent<TextMesh>();
        AttackSocket = transform.Find("AttackSocket");
	}
	
	// Update is called once per frame
	void Update () {
        DebugText.text = myState.ToString();
        switch(myState)
        {
            case TurretState.Idle:
                OnIdle();
                break;
            case TurretState.Move:
                OnMove();
                break;
            case TurretState.Rotate:
                OnRotate();
                break;
            case TurretState.Attack:
                OnAttack();
                break;
        }
	}
    void OnIdle()
    {
        TargetPlayer = GameObject.FindWithTag("Player");
        if(TargetPlayer != null)
        {
            myState = TurretState.Move;
            //이동할 위치를 -4부터 4까지의 높이를 갖는 위치를 생성합니다.
            GoalPosition = new Vector3(0.0f, Random.Range(-4.0f, 4.0f), 0.0f);
        }
    }
    void OnMove()
    {
        Vector3 Diff = GoalPosition - myTransform.position;
        Vector3 Direction = Diff.normalized;
        Direction.x = 0.0f;//y축 값만 사용하기 위해서.x와 z 방향은 없애줍니다.
        Direction.z = 0.0f;
        myTransform.Translate(Direction * MoveSpeed * Time.deltaTime, Space.World);
        float Distance = GoalPosition.y - myTransform.position.y; // 음수일수도,양수일수도
        Distance = Mathf.Abs(Distance); //부호를 없애고 절대값으로만 얻습니다.
        if(Distance < 0.1f) //목표지점에 도착했다면.
        {
            myState = TurretState.Rotate;
        }
    }
    void OnRotate()
    {   //up = (0,1,0)
        myTransform.Rotate(Vector3.up, RotateSpeed * Time.deltaTime);
        Vector3 Diff = TargetPlayer.transform.position - myTransform.position;
        Vector3 Direction = Diff.normalized;
        float Angle = Vector3.Angle(myTransform.forward, Direction);
        Debug.Log("Angle:" + Angle.ToString());
        if(Angle < 5.0f)
        {
            myState = TurretState.Attack;
        }
    }
    void OnAttack()
    {   //TurretMissile을 만들어주고.
        GameObject missile = (GameObject)Instantiate(TurretMissile,
            AttackSocket.position, AttackSocket.rotation);
        missile.AddComponent<TurretMissileControl>();//스크립트 붙여주고
        missile.SendMessage("SetTargetDirection", myTransform.forward);//이동방향 지정.
        //내 상태를 다시 돌려놓습니다.
        myState = TurretState.Wait;
        StartCoroutine(OnWait());//대기 상태의 코루틴 호출.
    }
    IEnumerator OnWait()
    {
        yield return new WaitForSeconds(1.0f);
        myState = TurretState.Idle;
    }
}
