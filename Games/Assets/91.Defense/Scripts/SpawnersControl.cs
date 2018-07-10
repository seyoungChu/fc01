using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnersControl : MonoBehaviour {

	public GameObject[] Spawners = null;
	public GameObject SpawnObject = null;

	public float SpawnDelay = 1.0f;
	private float LastSpawnTime = 0.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time > LastSpawnTime + SpawnDelay) {
			LastSpawnTime = Time.time;
			GameObject Beetle = (GameObject)Instantiate (SpawnObject,
				                    Spawners [0].transform.position,
				                    Spawners [0].transform.rotation);
			for (int i = 1; i < Spawners.Length; i++) {
				Beetle.SendMessage ("SetTargetPosition", Spawners [i].transform.position);
			}
		}
	}

	/// <summary>
	/// Draw gizmos only if it's selected in the editor (scene view)
	/// Gizmos aren't seen in the real game.
	/// </summary>
	private void OnDrawGizmosSelected(){
		Gizmos.DrawSphere (transform.position, 1.0f);
	}

	private void OnDrawGizmos(){
		if (Spawners == null || Spawners.Length == 0)
			return;

		for (int i = 0; i < Spawners.Length; i++) {

			if (Spawners [i] != null) {
				Gizmos.color = Color.red;
				// Vector3.one = (1.0f, 1.0f, 1.0f)
				Gizmos.DrawWireCube (Spawners [i].transform.position, Vector3.one);

				Gizmos.color = Color.white;
				Gizmos.DrawWireSphere (Spawners [i].transform.position, 1.0f);

				if (i < Spawners.Length - 1 && Spawners[i+1] != null) {
					Gizmos.DrawLine (Spawners [i].transform.position, Spawners [i + 1].transform.position);
				}
			}
		}
	}
}
	
