using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : SingletonMonobehaviour<Main>
{
    GridManager gridManager;
    InputManager inputManager;
    EntityManager entityManager;
    DataManager dataManager;
    TimeManager timeManager;
    CameraManager cameraManager;
	SoundManager soundManager;
    

	// Use this for initialization
	void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;
        Application.runInBackground = true;


        this.gridManager = (GridManager)AddManager<GridManager>();
        this.gridManager.Init();

        this.inputManager = (InputManager)AddManager<InputManager>();

        this.entityManager = (EntityManager)AddManager<EntityManager>();

        this.dataManager = (DataManager)AddManager<DataManager>();

        this.timeManager = (TimeManager)AddManager<TimeManager>();

        this.cameraManager = (CameraManager)AddManager<CameraManager>();

		this.soundManager = (SoundManager)AddManager<SoundManager>();

		SceneManager.LoadScene("InGame");
    }

    UnityEngine.Component AddManager<T>()
    {
        GameObject newObject = new GameObject(typeof(T).ToString());
        newObject.transform.SetParent(transform);
        return newObject.AddComponent(typeof(T));
    }

    // Update is called once per frame
    void Update()
    {
        this.timeManager.UpdateTime();

        this.inputManager.TouchUpdate();

        this.inputManager.UpdateGameInput();

        this.entityManager.UpdateEntities();


    }

	private void LateUpdate()
	{
        this.cameraManager.CameraUpdate();
	}
}
