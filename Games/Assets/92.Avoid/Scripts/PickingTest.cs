using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickingTest : MonoBehaviour {
    //픽킹한 게임오브젝트.
    public GameObject PickGameObject = null;


	// Use this for initialization
	void Start () {
        //x position
		for(int index1 = -3; index1 < 3; index1++)
        {   //y position
            for(int index2 = -3; index2 < 3;index2++)
            {   //Quad 게임오브젝트를 생성.
                GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
                //위치를 지정.
                quad.transform.position = new Vector3(index1, index2, 0f);
                //매트리얼의 색깔은 노랑색으로.
                quad.GetComponent<MeshRenderer>().material.color = Color.yellow;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	    //만약 마우스 왼쪽 버튼을 클릭했다면.
        if(Input.GetMouseButtonDown(0) == true)
        {
            //현재 마우스 위치에서 레이를 만들고 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //충돌 검출된 정보를 담을 레이캐스트 클래스를 만들어두고.
            RaycastHit raycastHit = new RaycastHit();
            //레이캐스트를 이용해서 충돌검출을 합니다.
            if(Physics.Raycast(ray, out raycastHit, Mathf.Infinity) == true)
            {
                //이전에 픽킹한 게임오브젝트가 있다면.
                if (PickGameObject != null)
                {   // 그 게임오브젝트의 매트리얼 색깔을 노란색으로 돌려놓고.
                    PickGameObject.GetComponent<MeshRenderer>().material.color =
                        Color.yellow;
                }
                //새로 픽킹한 게임오브젝트를얻어오고
                GameObject hitObj = raycastHit.collider.gameObject;
                //메쉬렌더러도 얻어오고
                MeshRenderer meshRenderer = hitObj.GetComponent<MeshRenderer>();
                //색깔은 빨강으로 변경.
                meshRenderer.material.color = Color.red;
                //픽킹한 게임오브젝트에 담아둡니다.
                PickGameObject = hitObj;
            }
        }
        //마우스 오른쪽 버튼을 클릭했다면.
        if(Input.GetMouseButtonDown(1) == true)
        {
            //현재 마우스 포지션으로 레이를 만들고
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //충돌 정보를 담을 클래스를 만들고.
            RaycastHit raycastHit = new RaycastHit();
            //백그라운드 레이어를 얻어서
            int BackgroundLayer = LayerMask.NameToLayer("Background");
            int BackgroundLayer2 = LayerMask.NameToLayer("Background2");
            //실제 픽킹에 사용하기위한 레이어값을 얻고 (시프트연산)
            int PickLayer = 1 << BackgroundLayer | 1 << BackgroundLayer2;
            // BackLayer:8 / PickLayer:256 -> BackLayer:8 / PickLayer:768 (256 + 512)
            Debug.Log("BackLayer:" + BackgroundLayer + " / PickLayer:" + PickLayer);
            // 레이캐스트를 이용해서 충돌검출을 했는데 충돌체를 찾았다면.
            if(Physics.Raycast(ray,out raycastHit, Mathf.Infinity,PickLayer) == true)
            {   //검출한 충돌체에서 게임오브젝트를 얻어오고.
                GameObject hitObj = raycastHit.collider.gameObject;
                //바로 이전에 선택됐던 게임오브젝트는 노랑색으로 바꿔주고.
                if(PickGameObject != null)
                {
                    PickGameObject.GetComponent<MeshRenderer>().material.color =
                        Color.yellow;
                }
                //새로 픽킹한 게임오브젝트는 파랑색으로 바꿔줍니다.
                MeshRenderer meshRenderer = hitObj.GetComponent<MeshRenderer>();
                meshRenderer.material.color = Color.blue;
                Debug.Log("HitObj :" + hitObj.name, hitObj);
                PickGameObject = hitObj;
            }
        }
	}

    private void OnGUI()
    {
        GUILayout.Label("Mouse X:" + Input.mousePosition.x.ToString() +
            " Y:" + Input.mousePosition.y.ToString());
    }
}
