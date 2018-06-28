using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainControl : MonoBehaviour {

    static public int Score = 0;
    static public int Life = 3;
    public GUISkin mySkin = null;


    void Update () {
        if(MainControl.Score > 1000)
        {
            MainControl.Score = 0;
            SceneManager.LoadScene("Victory");
        }

        if (MainControl.Life <= 0) 
        {
            SceneManager.LoadScene("Lose");
        }

        if(Score <= 0)
        {
            Score = 0; 
        }
	}

    private void OnGUI()
    {
        GUI.skin = mySkin;
        Rect labelRect1 = new Rect(10.0f, 10.0f, 400.0f, 100.0f);
        GUI.Label(labelRect1, "Score = " + MainControl.Score);
        Rect labelRect2 = new Rect(10.0f, 110.0f, 400.0f, 100.0f);
        GUI.Label(labelRect2, "Life = " + MainControl.Life);
    }
}
