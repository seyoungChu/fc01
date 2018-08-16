using UnityEngine;
using System.Collections;

public class Camera_Control : MonoBehaviour {
	
	//third view point camera
	//==========================
	public float distance = 10.0f;
	public float height = 5.0f;
	
	public float heightDamping = 2.0f;
	public float rotationDamping = 3.0f;
	// maybe player....
	public GameObject target;
	
	//first view point camera
	//============================
	public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY=2 }
	public RotationAxes axes = RotationAxes.MouseXAndY;
	
	public float sensitivityX = 15.0f;
	public float sensitivityY = 15.0f;
	
	public float minimumX = -360.0f;
	public float maximumX = 360.0f;
	
	public float minimumY = -60.0f;
	public float maximumY = 60.0f;
	
	private float rotationX = 0.0f;
	private float rotationY = 0.0f;
	
	public Transform cameraPositionObject;
	
	
	//second view point camera
	//===============================
	public float RotateSpeed = 10.0f;
	
	public enum CameraViewPointState { FIRST = 0, SECOND = 1, THIRD = 2}
	public CameraViewPointState currentState = CameraViewPointState.THIRD;
	
	
	// Use this for initialization
	void Start () {
		//localRotation = transform.localRotation;
	}
	
	void LateUpdate() 
	{
		switch(currentState)
		{
		case CameraViewPointState.FIRST:
			FirstView();
			break;
		case CameraViewPointState.SECOND:
			SecondView();
			break;
		case CameraViewPointState.THIRD:
			ThirdView();
			break;
		}
	}
	
	void ThirdView()
	{
		if(target == null)
		{
			//0924
			//Debug.LogError("The Target is Missing~~!!!!!!!!!");
			//0925
			bool isOn = SearchTarget();
			if(isOn == false)
			{
				return;
			}
			
		}
		
		float wantedRotationAngle = target.transform.eulerAngles.y;
		float wantedHeight = target.transform.position.y + height;
		
		float currentRotationAngle = transform.eulerAngles.y;
		float currentHeight = transform.position.y;
		
		currentRotationAngle = Mathf.LerpAngle( currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		
		currentHeight = Mathf.Lerp(currentHeight , wantedHeight, heightDamping * Time.deltaTime);
		
		
		Quaternion currentRotation = Quaternion.Euler(0 , currentRotationAngle, 0);
		
		transform.position = target.transform.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		
		transform.position = new Vector3(transform.position.x , currentHeight, transform.position.z);
		
		transform.LookAt(target.transform);
		
	}
	//0925
	bool SearchTarget()
	{
		bool isFind = false;
		 GameObject[] players  = GameObject.FindGameObjectsWithTag("Player");
		foreach(GameObject player in players)
		{
			Character_Control control = player.GetComponent<Character_Control>();
			if(control.owner == Network.player)
			{
				target = player;
				isFind = true;
				
				//
				if(cameraPositionObject == null)
				{
					cameraPositionObject = player.transform.Find("knight_head");
				}
				
				
				break;
			}
		}
		
		return isFind;
	}
	
	void FirstView()
	{
		if( axes == Camera_Control.RotationAxes.MouseXAndY )
		{
			rotationX = transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX;
			
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp(rotationY , minimumY , maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
		}else if( axes == Camera_Control.RotationAxes.MouseX )
		{
			//transform.Rotate(0, Input.GetAxis("Mouse X") * sensitivityX, 0);
			
			transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivityX, 0);
			
		}else
		{
			rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
			rotationY = Mathf.Clamp(rotationY , minimumY, maximumY);
			
			transform.localEulerAngles = new Vector3(-rotationY, transform.localEulerAngles.y , 0); 
		}
		
		transform.position = cameraPositionObject.position;
	}
	
	
	void SecondView()
	{
		transform.RotateAround(target.transform.position,Vector3.up, Time.deltaTime * RotateSpeed);
		
		transform.LookAt(target.transform);
	}
	
	public void ChangeCameraState(CameraViewPointState state)
	{
		currentState = state;
		
		if(currentState == Camera_Control.CameraViewPointState.FIRST)
		{
			if(gameObject.GetComponent<Rigidbody>())
			{
				gameObject.GetComponent<Rigidbody>().freezeRotation = true;
			}
		}else if(currentState == Camera_Control.CameraViewPointState.SECOND)
		{
			ThirdView(); // homework
			if(gameObject.GetComponent<Rigidbody>())
			{
				gameObject.GetComponent<Rigidbody>().freezeRotation = false;
			}
		}else
		{
			if(gameObject.GetComponent<Rigidbody>())
			{
				gameObject.GetComponent<Rigidbody>().freezeRotation = false;
			}
		}
		
	}
	
}







