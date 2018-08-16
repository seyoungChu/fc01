using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : Entity
{


    public bool isSelect
    {
        set
        {
            if (_isSelect == false && value == true)
            {
                if (myCollider != null)
                {
                    myCollider.enabled = false;
                }
            }
            if (_isSelect == true && value == false)
            {

                if (myCollider != null)
                {
                    myCollider.enabled = true;

                    AstarPath.active.Scan();
                }
            }
            _isSelect = value;
        }
        get
        {
            return _isSelect;
        }
    }
    private bool _isSelect = false;
    public Vector3 originPos = Vector3.zero;
    public Vector2 lastPos = Vector2.zero;

    private Vector3 dragStartPos = Vector3.zero;
    private Vector3 dragEndPos = Vector3.zero;
    private List<Vector2> movePosList = new List<Vector2>();

    private int myGridIndex = 0;

    private tk2dSprite mySprite;
    private BoxCollider2D myCollider;



    public List<Grid> obstacle = new List<Grid>();


    private BuildingController buildingController = null;

    public override void CashingObject()
    {
        myTransform = transform;
        mySprite = transform.Find("sprite").GetComponent<tk2dSprite>();
        myCollider = gameObject.GetComponent<BoxCollider2D>();
        if (myCollider == null)
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

        this.CreateBuildingControl();
    }

    public override void UpdateEntity()
    {
        if (isSelect == true)
        {
            DragUpdate();
        }
        if (this.buildingController != null)
        {
            this.buildingController.UpdateController();
        }
    }


    public void CreateBuildingControl()
    {
        if (this.buildingController == null)
        {
            if (gameObject.GetComponent<BuildingController>() == null)
            {
                this.buildingController = gameObject.AddComponent<BuildingController>();
            }
            else
            {
                this.buildingController = gameObject.GetComponent<BuildingController>();
            }

            this.buildingController.Init(this);
        }
    }

    void DragUpdate()
    {
        //BuildingManager에서 가상으로 그려둔 타일맵기반으로 움직인다. 
        lastPos = myTransform.localPosition;

        InputManager.FingerInput current_input = InputManager.Instance.GetCurrentInput();
        if (current_input.currentRayHitTransform != null)
        {
            if (current_input.currentRayHitTransform.gameObject.layer == LayerMask.NameToLayer("Grid"))
            {
                Vector2 currentHitTransformPosition = current_input.currentRayHitTransform.position;
                if (lastPos != currentHitTransformPosition)
                {
                    if (movePosList.Contains(currentHitTransformPosition) == false)
                    {
                        movePosList.Add(currentHitTransformPosition);
                    }
                }
            }
            else
            {
                //Debug.LogWarning(LayerMask.LayerToName(current_input.currentRayHitTransform.gameObject.layer));
            }
        }

        if (movePosList.Count > 0)
        {
            myTransform.localPosition = new Vector3(movePosList[0].x, movePosList[0].y, -1.0f);
            movePosList.RemoveAt(0);
        }

    }

    public Vector3 GetTargetPos(Vector3 targetPos, Vector3 origin)
    {
        Vector3 retPosition = myTransform.position;

        Vector3[] aroundPositions = new Vector3[8];

        aroundPositions[0] = targetPos + new Vector3(-Define.GridWidth, 0.0f, 0.0f);
        aroundPositions[1] = targetPos + new Vector3(-Define.GridWidthHalf, Define.GridHeightHalf, 0.0f);
        aroundPositions[2] = targetPos + new Vector3(0.0f, Define.GridHeight, 0.0f);
        aroundPositions[3] = targetPos + new Vector3(Define.GridWidthHalf, Define.GridHeightHalf, 0.0f);
        aroundPositions[4] = targetPos + new Vector3(Define.GridWidth, 0.0f, 0.0f);
        aroundPositions[5] = targetPos + new Vector3(Define.GridWidthHalf, -Define.GridHeightHalf, 0.0f);
        aroundPositions[6] = targetPos + new Vector3(0.0f, -Define.GridHeight, 0.0f);
        aroundPositions[7] = targetPos + new Vector3(-Define.GridWidthHalf, -Define.GridHeightHalf, 0.0f);


        float minDistance = 10000000f;
        for (int i = 0; i < aroundPositions.Length; ++i)
        {
            float Distance = Vector3.Distance(aroundPositions[i], origin);
            if (Distance < minDistance)
            {
                retPosition = aroundPositions[i];
            }
        }

        return retPosition;
    }

    public override void OnDamaged(int damage)
    {
        SoundManager.Instance.PlayOneShot(SoundList.BeShot.ToString());
    }

    public override void DestoryEntity()
    {
        SoundManager.Instance.PlayOneShot(SoundList.Explosion.ToString());

    }

}
