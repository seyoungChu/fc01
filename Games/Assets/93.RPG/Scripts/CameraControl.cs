using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour {
    //1인칭.2인칭.3인칭 모드
    public enum CameraViewpointState { FIRST, SECOND, THIRD }
    //기본적으로 3인칭카메라.
    public CameraViewpointState cameraState = CameraViewpointState.THIRD;

    private Transform myTransform = null;
    //3인칭 카메라 관련 멤버 변수.
    //카메라와 캐릭터간의 거리.
    public float Distance = 5.0f;
    //카메라와 캐릭터로부터의 높이.
    public float Height = 1.5f;
    //댐핑용 변수들.
    public float HeightDamping = 2.0f;
    public float RotationDamping = 3.0f;
    //카메라가 바라보게될 타겟 게임오브젝트.
    public GameObject Target = null;

    //2인칭 관련.(모델 회전뷰)
    public float RotateSpeed = 10.0f;

    //1인칭 관련
    public enum RotationAxes { MouseXandY , MouseX, MouseY }
    public RotationAxes axes = RotationAxes.MouseXandY;
    public float sensitivityX = 5.0f;
    public float sensitivityY = 5.0f;
    public float minimumX = 0.0f;
    public float maximumX = 90.0f;
    public float minimumY = -30.0f;
    public float maximumY = 30.0f;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;
    public Transform ViewPoint = null;


	// Use this for initialization
	void Start () {
        myTransform = GetComponent<Transform>();
	}
    //3인칭 뷰.
    void ThirdView()
    {
        if(Target != null)
        {   //타겟 트랜스폼.
            Transform targetTrans = Target.transform;
            //목표로하는 y축 회전값과 높이값.
            float wantedRotationAngle = targetTrans.eulerAngles.y;
            float wantedHeight = targetTrans.position.y + Height;
            //현재 나의 y축 회전값과 높이값.
            float currentRotationAngle = myTransform.eulerAngles.y;
            float currentHeight = myTransform.position.y;
            //현재 회전값을 보간해서 목표로하는 회전값에 가깝도록.
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle,
                wantedRotationAngle,
                RotationDamping * Time.deltaTime);
            //현재 높이값을 보간해서 목표로 하는 높이값에 가깝도록.
            currentHeight = Mathf.Lerp(currentHeight,
                wantedHeight,
                HeightDamping * Time.deltaTime);
            //회전 벡터를 만들기위해 사원수를 하나 만듭니다.
            //ii+ji+ki+a = 0 -> quaternion 3차원 공간의 위치와 각도를 표시하는 수 체계
            Quaternion currentRotation = Quaternion.Euler(0.0f, currentRotationAngle,
                0.0f);

            //카메라의 3인칭 이동.
            //카메라를 먼저 타겟의 위치로 이동
            myTransform.position = targetTrans.position;
            //카메라가 바라보아야할 방향으로 회전후 원하는 거리만큼 빼줍니다.
            myTransform.position -= currentRotation * Vector3.forward * Distance;
            //카메라의 높이를 지정합니다.
            myTransform.position = new Vector3(myTransform.position.x,
                                                currentHeight,
                                                myTransform.position.z);
            //카메라가 타겟을 바라보게 됩니다.
            myTransform.LookAt(targetTrans);
        }
    }
    //2인칭 뷰
    void SecondView()
    {   //카메라를 타겟위치를 중심으로 y축방향으로 RotateSpeed만큼 회전합니다.
        myTransform.RotateAround(Target.transform.position, Vector3.up,
            Time.deltaTime * RotateSpeed);
        //회전하면서 타겟을 바라보게 됩니다.
        myTransform.LookAt(Target.transform);
    }
    //1인칭 뷰
    void FirstView()
    {
        //마우스의 좌우이동값을 기준으로 y축 회전 값을 구하고.
        rotationX = myTransform.localEulerAngles.y +
            Input.GetAxis("Mouse X") * sensitivityX;
        //y축 회전값은 최소,최대값사이의 값으로 하고.
        rotationX = Mathf.Clamp(rotationX, minimumX, maximumX);
        //마우스의 상하이동값을 기준으로 현재 x축 값에 더해줍니다.
        rotationY = rotationY + Input.GetAxis("Mouse Y") * sensitivityY;
        //x축 회전값은 최소,최대값 사이 값으로 합니다.
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);
        //마우스 좌우상하로 시선을 조종한다면.
        if(axes == RotationAxes.MouseXandY)
        {
            myTransform.localEulerAngles = new Vector3(
                -rotationY, rotationX, 0.0f);
        }else if(axes == RotationAxes.MouseX)
        {   //마우스의 좌우로 시선을 조종한다면.
            myTransform.localEulerAngles = new Vector3(0.0f, rotationX, 0.0f);
        }else if(axes == RotationAxes.MouseY)
        {   //마우스의 상하로 시선을 조종한다면.
            myTransform.localEulerAngles = new Vector3(-rotationY,
                myTransform.localEulerAngles.y, 0.0f);
        }
        myTransform.position = ViewPoint.position;
    }
	
	// 모든 업데이트가 호출된 이후에 호출되는 업데이트.
	void LateUpdate () {
		switch(cameraState)
        {
            case CameraViewpointState.FIRST:
                FirstView();
                break;
            case CameraViewpointState.SECOND:
                SecondView();
                break;
            case CameraViewpointState.THIRD:
                ThirdView();
                break;
        }
	}
}
