using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControl : MonoBehaviour {

    public float speed = 3.0f;
    private Transform myTransform = null;
    // target's position = 0.0f, 0.0f, 0.0f
    public Vector3 TargetPosition = Vector3.zero;
    // tower's target
    public GameObject Target = null;


	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
	
    public void SetTarget(GameObject target)
    {   
        // tower is saving target = Target
        Target = target;
        // up's Target = TargetPosition
        TargetPosition = Target.transform.position;
    }

    void CreateEffect(){
        GameObject effect = (GameObject)Instantiate(Resources.Load("Prefabs/Explosion2"), myTransform.position, myTransform.rotation);
        SphereCollider sphere = effect.AddComponent<SphereCollider>();
        sphere.radius = 1.0f;
        sphere.isTrigger = true;
        Rigidbody rigid = effect.AddComponent<Rigidbody>();
        rigid.useGravity = false;
        effect.tag = "AttackConllider";
        int damage = GetComponent<DamageComponent>().GetDamage();
        effect.AddComponent<DamageComponent>().SetDamage(damage);
        // 타워로부터 넘겨받은 타워의 ID를 이펙트에게 부여한다
        effect.GetComponent<DamageComponent>().OwnerID = GetComponent<DamageComponent>().OwnerID;
    }

	// Update is called once per frame
	void Update () {
        if(Target != null)
        {
            Vector3 Diff = TargetPosition - myTransform.position;
            //목표 위치와 내 위치 사이의 거리
            float distance = Vector3.Distance(TargetPosition, myTransform.position);
            if(distance < 0.1f)
            {
                CreateEffect();
                Destroy(gameObject);
            }
            Vector3 direction = Diff.normalized;
            myTransform.Translate(direction * speed * Time.deltaTime, Space.World);
            myTransform.LookAt(TargetPosition);
        } else {
            Destroy(gameObject);
        }
	}
}
