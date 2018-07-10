using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainControl : MonoBehaviour {

    static public int Score = 0;
    static public int Life = 3;
    public GUISkin mySkin = null;
    //GUISkin을 사용하기 위해 변수 지정

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if(MainControl.Score > 5000)
        {
            MainControl.Life = 3;
            MainControl.Score = 0;
            SceneManager.LoadScene("Victory");
        }
        else if(MainControl.Life < 0)
        {
            MainControl.Life = 3;
            MainControl.Score = 0;
            SceneManager.LoadScene("Lose");

        }
		
	}

    private void OnGUI()
    {
        //UI를 출력하기 위해 새로운 Rect를 생성 Rect(x, y, width, height)
        GUI.skin = mySkin;
        //GUI의 스킨에 mySkin을 적용
        Rect labelRect1 = new Rect(10.0f, 10.0f, 400.0f, 100.0f);
        GUI.Label(labelRect1, "Score : " + MainControl.Score);

        Rect labelRect2 = new Rect(10.0f, 110.0f, 400.0f, 1000.0f);
        GUI.Label(labelRect2, "Life : " + MainControl.Life);

    }

}
