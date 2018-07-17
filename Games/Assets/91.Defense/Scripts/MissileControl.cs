using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControl : MonoBehaviour {
    //이동속도.
    public float Speed = 3.0f;
    private Transform myTransform = null;
    //타겟 위치.
    public Vector3 TargetPosition = Vector3.zero;//0.0f,0.0f,0.0f
    //타겟
    public GameObject Target = null;
	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
    //타워에서 미사일에게 타겟을 알려줍니다.
    public void SetTarget(GameObject target)
    {   //타워가 전달해준 타겟을 저장하고.
        Target = target;
        //현재 타겟의 위치를 목표 위치로 지정해 놓습니다.
        TargetPosition = Target.transform.position;
    }

    void CreateEffect()
    {
        GameObject effect = (GameObject)Instantiate(
            Resources.Load("Prefabs/Explosion2"),
            myTransform.position,
            myTransform.rotation);
        SphereCollider sphere = effect.AddComponent<SphereCollider>();//구충돌체를달아주
        sphere.radius = 1.0f; //반경은 1.0..
        sphere.isTrigger = true; //Trigger Collider.
        Rigidbody rigid = effect.AddComponent<Rigidbody>();
        rigid.useGravity = false;//Use Gravity 는 해제.
        effect.tag = "AttackCollider";
        int damage = GetComponent<DamageComponent>().GetDamage();
        effect.AddComponent<DamageComponent>().SetDamage(damage);
        //타워로부터 넘겨받은 타워의 ID를 이펙트에게 부여합니다.
        effect.GetComponent<DamageComponent>().OwnerID = GetComponent<DamageComponent>().OwnerID;
    }

	// Update is called once per frame
	void Update () {
        //타겟이 살아있다면.
        if(Target != null)
        {
            //목표위치 - 내위치 = 방향을 가리키고 있는 벡터.
            Vector3 Diff = TargetPosition - myTransform.position;
            //목표위치 내위치 사이의 거리값.
            float distance = Vector3.Distance(TargetPosition, myTransform.position);
            if(distance < 0.1f)
            {
                CreateEffect();
                //미사일 없애기.
                Destroy(gameObject);
            }
            //방향 = 방향을 가리키고 있는 벡터에서 크기를 제거.
            Vector3 direction = Diff.normalized;
            //이동 = 방향 X 속도 X 경과시간, 카메라 축을 중심으로 이동합니다.
            myTransform.Translate(direction * Speed * Time.deltaTime, Space.World);
            //목표위치를 바라보도록 회전.
            myTransform.LookAt(TargetPosition);
        }else
        {   //타겟이 죽었다면 미사일 파괴.
            Destroy(gameObject);
        }
		
	}
}
