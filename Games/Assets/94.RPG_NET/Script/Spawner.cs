using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {
	
	public float spawntime = 0.5f;
	
	public float lasttime;
	
	public GameObject monster;
	
	

	// Use this for initialization
	void Start () {
		lasttime = Time.time;	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Time.time > spawntime + lasttime && Network.isServer && Network.connections.Length > 0)
		{
			lasttime = Time.time;
			//GameObject.Instantiate(monster, transform.position , transform.rotation);
			Network.Instantiate(monster,transform.position, transform.rotation,1);
		}
	
	}
}
