using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : BaseController
{

    public Transform myTarget = null;

    public AILerp aILerp = null;

    public AniControllerNormalUnit aniController = null;

    public Direction8Way myDirection = Direction8Way.e;

    public override void Init(Entity owner)
    {
        base.Init(owner);

        if (this.aniController == null)
        {
            if (gameObject.GetComponent<AniControllerNormalUnit>() == null)
            {
                this.aniController = gameObject.AddComponent<AniControllerNormalUnit>();

            }
            else
            {
                this.aniController = gameObject.AddComponent<AniControllerNormalUnit>();
            }
            this.aniController.Init(this.OwnerEntity.category);
        }
        this.aILerp = GetComponent<AILerp>();
        this.aILerp.unitcontroll = this;

        FSM_SetUp();
    }

    public override void FSM_SetUp()
    {
        this.myTarget = null;

        UnitState_IDLE idleState = new UnitState_IDLE(this);
        idleState.AddTransition(Transition.IdleToSearch, StateID.SEARCH);

        UnitState_TargetFind targetFindState = new UnitState_TargetFind(this);
        targetFindState.AddTransition(Transition.SearchToWalk, StateID.WALK);
        targetFindState.AddTransition(Transition.SearchToIdle, StateID.IDLE);

        UnitState_Walk walkState = new UnitState_Walk(this);
        walkState.AddTransition(Transition.WalkToAttack, StateID.ATTACK);
        walkState.AddTransition(Transition.WalkToSearch, StateID.SEARCH);
        walkState.AddTransition(Transition.WalkToIdle, StateID.IDLE);

        UnitState_Attack attackState = new UnitState_Attack(this);
        attackState.AddTransition(Transition.AttackToSearch, StateID.SEARCH);
        attackState.AddTransition(Transition.AttackToIdle, StateID.IDLE);

        this.fsm = new FSMSystem();
        this.fsm.CreateStates();
        this.fsm.AddState(idleState);
        this.fsm.AddState(targetFindState);
        this.fsm.AddState(walkState);
        this.fsm.AddState(attackState);

    }
    private StateID LastStateID = StateID._MAX;
    public override void UpdateController()
    {
        if (this.fsm != null && this.fsm.CurrentState != null)
        {
            if (LastStateID != CurrentStateID)
            {
                Debug.LogWarning("CurrentState :" + CurrentStateID.ToString());
                LastStateID = CurrentStateID;
            }
            this.fsm.CurrentState.DoCheck();
            this.fsm.CurrentState.DoAct();
        }

        if (this.aniController != null)
        {
            this.aniController.UpdateAnimation();
        }
        this.aniController.sprite_main.SortingOrder = 10000 - (int)this.myTransform.position.y;
    }


    public override Entity TargetFindandPathSearch(EntityType type)
    {
        float searchRange = 1000000.0f;//this.OwnerEntity.searchRange * Define.GridDiagonal;
        Building targetEntity = null;
        Transform target = EntityManager.Instance.GetCloseEntity(myTransform.position, searchRange, EntityType.Defense);
        if (target != null)
        {
            Debug.LogWarning("TargetFind :" + target.gameObject.name, target);
            this.myTarget = target;
            targetEntity = this.myTarget.GetComponent<Building>();
            if (targetEntity != null)
            {
                Vector3 targetPos = targetEntity.GetTargetPos(myTarget.position, myTransform.position);
                Debug.Log("Target Pos:" + targetPos.ToString());
                if (aILerp != null)
                {
                    this.aILerp.SearchPath(targetPos);
                }
            }

        }
        return targetEntity;

    }

    public bool DirectionChange(Vector3 Dir)
    {
        Direction8Way newDirection = Direction8Way.e;
        if (Mathf.Abs(Dir.x) > 0.2f)
        {
            if (Mathf.Abs(Dir.y) > 0.2f)
            {
                if (Dir.x > 0f)
                {
                    if (Dir.y > 0f)
                    {
                        newDirection = Direction8Way.ne;
                    }
                    else
                    {
                        newDirection = Direction8Way.se;
                    }
                }
                else
                {
                    if (Dir.y > 0f)
                    {
                        newDirection = Direction8Way.nw;
                    }
                    else
                    {
                        newDirection = Direction8Way.sw;
                    }
                }
            }
            else
            {
                if (Dir.x > 0)
                {
                    newDirection = Direction8Way.e;
                }
                else
                {
                    newDirection = Direction8Way.w;
                }
            }
        }
        else
        {
            if (Mathf.Abs(Dir.y) > 0.2f)
            {
                if (Dir.y > 0f)
                {
                    newDirection = Direction8Way.n;
                }
                else
                {
                    newDirection = Direction8Way.s;
                }
            }
            else
            {
                if (Dir.x > 0f)
                {
                    if (Dir.y > 0f)
                    {
                        newDirection = Direction8Way.ne;

                    }
                    else
                    {
                        newDirection = Direction8Way.se;
                    }
                }
                else
                {
                    if (Dir.y > 0f)
                    {
                        newDirection = Direction8Way.nw;
                    }
                    else
                    {
                        newDirection = Direction8Way.sw;
                    }
                }
            }
        }
        bool retValue = false;

        if (this.myDirection != newDirection)
        {
            retValue = true; //changed!
        }
        this.myDirection = newDirection;

        this.aniController.ChangeDirection(this.myDirection);



        return retValue;
    }

    public override void OnTargetFind()
    {
        if (CurrentStateID != StateID.SEARCH)
        {
            return;
        }
        if (fsm.CurrentState is UnitState_TargetFind)
        {
            UnitState_TargetFind state = (UnitState_TargetFind)fsm.CurrentState;
            state.isSearchCompleted = true;
        }
        this.aILerp.canMove = true;
    }

}
