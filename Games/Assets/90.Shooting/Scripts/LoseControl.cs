using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoseControl : MonoBehaviour {


    public GUISkin mySkin = null;

    private void OnGUI()
    {
        GUI.skin = mySkin;

        float CenterX = Screen.width / 2;
        float CenterY = Screen.height / 2;

        Rect ButtonRect = new Rect(CenterX - 250.0f, CenterY - 50.0f, 500.0f, 100.0f);

        //만약 버튼이 눌렸다면
        if (GUI.Button(ButtonRect, "You Lose!!") == true)
        {
            SceneManager.LoadScene("Game");
            //Game 씬을 로드
        }
    }

}
