using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeControl : MonoBehaviour {

    public GameObject Player = null;
    public Vector3 PickPosition = Vector3.zero;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if(Input.GetMouseButtonDown(0))
        {
            if(Player == null)
            {
                Player = GameObject.CreatePrimitive(PrimitiveType.Cube);
                Player.transform.position = PickPosition;
                Player.AddComponent<CubeMove>();
            }
        }
	}

    private void FixedUpdate()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit raycastHit = new RaycastHit();
        int BackgroundLayer = LayerMask.NameToLayer("Background");
        int PickLayer = 1 << BackgroundLayer;
        if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity, PickLayer))
        {
            PickPosition = raycastHit.point;
            if(Player != null){
                Player.SendMessage("SetNextPosition", PickPosition);
            }
        }
    }
}
