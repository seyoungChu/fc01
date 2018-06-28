using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainControl : MonoBehaviour {
    static public int Score = 0;
    static public int Life = 3;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnGUI()
	{
        Rect labelRect1 = new Rect(10.0f, 10.0f, 100.0f, 30.0f);
        GUI.Label(labelRect1, "Score :" + MainControl.Score);
        Rect labelRect2 = new Rect(10.0f, 40.0f, 100.0f, 30.0f);
        GUI.Label(labelRect2, "Life: " + MainControl.Life);
	}
}
