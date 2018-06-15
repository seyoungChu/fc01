using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 게임상의 모든 컨트롤러의 베이스 클래스.
/// </summary>
public class VirtualControl : MonoBehaviour
{
    protected FSMSystem fsm; //fsm머신
    protected StateID LastStateID; //이전

    public StateID CurrStateID
    {
        get
        {
            if (fsm == null)
            {
                return StateID.NULLSTATEID;
            }
            return fsm.CurrentStateID;
        }
        set { }
    }


    public FSMSystem FSM
    {
        get 
        {
            if(fsm == null)
            {
                return null;
            }
            return fsm;
        }
        private set { }
    }

    public Vector3 originScale; //원본 스케일
    public Vector3 flipScale; //플립 스케일 

    public Entity OwnerEntity = null;//이 컨트롤을 소유한 엔티티 

    public bool bIsRepath = false; //길을 다시 찾아야하는지 체크변수

    /// <summary>
    /// 초기화 
    /// </summary>
    public virtual void FSM_SetUp()
    {

    }

    /// <summary>
    /// 트랜지션 변경
    /// </summary>
    /// <param name="t"></param>
    public virtual void SetTransition(Transition t)
    {

    }

    public virtual bool CheckStateID(StateID id)
    {
        return CurrStateID == id;
    }
    

    /// <summary>
    /// fsm셋업
    /// </summary>
    protected virtual void OnFsmSetup()
    {

    }

    /// <summary>
    /// 컨트롤 업데이트
    /// </summary>
    public virtual void UpdateController()
    {

    }

    /// <summary>
    /// 락온중이던 타겟이 파괴됬을때 트랜지션 정의
    /// </summary>
    public virtual void OnTargetDestoryed()
    {

    }


    /// <summary>
    /// 엔티티소멸시의 동작들 정의
    /// </summary>
    public virtual void OnDestroyEntity()
    {

    }

    /// <summary>
    /// 타겟찾음.
    /// </summary>
    public virtual void OnTargetFind()
    {

    }

    /// <summary>
    /// 특정 프레임에 애니메이션 이벤트가 필요한경우에 실행
    /// </summary>
    public virtual void OnTk2dAnimationEvent(tk2dSpriteAnimationFrame frame)
    {

    }

    /// <summary>
    /// 스파인 애니메이션 이벤트
    /// </summary>
    public virtual void OnSpineAnimationEvent()
    {

    }

}

