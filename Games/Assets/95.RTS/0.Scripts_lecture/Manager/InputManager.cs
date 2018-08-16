using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : SingletonMonobehaviour<InputManager>
{

    /// <summary> RaycastCamera에서 Raycast를 할 Layer</summary>
    public static int rayHitLayer { get; set; }
    /// <summary> 터치 관련 정보를 담아둔다. </summary>
    public static Dictionary<int, FingerInput> fingerInputDic = new Dictionary<int, FingerInput>();
    /// <summary> 시작점으로부터 다음의 길이보다 크게 드래그 되어야 실제 드래그로 인정한다. </summary>
    public float DRAG_THRESHOLD = 15.0f;

    public Building SelectBuilding = null;
    public Building DragBuilding = null;
    public enum MouseButton { Left = 0, Right, Wheel };
    public class FingerInput
    {
        public int fingerID = -1;

        public Vector2 beginPoint = Vector2.zero;
        public Vector3 beginTouchOrMousePosition = Vector3.zero;
        public Vector3 beginRayHitPosition = Vector3.zero;
        public Collider2D beginRayHitCollider = null;
        public Transform beginRayHitTransform = null;
        public int beginRayHitLayer = 0;

        public Vector2 currentPoint = Vector2.zero;
        public Vector3 currentTouchOrMousePosition = Vector3.zero;
        public Vector3 currentRayHitPosition = Vector3.zero;
        public Collider2D currentRayHitCollider = null;
        public Transform currentRayHitTransform = null;
        public int currentRayHitLayer = 0;

        public Vector2 deltaPoint = Vector2.zero;
        public Vector3 deltaRayHitPosition = Vector3.zero;
        public Vector3 deltaTouchOrMousePosition = Vector3.zero;


        public Vector2 prevPoint = Vector2.zero;
        public Vector3 prevTouchOrMousePosition = Vector3.zero;
        public Vector3 prevRayHitPosition = Vector3.zero;
        public Collider2D prevRayHitCollider = null;
        public Transform prevRayHitTransform = null;
        public int prevRayHitLayer = 0;

        public TouchPhase currentTouchPhase;
        public TouchPhase prevTouchPhase;


        public Vector3 currentScreenPoint = Vector3.zero;

        public tk2dCamera MainCamera;

        public float pressingAcumTime = 0.0f;
        public bool isDragging = false;

        public bool isPressed = false;


        public bool IsHoveringUI
        {
            get
            {
                return currentRayHitLayer == Define.LAYERMASK_UI;
            }
        }

        public FingerInput(int fID)
        {
            this.fingerID = fID;
            this.MainCamera = Camera.main.GetComponent<tk2dCamera>();
        }

        public void SetCurrentPoint(TouchPhase touchphase, Vector2 currentpos)
        {
            Vector2 newPoint = Vector2.zero;
            Vector3 newTouchOrMousePosition = Vector3.zero;
            Vector3 newRayHitPosition = Vector3.zero;
            Collider2D newRayHitCollider = null;
            Transform newRayHitTransform = null;
            int newRayHitLayer = -1;

            currentScreenPoint = currentpos;

            newTouchOrMousePosition = this.MainCamera.ScreenCamera.ScreenToWorldPoint(currentpos);

            newPoint = new Vector2(currentpos.x, Screen.height - currentpos.y);
            RaycastHit2D raycasthitInfo = Physics2D.Raycast(newTouchOrMousePosition, Vector2.zero);

            if (raycasthitInfo.collider != null)
            {
                //Debug.LogWarning("RayCastHit On :" + raycasthitInfo.collider.transform.name);
                newRayHitPosition = raycasthitInfo.point;
                newRayHitCollider = raycasthitInfo.collider;
                newRayHitTransform = raycasthitInfo.transform;
                newRayHitLayer = 1 << raycasthitInfo.transform.gameObject.layer;

            }
            else
            {
                //Debug.Log("RayCast Miss");
                newRayHitCollider = null;
                newRayHitLayer = -1;
                newRayHitTransform = null;
            }
            if (touchphase == TouchPhase.Began)
            {
                this.beginPoint = this.currentPoint = newPoint;
                this.beginTouchOrMousePosition = this.currentTouchOrMousePosition = newTouchOrMousePosition;

                this.beginRayHitPosition = this.currentRayHitPosition = newRayHitPosition;
                this.beginRayHitCollider = this.currentRayHitCollider = newRayHitCollider;
                this.beginRayHitLayer = this.currentRayHitLayer = newRayHitLayer;
                this.beginRayHitTransform = this.currentRayHitTransform = newRayHitTransform;

            }
            else
            {
                deltaPoint = newPoint - currentPoint;
                deltaTouchOrMousePosition = newTouchOrMousePosition - currentTouchOrMousePosition;

                deltaRayHitPosition = newRayHitPosition - currentRayHitPosition;

                prevPoint = currentPoint;
                prevTouchOrMousePosition = currentTouchOrMousePosition;
                prevRayHitPosition = currentRayHitPosition;
                prevRayHitCollider = currentRayHitCollider;
                prevRayHitLayer = currentRayHitLayer;
                prevRayHitTransform = currentRayHitTransform;

                currentPoint = newPoint;
                currentTouchOrMousePosition = newTouchOrMousePosition;
                currentRayHitPosition = newRayHitPosition;
                currentRayHitCollider = newRayHitCollider;
                currentRayHitLayer = newRayHitLayer;
                currentRayHitTransform = newRayHitTransform;


            }
            this.currentTouchPhase = touchphase;
            //Debug.Log("Touch :" + this.currentTouchPhase.ToString());
        }

    }

    public void TouchUpdate()
    {

        FingerInput fingerinput;


        if (Input.touchCount > 0)
        {

            for (int i = 0; i < Input.touchCount; i++)
            {
                if (fingerInputDic.TryGetValue(Input.touches[i].fingerId, out fingerinput) == false)
                {
                    if (Input.touches[i].phase == TouchPhase.Began)
                    {
                        fingerinput = new FingerInput(Input.touches[i].fingerId);
                        fingerInputDic.Add(Input.touches[i].fingerId, fingerinput);
                    }
                }

                rayHitLayer = Define.LAYERMASK_ALL_PICKLAYER;

                fingerinput.SetCurrentPoint(Input.touches[i].phase, Input.touches[i].position);

                switch (Input.touches[i].phase)
                {
                    case TouchPhase.Moved:
                        {
                            fingerinput.pressingAcumTime += Time.deltaTime;
                            float dragDelta = Vector2.Distance(fingerinput.beginPoint, fingerinput.currentPoint);
                            fingerinput.isDragging = (dragDelta > DRAG_THRESHOLD) ? true : false;
                        }
                        break;
                    case TouchPhase.Stationary:
                        {
                            fingerinput.pressingAcumTime += Time.deltaTime;
                            fingerinput.isPressed = true;
                        }
                        break;
                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        {
                            fingerinput.isPressed = false;
                            fingerinput.pressingAcumTime = 0.0f;
                            fingerinput.isDragging = false;
                        }
                        break;

                }
                fingerinput.prevTouchPhase = fingerinput.currentTouchPhase;

            }

        }
        else
        {
            if (Input.GetMouseButtonDown((int)MouseButton.Left))
            {
                rayHitLayer = Define.LAYERMASK_ALL_PICKLAYER;
                if (fingerInputDic.TryGetValue((int)MouseButton.Left, out fingerinput) == false)
                {
                    fingerinput = new FingerInput((int)MouseButton.Left);
                    fingerinput.SetCurrentPoint(TouchPhase.Began, Input.mousePosition);
                    fingerInputDic.Add((int)MouseButton.Left, fingerinput);
                }

            }
            else if (Input.GetMouseButtonUp((int)MouseButton.Left))
            {
                if (fingerInputDic.TryGetValue((int)MouseButton.Left, out fingerinput) == true)
                {
                    fingerinput.SetCurrentPoint(TouchPhase.Ended, Input.mousePosition);

                    fingerinput.isDragging = false;
                    fingerinput.isPressed = false;
                    fingerinput.pressingAcumTime = 0.0f;
                }

            }
            else if (Input.GetMouseButton((int)MouseButton.Left))
            {
                if (fingerInputDic.TryGetValue((int)MouseButton.Left, out fingerinput) == true)
                {
                    fingerinput.SetCurrentPoint(TouchPhase.Moved, Input.mousePosition);
                    float dragDelta = Vector2.Distance(fingerinput.prevPoint, fingerinput.currentPoint);
                    fingerinput.isDragging = (dragDelta > DRAG_THRESHOLD) ? true : false;
                    fingerinput.isPressed = true;
                    fingerinput.pressingAcumTime += Time.deltaTime;
                }
            }
            else
            {
                if (fingerInputDic.Count > 0)
                {
                    fingerInputDic.Clear();
                }
            }
        }


    }

    public void UpdateGameInput()
    {
        //phase 2 shortcut Key
        if (Input.GetKeyDown(KeyCode.F1))
        {
            EntityManager.Instance.SpawnEntity((int)EntityList.WatchTower);
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            EntityManager.Instance.SpawnEntity((int)EntityList.Creed);
        }



        //phase 1 touch control
        if (fingerInputDic.Count == 0)
        {
            return;
        }
        foreach (FingerInput input in fingerInputDic.Values)
        {

            if (input.currentTouchPhase == TouchPhase.Began)
            {
                TouchBeginState(input);
            }
            else if (input.currentTouchPhase == TouchPhase.Moved)
            {
                TouchPressingState(input);
            }
            else if (input.currentTouchPhase == TouchPhase.Ended)
            {
                TouchEndState(input);
            }
        }

    }

    void TouchBeginState(FingerInput input)
    {
        if (input.currentRayHitLayer == Define.LAYERMASK_BUILDING)
        {

            CameraManager.Instance.isEnablePan = false;

            this.SelectBuilding = input.currentRayHitTransform.GetComponent<Building>();

            if (this.DragBuilding == null)
            {
                this.DragBuilding = this.SelectBuilding;
                this.DragBuilding.isSelect = true;
            }
            else
            {
                this.DragBuilding.isSelect = false;
                this.DragBuilding = this.SelectBuilding;
                this.DragBuilding.isSelect = true;
            }
        }
    }

    void TouchPressingState(FingerInput input)
    {

    }

    void TouchEndState(FingerInput input)
    {
        if (this.DragBuilding != null)
        {
            this.DragBuilding.isSelect = false;
        }
        if (this.SelectBuilding != null)
        {
            this.SelectBuilding = null;
        }

        if (CameraManager.Instance.isEnablePan == false)
        {
            CameraManager.Instance.isEnablePan = true;
        }
    }


    public Vector3 GetDragPoint()
    {
        if (fingerInputDic.Count <= 0)
        {
            return Define.EXCEPT_POSITON;
        }

        FingerInput current_input = GetCurrentInput();

        return current_input.currentTouchOrMousePosition;
    }

    public FingerInput GetCurrentInput()
    {
        if (fingerInputDic.Count <= 0)
        {
            return null;
        }

        var enumerator = fingerInputDic.GetEnumerator();
        enumerator.MoveNext();
        FingerInput current_input = enumerator.Current.Value;
        return current_input;
    }

}
