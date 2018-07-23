using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // UGUI 클래스 모음 

public class HPBarControl : MonoBehaviour {

    //HPBar UI 게임 오브젝트
    public GameObject HPBar = null;
    public Slider mySlider = null; // 슬라이더 HPBar
    public Text myText = null; // 현재 HP/최대HP를 출력할 텍스트 
    public int CurrentHP = 10; // 현재 HP
    public int MaxHP = 10; // 최대 HP

	// Use this for initialization
	void Start () {
        HPBar = (GameObject)Instantiate(Resources.Load("Prefabs/HPBar"), transform.position, Quaternion.identity);
        // HPBar 생성 
        HPBar.transform.SetParent(GameObject.FindWithTag("Canvas").transform);
        // HPBar의 부모를 Canvas 태그가 달린 게임 오브젝트로 지정
        HPBar.transform.localPosition = Vector3.zero;
        // HPBar 위치 초기화

        mySlider = HPBar.GetComponent<Slider>(); // 슬라이더 컴포넌트 얻어옴
        myText = HPBar.transform.Find("Text").GetComponent<Text>();
	}
	
    public void SetHP(int newHP){
        MaxHP = newHP;
        CurrentHP = newHP;


    }
	// Update is called once per frame
	void Update () {
        // 현재 내 월드차원의 좌표를 이용하여 모니터 스크린상의 2차원 좌표를 구합니다.
        Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        GameObject canvas = GameObject.FindWithTag("Canvas");
        // Canvas 태그를 갖는 Canvas 게임오브젝트를 찾고 
        float CenterX = canvas.GetComponent<RectTransform>().position.x;
        // Canvas 가 갖고 있는 위치값을 얻어온다
        float CenterY = canvas.GetComponent<RectTransform>().position.y;
        Vector3 newPosition = new Vector3(screenPosition.x - CenterX, screenPosition.y - CenterY + 30.0f, 0.0f);
        // 내 위치로 얻은 스크린좌표로부터 캔버스가 위치한 오프셋만큼 빼준다

        HPBar.transform.localPosition = newPosition;
        // HPBar 위치를 새로 지정해준다
        myText.text = CurrentHP.ToString() + "/" + MaxHP.ToString();
        // Text 라벨 컴포넌트에서 촐력할 HP수치를 만들어준다

        float ratio = (float)CurrentHP / (float)MaxHP;
        mySlider.value = ratio;
	}

    //Bettle Control에서 AttackCollider 트리거가 내 영역 안에 생겼을 경우 호출
    void OnDamage(int damage){
        CurrentHP = CurrentHP - damage;
        if(CurrentHP <= 0)
        {
            Destroy(HPBar);
            Destroy(gameObject); // Bettle 제거
        }
    }
}
