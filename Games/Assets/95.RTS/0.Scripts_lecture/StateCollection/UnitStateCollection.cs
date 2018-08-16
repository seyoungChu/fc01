using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState_IDLE : FSMState
{
    UnitController Owner;

    public UnitState_IDLE(UnitController owner)
    {
        base.controller = owner;
        this.Owner = owner;
        stateID = StateID.IDLE;
    }

    public override void DoBeforeEntering()
    {
        this.Owner.aILerp.canMove = false;
    }

    public override void DoCheck()
    {
        if (this.changeState == true)
        {
            this.Owner.SetTransition(Transition.IdleToSearch);
        }
    }

    public override void DoAct()
    {
        if (this.changeState == false)
        {
            this.changeState = true;
        }
    }

    public override void DoBeforeLeaving()
    {
        this.changeState = false;
    }


}

public class UnitState_TargetFind : FSMState
{
    UnitController OwnerController;

    bool finishSearchPath = false;

    public UnitState_TargetFind(UnitController owner)
    {
        base.controller = owner;
        this.OwnerController = owner;
        this.stateID = StateID.SEARCH;
    }


    public override void DoBeforeEntering()
    {
        this.OwnerController.aniController.PlayAnimation(AnimationType.Walk);

        this.isSearchCompleted = false;
        this.finishSearchPath = false;
        if (this.OwnerController.aILerp != null)
        {
            this.OwnerController.aILerp.canMove = false;
        }

        OwnerController.myTarget = null;
    }

    public override void DoCheck()
    {
        if (this.isSearchCompleted == true)
        {
            this.OwnerController.SetTransition(Transition.SearchToWalk);
        }
    }

    public override void DoAct()
    {
        if (finishSearchPath == false)
        {
            Entity target = null;
            target = OwnerController.TargetFindandPathSearch(EntityType.Defense);
            if (target != null)
            {
                this.finishSearchPath = true;
            }
        }
    }

    public override void DoBeforeLeaving()
    {
        this.isSearchCompleted = false;
        this.finishSearchPath = false;
    }


}

public class UnitState_Walk : FSMState
{
    UnitController ownerController;

    Vector3 currDirection;
    Direction8Way GoalDirection;
    float lastDistance = 0.0f;
    Entity targetEntity = null;

    public UnitState_Walk(UnitController owner)
    {
        base.controller = owner;
        this.ownerController = owner;
        stateID = StateID.WALK;
    }


    public override void DoBeforeEntering()
    {
        if (ownerController.myTarget != null)
        {
            Vector3 targetDir = (ownerController.myTarget.position - ownerController.myTransform.position).normalized;
            ownerController.DirectionChange(targetDir);

            if (ownerController.myTarget.GetComponent<Entity>() != null)
            {
                this.targetEntity = ownerController.myTarget.GetComponent<Entity>();
            }

        }
        ownerController.aniController.PlayAnimation(AnimationType.Walk);
    }

    public override void DoCheck()
    {
        if (ownerController.myTarget == null ||
            this.targetEntity != null && this.targetEntity.IsDead() == true)
        {
            ownerController.SetTransition(Transition.WalkToSearch);
            return;
        }

        float Distance = Vector3.Distance(ownerController.myTarget.position, ownerController.myTransform.position);
        if (Distance < ownerController.OwnerEntity.searchRange * Define.GridDiagonal)
        {
            ownerController.SetTransition(Transition.WalkToAttack);
            return;
        }
    }

    public override void DoAct()
    {
        if (this.currDirection != ownerController.aILerp.AIdirection)
        {
            ownerController.DirectionChange(ownerController.aILerp.AIdirection);
            ownerController.aniController.PlayAnimation(AnimationType.Walk);
            this.currDirection = ownerController.aILerp.AIdirection;
        }
    }

    public override void DoBeforeLeaving()
    {
        this.ownerController.aILerp.canMove = false;
        if (this.ownerController.myTarget != null)
        {
            Vector3 targetDir = (ownerController.myTarget.position - ownerController.myTransform.position).normalized;
            ownerController.DirectionChange(targetDir);
            ownerController.aniController.PlayAnimation(AnimationType.Walk);

        }
    }
}


public class UnitState_Attack : FSMState
{
    UnitController OwnerController;

    Timer attackCoolTime;


    Entity TargetEntity = null;

    public UnitState_Attack(UnitController owner)
    {
        this.OwnerController = owner;
        base.controller = owner;
        stateID = StateID.ATTACK;
    }

    public void AttackCallBack()
    {
        Debug.Log("Time Check :" + Time.time.ToString());
        if (OwnerController.OwnerEntity.IsDead() == false)
        {
            OwnerController.aniController.PlayAnimation(AnimationType.Attack, false);
            //effect

            //attack
            if (this.TargetEntity != null)
            {
                BattleManager.Instance.AttackEntity(OwnerController.OwnerEntity,
                                                    TargetEntity,
                                                    OwnerController.OwnerEntity.attackPower,
                                                    OwnerController.myTarget.position);
            }

        }
        else
        {
            TimeManager.Instance.RemoveTimer(attackCoolTime);
        }
    }


    public override void DoBeforeEntering()
    {
        if (this.OwnerController.myTarget != null)
        {
            this.TargetEntity = this.OwnerController.myTarget.GetComponent<Entity>();
        }
        this.OwnerController.aniController.PlayAnimation(AnimationType.Attack, false);
        this.OwnerController.aILerp.canMove = false;
        if (this.attackCoolTime == null)
        {
            attackCoolTime = new Timer();
        }
        attackCoolTime.Repeat(OwnerController.OwnerEntity.attackSpeed, AttackCallBack, OwnerController.OwnerEntity.firstAttackDelay);
        TimeManager.Instance.AddTimer(attackCoolTime);
    }

    public override void DoCheck()
    {
        if (this.OwnerController.myTarget == null || this.TargetEntity == null ||
           this.TargetEntity != null && this.TargetEntity.IsDead() == true)
        {
            OwnerController.SetTransition(Transition.AttackToSearch);
        }
    }

    public override void DoAct()
    {

    }

    public override void DoBeforeLeaving()
    {
        if (attackCoolTime != null)
        {
            TimeManager.Instance.RemoveTimer(attackCoolTime);
        }
        this.TargetEntity = null;

    }




}