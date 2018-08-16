using UnityEngine;
using System.Collections;

public class BillBoardAndUp : MonoBehaviour {
	
	private TextMesh textmesh;
	public float UpSpeed = 1.0f;
	
	
	void Awake() {
		textmesh = gameObject.GetComponent<TextMesh>();
	}
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		transform.position = new Vector3(transform.position.x,
		                                 transform.position.y + Time.deltaTime * UpSpeed,
		                                 transform.position.z);
		
		transform.LookAt(Camera.main.transform.position);
		transform.Rotate(new Vector3(0.0f,180.0f,0.0f));
		                 
		
		
	}
	
	public void SetText(string str)
	{
		if(textmesh == null)
		{
			Debug.LogError("Text mesh is null~!!!");
		}
		
		textmesh.text = str;
		textmesh.GetComponent<Renderer>().material.color = Color.red;
	}
}

		                                 
		                                 
		                                 
		                                 
		                                 
		                                 
		                                 
		                                 
		                                 