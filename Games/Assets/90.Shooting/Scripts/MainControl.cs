using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainControl : MonoBehaviour {

    static public int Score = 0;
    static public int Life = 3;
    public GUISkin mySkin = null;


    // Update is called once per frame
    void Update () {
        //만약 내 점수가 500점이 넘는다면.
		if(MainControl.Score > 500)
        {
            //스코어는 초기화.
            MainControl.Score = 0;
            
            SceneManager.LoadScene("Victory");//새로운 씬을 불러온다.
        }
	}

    private void OnGUI()
    {
        GUI.skin = mySkin;
        Rect labelRect1 = new Rect(10.0f, 10.0f, 400.0f, 100.0f);
        GUI.Label(labelRect1, "Score : " + MainControl.Score);
        Rect labelRect2 = new Rect(10.0f, 110.0f, 400.0f, 100.0f);
        GUI.Label(labelRect2, "Life : " + MainControl.Life);
    }

}
