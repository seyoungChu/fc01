//For Lecture

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity : MonoBehaviour {


    [HideInInspector]
    public int UID;

    [HideInInspector]
    public EntityType entitytype; //엔티티종류 - 빌딩,유닛,이펙트

    [HideInInspector]
    public EntityCategory category = EntityCategory.None;

    [HideInInspector]
    public Transform myTransform; //트랜스폼 캐싱


    [HideInInspector]
    public int level; //레벨

    public int hp
    {
        get
        {
            return HP;
        }
        set
        {
            HP = value;
            this.SetHP();
        }
    }

    [HideInInspector]
    private int HP; //hp

    [HideInInspector]
    public int maxHP; //hp

    [HideInInspector]
    public float moveSpeed;

    [HideInInspector]
    public float moveType; //0 = 지상 이동, 1 = 공중 레이어1, 2 = 공중 레이어2(1보다 상위 레이어)

    [HideInInspector]
    public float attackSpeed = 1.5f;

    [HideInInspector]
    public int attackPower = 100;

    [HideInInspector]
    public int defencePower = 0;

    [HideInInspector]
    public float bulletSpeed = 0.0f;

    [HideInInspector]
    public int hit_timing = 0;//틱 : 공격 애니메이션 재생 시 타격판정 하는 타이밍.

    [HideInInspector]
    public int hit_timing_bullet = 0;//bool : 0일경우 근접타격 애니메이션 출력시 타격 타이밍 제어. 1일 경우 투사체 충돌시에 타격 판정.

    [HideInInspector]
    public float searchRange = 0.0f;

    [HideInInspector]
    public float firstAttackDelay = 0.1f;

    //[HideInInspector]
    //public EffectList attack_effect = EffectList.None;
    //[HideInInspector]
    //public EffectList behit_effect = EffectList.None;
    //[HideInInspector]
    //public EffectList die_effect = EffectList.None;
    [HideInInspector]
    public Transform[] fire_points = new Transform[0]; //발사 지점, 유닛, 빌딩, 특수 이펙트 공통적으로 사용해보자.
    [HideInInspector]
    public Transform behit_point = null; //피격 지점, 유닛 , 빌딩, 이펙트 공통적으로 사용해보자.
    [HideInInspector]
    public bool tobeHide_HPbar = false;
    #region 임시테스트코드
    [HideInInspector]
    public GameObject TextMeshObject = null; //텍스트오브젝트
    #endregion

    public virtual void InitEntity(EntityData data)
    {
        
    }

    /// <summary>
    /// Cashings the object.
    /// </summary>
    public virtual void CashingObject()
    {
        
    }

    /// <summary>
    /// 엔티티 업데이트
    /// </summary>
    public virtual void UpdateEntity()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public virtual void DestoryEntity()
    {

    }
    /// <summary>
    /// 
    /// </summary>
    public virtual void OnTargetDestroy()
    {

    }
    /// <summary>
    /// 데미지를 입었을때 호출된다. 빌딩의 경우.
    /// </summary>
    public virtual void OnDamaged(int damage)
    {

    }

    public bool IsDead()
    {
        if(this.HP > 0)
        {
            return false;
        }else
        {
            return true;
        }
    }

    /// <summary>
    /// 텍스트메쉬 on,off
    /// </summary>
    public void ToggleTextMesh(bool bOn)
    {
        if (TextMeshObject == null)
            return;

        if (bOn)
        {
            TextMeshObject.SetActive(false);
        }
        else
        {
            TextMeshObject.SetActive(true);
        }

    }
    /// <summary>
    /// 공격지점과 피격지점 더미 위치를 찾아놓자.
    /// </summary>
    public void FindDummies()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (child.name.ToLower().Contains("firepoint"))
            {
                this.fire_points = new Transform[child.childCount];
                for (int j = 0; j < child.childCount; j++)
                {
                    this.fire_points[j] = child.GetChild(j);
                }
            }
            if (child.name.ToLower().Equals("behit_point"))
            {
                this.behit_point = child;
            }
        }
        if (this.behit_point == null)
        {
            this.behit_point = transform;
        }
    }
    /// <summary>
    /// 공격 지점 찾기.
    /// </summary>
    public Transform GetFirePoint(string dir)
    {
        if (fire_points == null || fire_points.Length == 0)
        {
            return transform;
        }
        Transform retTrans = transform;
        for (int i = 0; i < fire_points.Length; i++)
        {
            if (fire_points[i].name.Contains("_"))  //firepoint_n
            {
                string[] names = fire_points[i].name.Split('_');
                if (names[names.Length - 1].ToLower().Equals(dir.ToLower()))
                {
                    retTrans = fire_points[i];
                    break;
                }

            }
            else
            {
                if (fire_points[i].name.ToLower().Equals(dir.ToLower()))
                {
                    retTrans = fire_points[i];
                    break;
                }
            }
        }
        return retTrans;
    }

    GameObject HPBarObj = null;
    tk2dClippedSprite HPBarProgress = null;
    /// <summary>
    /// 무조건 BattleCollision 레이어와 충돌체가 세팅된 후에 호출되어야 한다.
    /// </summary>
    public void EntityHPBarSetup()
    {
        //빌딩과 유닛.
        if (this.HPBarObj == null)
        {
            //float offsetY = 50.0f;
            //this.HPBarObj = ResourceManager.SingleLoad("UI/IngameUI/HpBar"); //TODO: 체력바로딩을 테이블에서 할것.
            //this.HPBarObj.transform.SetParent(myTransform);
            //this.HPBarObj.transform.localPosition = Vector3.zero;
            //GameObject battlecolObj = TransformHelper.FindChildByComponent<BoxCollider>(gameObject);
            //if (battlecolObj != null)
            //{
            //    BoxCollider box = battlecolObj.GetComponent<BoxCollider>();
            //    if (entitytype == EntityType.Building)
            //    {
            //        offsetY += battlecolObj.transform.localPosition.y; //인터셉터가 차일드로 붙어 있는 경우가 있다.
            //    }
            //    else if (entitytype == EntityType.Unit)
            //    {
            //        offsetY += behit_point.localPosition.y;
            //    }
            //    offsetY += box.center.y + (box.size.y * 0.5f);
            //    this.HPBarObj.transform.localPosition = new Vector3(0.0f, offsetY, 0.0f);
            //}

            //Transform progressTrans = this.HPBarObj.transform.Find("progressbar");
            //if (progressTrans != null)
            //{
            //    this.HPBarProgress = progressTrans.GetComponent<tk2dClippedSprite>();
            //}
            //else
            //{
            //    Debug.LogWarning("progressbar 를 찾을 수 없습니다.");
            //}
        }
    }

    /// <summary>
    /// HP 바에 UI 표시.
    /// </summary>
    public void SetHP()
    {
       
        if (this.hp == 0 || this.maxHP == 0)
        {
            return;
        }
        //HPBar를 굳이 보여주지 않아도 되는 유닛(인터셉터 같은거)은 생성하지 않는다.
        if (this.tobeHide_HPbar == true)
        {
            return;
        }
        //Debug.Log("SetHP Called");

        if (this.HPBarProgress == null)
        {
            EntityHPBarSetup();
        }

        if (this.HPBarProgress != null)
        {
            float value = (this.hp + 0.0f) / (this.maxHP + 0.0f);
            //Debug.Log(value.ToString());
            this.HPBarProgress.ClipRect = new Rect(0.0f, 0.0f, value, 1.0f);
        }
        if (this.HPBarObj != null)
        {
            //만땅이면 꺼지고.
            if (this.hp == this.maxHP)
            {
                if (this.HPBarObj.activeSelf == true)
                {
                    this.HPBarObj.SetActive(false);
                }
            }
            else //HP가 조금이라도 소모됬다면 켜진다.
            {
                if (this.HPBarObj.activeSelf == false)
                {
                    this.HPBarObj.SetActive(true);
                }
            }
        }

    }

    private void OnDisable()
    {
        if (this.HPBarObj != null)
        {
            this.HPBarObj.SetActive(false);
        }
    }

}
