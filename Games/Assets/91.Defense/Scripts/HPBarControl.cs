using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//UGUI 클래스 모음.
public class HPBarControl : MonoBehaviour {
    //HPBar UI 게임오브젝트.
    public GameObject HPBar = null;
    public Slider mySlider = null;//슬라이더 HPBar
    public Text myText = null; //현재HP/최대HP 를 출력할 텍스트.
    public int CurrentHP = 10; //현재HP
    public int MaxHP = 10;      //최대HP
	// Use this for initialization
	void Start () {
        //HPBar 생성.
        HPBar = (GameObject)Instantiate(Resources.Load("Prefabs/HPBar"),
            transform.position, Quaternion.identity);
        //HPBar의 부모를 Canvas GameObject로 지정.
        //지정한 이유는 모든 UGUI게임오브젝트는 Canvas의 차일드로 붙어있어야 화면에 나타납니다.
        HPBar.transform.SetParent(GameObject.FindWithTag("Canvas").transform);
        //HPBar의 위치를 초기화.
        HPBar.transform.localPosition = Vector3.zero; //0.0f,0.0f,0.0f
        //슬라이더 컴포넌트를 얻어옵니다.
        mySlider = HPBar.GetComponent<Slider>();
        //HPBar의 Child 게임오브젝트중 Text라는 이름의 게임오브젝트의 Text컴포넌트를 얻어온다.
        myText = HPBar.transform.Find("Text").GetComponent<Text>();
	}

    public void SetHP(int newHP)
    {
        MaxHP = newHP;
        CurrentHP = newHP;
    }
	
	// Update is called once per frame
	void Update () {
        //현재 내 월드차원의 좌표를 이용해 모니터 스크린상의 2차원 좌표를 구합니다.
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        //Canvas 태그를 갖는 Canvas게임오브젝트를 찾고.
        GameObject canvas = GameObject.FindWithTag("Canvas");
        //Canvas 가 갖고있는 위치값을 얻어옵니다.
        float CenterX = canvas.GetComponent<RectTransform>().position.x;
        float CenterY = canvas.GetComponent<RectTransform>().position.y;
        //내위치로 얻은 스크린좌표로부터 캔버스가 위치한 오프셋 만큼 빼줍니다.
        Vector3 newPosition = new Vector3(screenPosition.x - CenterX,
                                        screenPosition.y - CenterY + 30.0f,
                                        0.0f);
        //HPBar의 위치를 새로 지정해줍니다.
        HPBar.transform.localPosition = newPosition;
        //Text 라벨 컴포넌트에서 출력할 HP수치를 만들어줍니다.
        myText.text = CurrentHP.ToString() + "/" + MaxHP.ToString();// "100/100"
        //현재 HP와 MaxHP를 통해 슬라이더의 값을 구합니다.
        float ratio = (float)CurrentHP / (float)MaxHP;
        mySlider.value = ratio;
	}
    //BeetleControl에서 AttackCollider트리거가 내 영역안에 생겼을경우 호출.
    void OnDamage(int damage)
    {
        CurrentHP = CurrentHP - damage;
        if(CurrentHP <= 0)
        {
            Destroy(HPBar);//HPBar 제거.
            Destroy(gameObject);//Beetle 제거.
        }
    }
}
