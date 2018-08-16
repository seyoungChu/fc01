using UnityEngine;
using System.Collections;

public class Log : MonoBehaviour {

	public GUISkin skin;
	public int ScrollSize;
	private int index = 0;
	private Vector2 scrollPosition = Vector2.zero;
	
	private string[] logtext;
	
	public static Log logInstance;
	
	public bool ison = false;
	
	void Awake() {
		
			logInstance = this;
			DontDestroyOnLoad(transform.gameObject);
				
	}
	
	void Start () {
		if(ScrollSize == 0)
		{
			ScrollSize = 1000;
		}
		logtext = new string[ScrollSize];
		for(int i = 0 ; i < ScrollSize ; i++)
		{
			logtext[i] = " ";
		}        
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 48, 16), "X"))
        {
            index = 0;
            for (int i = 0; i < ScrollSize; i++)
            {
                logtext[i] = " ";
            }

            scrollPosition = Vector2.zero;
        }
        

		ison = GUI.Toggle(new Rect(100, 0, 100, 24), ison, "On/Off");
		if(ison.Equals(true))
		{
			GUI.skin = skin;            
		
			scrollPosition = GUI.BeginScrollView(new Rect(10, 50, Screen.width-10, Screen.height-100), scrollPosition, new Rect(0, 0, 1000, ScrollSize * 24));

        
        	GUI.color = Color.red;
        	for(int i = 0; i < ScrollSize ; i++ )
			{
				GUI.Label(new Rect(0, i * 28, 700, 28), logtext[i]);
			}
			GUI.EndScrollView();

      	
		}
	}
	
	[RPC]
	public void ScreenLog(string text)
	{
		if(logtext == null)
			return;
		
		logtext[index] = text;
		index++;
		if(index > ScrollSize - 1)
		{
			for(int i = 0 ; i < ScrollSize - 1 ; i++)
			{
				logtext[i] = logtext[i+1];
			}
			index = ScrollSize - 1;
		}
	}
	
	public void SetOnOff(bool onoff)
	{
		this.ison = onoff;
	}
}
