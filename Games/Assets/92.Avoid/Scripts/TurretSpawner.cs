using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretSpawner : MonoBehaviour {
    public GameObject Turret = null;
	// Use this for initialization
	void Start () {
        //OnSpawnTurret이라는 함수를 3초 기다렸다가 5초간격으로 반복 호출해줍니다.
        InvokeRepeating("OnSpawnTurret", 3.0f, 5.0f);
        //CancelInvoke("OnSpawnTurret");
	}
	void OnSpawnTurret()
    {   // 0 ~ 3...
        int SelectChildNumber = Random.Range(0, transform.childCount - 1);
        Transform spawnPoint = transform.GetChild(SelectChildNumber);
        Instantiate(Turret, spawnPoint.position, spawnPoint.rotation);
    }
}
