using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : SingletonMonobehaviour<CameraManager>
{
    public static tk2dCamera tk2DCamera;
	private bool isInit = false;

    public bool isEnableZoom = true;

    public bool isEnablePan = true;

    public bool isEnableTween = false;

    private int prevInputCount = 0;

    public Bounds rootForBounds = new Bounds();

    public float springDampStrengthen = 90f;

    public float springDampStrengthenMin = 2.5f;

    public float springDampStrengthenMax = 36.0f;

    public bool smoothDragStart = false;

    public float momentumAmount = 36.0f;

    public Vector2 momentumVector = Vector2.zero;

    public Vector3 dragStartPosition = Vector3.zero;

    float panThresHolds = 200.0f;

    public Vector3 lastMousePosition = Vector3.zero;

    public Vector2 panAcumVector = Vector2.zero;

    public bool isCameraPanning = false;

    SpringPosition springPosition = null;

    Bounds CalculateBounds()
    {
        GameObject boundsObject = GameObject.FindWithTag("MapBounds");

        if (boundsObject != null)
        {
            this.rootForBounds = boundsObject.GetComponent<BoxCollider>().bounds;
        }

        return this.rootForBounds;
    }

    private void InitCamera()
	{
		if(Camera.main != null && CameraManager.tk2DCamera == null)
		{
			this.isInit = true;
			this.CalculateBounds();
            CameraManager.tk2DCamera = Camera.main.GetComponent<tk2dCamera>();

            springPosition = Camera.main.GetComponent<SpringPosition>();
		}
	}

    private void Start()
    {
        
    }

    Vector3 CalculateConstrainOffset()
    {
        Vector3 bottomLeft = Vector3.zero;
        Vector3 topRight = new Vector3(Screen.width, Screen.height, 0.0f);

        bottomLeft = CameraManager.tk2DCamera.ScreenCamera.ScreenToWorldPoint(bottomLeft);
        topRight = CameraManager.tk2DCamera.ScreenCamera.ScreenToWorldPoint(topRight);

        Vector2 minRect = new Vector2(this.rootForBounds.min.x, this.rootForBounds.min.y);
        Vector2 maxRect = new Vector2(this.rootForBounds.max.x, this.rootForBounds.max.y);

        return NGUIMath.ConstrainRect(minRect, maxRect, bottomLeft, topRight);
    }

    bool ContrainBounds(bool immediate)
    {
        Vector3 offset = CalculateConstrainOffset();
        tk2dCamera tkCamera = CameraManager.tk2DCamera;
        if (offset.magnitude > 0f)
        {
            if (immediate == true)
            {
                tkCamera.transform.position -= offset;
            }
            else
            {
                SpringPosition sp = SpringPosition.Begin(tkCamera.gameObject, tkCamera.transform.position - offset, this.springDampStrengthen);
                sp.ignoreTimeScale = true;
                sp.worldSpace = true;
            }
            return true;
        }
        return false;
    }

    void Pan()
    {
        if (InputManager.fingerInputDic.Count == 1)
        {
            InputManager.FingerInput input = InputManager.Instance.GetCurrentInput();

            if (input.currentTouchPhase == TouchPhase.Began)
            {
                this.momentumVector = Vector2.zero;
                this.springDampStrengthen = this.springDampStrengthenMax;
                this.dragStartPosition = input.currentPoint;
                if (springPosition != null)
                {
                    springPosition.enabled = false;
                }
            }
            else if (input.currentTouchPhase == TouchPhase.Moved)
            {
                float x = (input.prevPoint.x - input.currentPoint.x);
                float y = (input.currentPoint.y - input.prevPoint.y);

                Vector2 deltaDrag = new Vector2(x, y);
                this.momentumVector = momentumVector + deltaDrag * ((0.02f / CameraManager.tk2DCamera.ZoomFactor) * momentumAmount);
                this.panAcumVector += this.momentumVector;
            }
            else if (input.currentTouchPhase == TouchPhase.Canceled || input.currentTouchPhase == TouchPhase.Ended)
            {
                Vector3 direction = (Vector3)input.currentPoint - this.dragStartPosition;
                Direction8Way way = Helper.GetDirectionType(direction);
                float panTH = this.panThresHolds;
                if (way == Direction8Way.n || way == Direction8Way.s)
                {
                    panTH = panTH * (Screen.height / Screen.width);
                }


                float distance = Vector3.Distance(input.currentPoint, this.dragStartPosition);

                if (distance > panTH)
                {
                    this.springDampStrengthen = this.springDampStrengthenMin;
                }
                else
                {
                    this.springDampStrengthen = this.springDampStrengthenMax;
                }

                this.ContrainBounds(false);
            }
        }
        else
        {
            this.panAcumVector = Vector2.zero;
            this.isCameraPanning = false;
        }

        float delTaTime = Time.deltaTime;

        if (momentumVector.magnitude > 0.01f)
        {
            Vector2 dampingVec = NGUIMath.SpringDampen(ref momentumVector, this.springDampStrengthen, delTaTime);
            if (float.IsNaN(dampingVec.x) == false && float.IsNaN(dampingVec.y) == false)
            {
                this.isCameraPanning = true;
                CameraManager.tk2DCamera.transform.Translate(dampingVec, Space.Self);
            }
            if (!ContrainBounds(false))
            {
                springPosition.enabled = false;
            }
        }
    }

    public void CameraUpdate()
    {

		if(this.isInit == false)
		{
			this.InitCamera();
		}

        InputManager.FingerInput currentInput = InputManager.Instance.GetCurrentInput();

        if (this.isEnableTween == true)
        {

        }
        else
        {
            if (this.isEnablePan == true)
            {
                Pan();
            }
            if (this.isEnableZoom == true)
            {

            }
        }
    }




}
