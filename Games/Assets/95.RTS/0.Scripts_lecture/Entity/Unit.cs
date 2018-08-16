using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : Entity
{
    private tk2dSprite mySprite;
    private BoxCollider2D myCollider;
    private UnitController unitController = null;

	public override void CashingObject()
	{
        myTransform = transform;

        mySprite = transform.Find("ani_sprite").GetComponent<tk2dSprite>();

        myCollider = gameObject.GetComponent<BoxCollider2D>();
        if(myCollider == null)
        {
            myCollider = gameObject.AddComponent<BoxCollider2D>();
        }
	}

	public override void InitEntity(EntityData data)
	{
        this.category = data.entCategory;
        this.entitytype = data.entType;
        this.level = data.Level;
        this.hp = data.HP;
        this.searchRange = data.SearchRange;
        this.attackPower = data.AttackPower;
        this.attackSpeed = data.AttackSpeed;
        CreateUnitControl();
	}

    public void CreateUnitControl()
    {
        if(this.unitController == null)
        {
            if(gameObject.GetComponent<UnitController>() == null)
            {
                this.unitController = gameObject.AddComponent<UnitController>();
            }else
            {
                this.unitController = gameObject.GetComponent<UnitController>();
            }
            this.unitController.Init(this);
        }
    }

	public override void UpdateEntity()
	{
        if(this.unitController != null)
        {
            this.unitController.UpdateController();
        }
	}

}
