using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameoverControl : MonoBehaviour {

	private void OnGUI(){
		float CenterX = Screen.width / 2;
		float CenterY = Screen.height / 2;
		Rect ButtonRect = new Rect (CenterX - 50.0f, CenterY - 50.0f, 100.0f, 100.0f);
		if (GUI.Button (ButtonRect, "Game Over !") == true) {
			SceneManager.LoadScene ("Game");
		}
	}
}