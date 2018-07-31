using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;
public class CameraDamageEffect : MonoBehaviour {
    //트위닝을 할 게임오브젝트.
    public Transform TargetTransform = null;
    //타겟의 메시렌더러 컴포넌트
    private MeshRenderer TargetMeshRenderer = null;
    //타겟의 원본 칼라를 저장해놓습니다.
    private Color TargetOriginalColor;
    //현재 트위닝되고 있는 트위너.
    private Tweener CurrentTweener = null;

	// Use this for initialization
	void Start () {
        //트위너 초기화.
        HOTween.Init(true, true, true);
        //타겟이 있다면.
        if(TargetTransform != null)
        {
            //타겟의 메쉬렌더러를 구해놓고.
            TargetMeshRenderer = TargetTransform.GetComponent<MeshRenderer>();
            //타겟의 원본 칼라도 구해 놓고.
            TargetOriginalColor = TargetMeshRenderer.material.color;
            //타겟의 렌더러 컴포넌트를 꺼서 화면에 그려지지 않도록 합니다.
            TargetMeshRenderer.enabled = false;
        }
	}
    //트위닝이 끝나면 호출될 함수.연출이 끝났다는 것을 알 수 있습니다.
    void TweenFinished()
    {
        TargetMeshRenderer.enabled = false;
        TargetMeshRenderer.material.color = TargetOriginalColor;
    }
    public void DamageEffectTweenStart()
    {   
        //현재 트위닝하고있는 연출이 있고 아직 끝나지 않았다면 중복호출하지 않습니다.
        if(CurrentTweener != null && CurrentTweener.isComplete == false)
        {
            return;
        }
        //타겟이 있다면.
        if(TargetTransform != null)
        {   //렌더러를 켜서 화면에 그려지도록 하고.
            TargetMeshRenderer.enabled = true;
            Material targetMaterial = TargetMeshRenderer.material;
            Color colorTo = TargetOriginalColor;
            //깜빡이는 용도의 칼라값을 구해놓습니다.
            colorTo.a = 0.0f;
            //트위닝 이펙트를 시작합니다. 0.2초동안 알파값을 0으로 줄였다가 다시 1로.
            CurrentTweener = HOTween.To(targetMaterial, 0.2f, new TweenParms()
                .Prop("color", colorTo)
                .Loops(1, LoopType.Yoyo)
                .OnStepComplete(TweenFinished));
        }
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Space) == true)
        {
            DamageEffectTweenStart();
        }
	}
}
