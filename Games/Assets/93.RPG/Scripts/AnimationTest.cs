using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationTest : MonoBehaviour {
    //테스트 타입
    public enum TestState { Loop, Mixing, Additive }
    //현재 테스트 타입.
    public TestState state = TestState.Loop;
    //애니메이션 컴포넌트 캐싱.
    private Animation MyAnimation = null;
    //이동 애니메이션 클립.
    public AnimationClip MoveClip;
    //공격 애니메이션 클립.
    public AnimationClip AttackClip;
    //믹싱 애니메이션을 적용할 뼈대.
    public Transform MixingTransform = null;

	// Use this for initialization
	void Start () {
        //애니메이션 컴포넌트 캐싱.
        MyAnimation = GetComponent<Animation>();
        if(state == TestState.Loop)
        {   //애니메이션 컴포넌트의 기본 재생 모드를 반복모드로 설정합니다.
            MyAnimation.wrapMode = WrapMode.Loop;
        }else if(state == TestState.Mixing)
        {   //공격 애니메이션만 반복 모드로 설정하게 됩니다.
            MyAnimation[AttackClip.name].wrapMode = WrapMode.Loop;
            //애니메이션을 적용할 뼈대 트랜스폼을 지정해주고.
            MyAnimation[AttackClip.name].AddMixingTransform(MixingTransform);
            //애니메이션을 재생시킵니다.
            MyAnimation.CrossFade(AttackClip.name);
        }else if(state == TestState.Additive)
        {
            //달리는 애니메이션을 반복모드로 재생합니다.
            MyAnimation[MoveClip.name].wrapMode = WrapMode.Loop;
            MyAnimation.Play(MoveClip.name);
            //애니메이션을 더해주기
            MyAnimation[AttackClip.name].blendMode = AnimationBlendMode.Additive;
            MyAnimation[AttackClip.name].layer = 10;
            MyAnimation[AttackClip.name].weight = 1.0f;
            MyAnimation[AttackClip.name].enabled = true;
            MyAnimation[AttackClip.name].wrapMode = WrapMode.Loop;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
