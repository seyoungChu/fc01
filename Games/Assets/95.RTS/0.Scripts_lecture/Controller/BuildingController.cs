using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingController : BaseController
{
    public Transform myTarget = null;

    public AniControllerDefenseBuilding aniController;

    public override void FSM_SetUp()
    {
        this.myTarget = null;

        BuildingState_IDLE buildingIDLE = new BuildingState_IDLE(this);
        buildingIDLE.AddTransition(Transition.IdleToSearch, StateID.SEARCH);

        BuildingState_TargetFind buildingSEARCH = new BuildingState_TargetFind(this);
        buildingSEARCH.AddTransition(Transition.SearchToAttack, StateID.ATTACK);
        buildingSEARCH.AddTransition(Transition.SearchToIdle, StateID.IDLE); //if die.

        BuildingState_Attack buildingATTACK = new BuildingState_Attack(this);
        buildingATTACK.AddTransition(Transition.AttackToSearch, StateID.SEARCH);
        buildingATTACK.AddTransition(Transition.AttackToIdle, StateID.IDLE); //if die.

        fsm = new FSMSystem();
        fsm.CreateStates();
        fsm.AddState(buildingIDLE);
        fsm.AddState(buildingSEARCH);
        fsm.AddState(buildingATTACK);

    }

    public override void UpdateController()
    {
        if (fsm != null && fsm.CurrentState != null)
        {
            fsm.CurrentState.DoCheck();
            fsm.CurrentState.DoAct();
        }

        if (aniController != null)
        {
            aniController.UpdateAnimation();
        }
    }

    public override void Init(Entity owner)
    {
        base.Init(owner);
        if (this.aniController == null)
        {
            if (gameObject.GetComponent<AniControllerDefenseBuilding>() == null)
            {
                this.aniController = gameObject.AddComponent<AniControllerDefenseBuilding>();
            }
            else
            {
                this.aniController = gameObject.GetComponent<AniControllerDefenseBuilding>();
            }
        }
        this.aniController.Init(this.OwnerEntity.category);
    }

    public override void OnDestroyEntity()
    {
        this.myTarget = null;

        if (CurrentStateID == StateID.SEARCH)
        {
            SetTransition(Transition.SearchToIdle);
        }
        else if (CurrentStateID == StateID.ATTACK)
        {
            SetTransition(Transition.AttackToIdle);
        }

    }

    public override void OnTargetDestroyed()
    {
        this.myTarget = null;
        if (CurrentStateID == StateID.ATTACK)
        {
            SetTransition(Transition.AttackToSearch);
        }
    }


}
