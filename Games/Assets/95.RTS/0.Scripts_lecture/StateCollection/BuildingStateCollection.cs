using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingState_IDLE : FSMState
{
    BuildingController Owner;

    public BuildingState_IDLE(BuildingController _owner)
    {
        base.controller = _owner;
        this.Owner = _owner;
        stateID = StateID.IDLE;
    }

    public override void DoBeforeEntering()
    {
        base.DoBeforeEntering();
    }

    public override void DoCheck()
    {
        if (this.changeState == true)
        {
            Owner.SetTransition(Transition.IdleToSearch);
        }
    }

    public override void DoAct()
    {
        if (Owner.gameObject.activeSelf == true)
        {
            this.changeState = true;
        }
    }

    public override void DoBeforeLeaving()
    {
        this.changeState = false;
    }
}

public class BuildingState_TargetFind : FSMState
{
    BuildingController Owner;
    Transform target = null;

    public BuildingState_TargetFind(BuildingController _owner)
    {
        base.controller = _owner;
        this.Owner = _owner;
        stateID = StateID.SEARCH;

    }

    public override void DoBeforeEntering()
    {
        target = null;
    }

    public override void DoCheck()
    {
        if (this.changeState == true)
        {
            Owner.SetTransition(Transition.SearchToAttack);
        }
    }

    public override void DoAct()
    {
        float searchRange = Owner.OwnerEntity.searchRange * Define.GridDiagonal;
        target = EntityManager.Instance.GetCloseEntity(Owner.transform.position, searchRange, EntityType.Unit);
        if (target != null)
        {
            this.changeState = true;
            Owner.myTarget = target;
        }
    }

    public override void DoBeforeLeaving()
    {
        this.changeState = false;
    }
}

public class BuildingState_Attack : FSMState
{
    BuildingController Owner;
    Entity target;
    float attackRange;
    Timer attackCoolTime = null;

    public BuildingState_Attack(BuildingController Owner)
    {
        base.controller = Owner;
        this.Owner = Owner;

        stateID = StateID.ATTACK;
    }

    public override void DoBeforeEntering()
    {
        this.attackRange = this.Owner.OwnerEntity.searchRange * Define.GridDiagonal;
        if (Owner.myTarget != null)
        {
            this.target = Owner.myTarget.GetComponent<Entity>();

            if (attackCoolTime == null)
            {
                attackCoolTime = new Timer();
            }

            attackCoolTime.Repeat(Owner.OwnerEntity.attackSpeed, AttackCallback, 0.1f);
            TimeManager.Instance.AddTimer(attackCoolTime);
        }
        else
        {
            this.changeState = true;
        }
    }

    public override void DoCheck()
    {
        if (this.changeState == true)
        {
            Owner.SetTransition(Transition.AttackToSearch);
        }
    }

    public override void DoAct()
    {
        if (this.target != null && this.target.IsDead() == true)
        {
            this.changeState = true;
        }
        float distance = Vector3.Distance(Owner.myTransform.position, this.target.myTransform.position);
        if (distance > this.attackRange)
        {
            this.changeState = true;
        }

    }

    public override void DoBeforeLeaving()
    {
        if (this.attackCoolTime != null)
        {
            TimeManager.Instance.RemoveTimer(this.attackCoolTime);
        }
        this.changeState = false;
        this.target = null;
    }

    public void AttackCallback()
    {
        if (Owner.gameObject.activeSelf == true && this.target != null && this.target.IsDead() == false)
        {
            Owner.aniController.PlayAnimation(AnimationType.Attack, false);

            BattleManager.Instance.AttackEntity(Owner.OwnerEntity, this.target, this.Owner.OwnerEntity.attackPower, this.target.myTransform.position);
        }
    }
}


