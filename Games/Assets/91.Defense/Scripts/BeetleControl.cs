using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeetleControl : MonoBehaviour {

	public float Speed = 1.0f;
	private Transform myTransform = null;
	public Vector3[] TargetPositions = new Vector3[10];
	private int AddIndex = 0;
	private int CurrentIndex = 0;

	// Use this for initialization
	void Start () {
		myTransform = GetComponent<Transform> ();

		// animation mode
		GetComponent<Animation>().wrapMode = WrapMode.Loop;
	}
	
	// Update is called once per frame
	void Update () {
		// Get direction
		Vector3 Diff = TargetPositions [CurrentIndex] - myTransform.position;
		Vector3 Direction = Diff.normalized;

		// Move. YOU MUST USE SPACE WORLD
		myTransform.Translate(Speed * Direction * Time.deltaTime, Space.World);
		myTransform.LookAt (TargetPositions [CurrentIndex]);

		// Get Distance
		float Distance = Vector3.Distance (TargetPositions [CurrentIndex], myTransform.position);
		// If at current target position
		if (Distance < 0.1f) {
			CurrentIndex++;
			if (CurrentIndex == AddIndex) {
				Destroy (gameObject);
			}
		}
	}

	void SetTargetPosition(Vector3 targetPos)
	{
		TargetPositions [AddIndex] = targetPos;
		AddIndex++;
	}
}
