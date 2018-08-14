using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinSpawner : MonoBehaviour {
    //스폰할 몬스터.
    public GameObject goblin = null;
    //현재 스폰한 몬스터 리스트.
    public List<GameObject> GoblinList = new List<GameObject>();
    //최대 스폰 갯수.
    public int MaxCount = 100;
	// Use this for initialization
	void Start () {
        InvokeRepeating("SpawnMonster", 3f, 5f);
	}
    void SpawnMonster()
    {
        int currentCount = 0;
        foreach(GameObject monster in GoblinList)
        {
            if(monster != null)
            {
                currentCount++; //하나씩 증가.
            }
        }
        if(currentCount >= MaxCount)
        {
            return;
        }
        Vector3 SpawnPos = new Vector3(
            Random.Range(-100.0f, 100.0f),
            1000.0f,
            Random.Range(-100.0f, 100.0f));
        Ray ray = new Ray(SpawnPos, Vector3.down);
        RaycastHit raycastHit = new RaycastHit();
        if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
        {
            SpawnPos.y = raycastHit.point.y;
        }
        GameObject Goblin = Instantiate(goblin, SpawnPos, Quaternion.identity);
        GoblinList.Add(Goblin);
    }
	
}
