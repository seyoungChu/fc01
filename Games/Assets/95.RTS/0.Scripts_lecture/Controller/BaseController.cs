using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    protected FSMSystem fsm;
    protected StateID LastStateID;

    public StateID CurrentStateID
    {
        get
        {
            if (fsm == null)
            {
                return StateID.NULLSTATEID;
            }
            return fsm.CurrentStateID;
        }
    }

    public Entity OwnerEntity = null;

    public Transform myTransform = null;

    public virtual void Init(Entity owner)
    {
        this.OwnerEntity = owner;
        this.myTransform = transform;
    }

    public virtual void FSM_SetUp()
    {

    }

    public virtual void SetTransition(Transition t)
    {
        fsm.PerformTransition(t);
    }

    public virtual bool CheckStateID(StateID id)
    {
        return CurrentStateID == id;
    }

    public virtual void UpdateController()
    {

    }

    public virtual void OnTargetDestroyed()
    {

    }

    public virtual void OnDestroyEntity()
    {

    }

    public virtual void OnTargetFind()
    {

    }

    public virtual Entity TargetFindandPathSearch(EntityType type)
    {
        return null;
    }
}
