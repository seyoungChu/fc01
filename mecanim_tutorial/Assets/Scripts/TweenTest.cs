using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Holoville.HOTween;
public class TweenTest : MonoBehaviour {

    public string AnimString = string.Empty; //"";
    public float AnimFloat = 10.0f;
    public Transform otherCubeTransform1 = null;
    public Transform otherCubeTransform2 = null;
	// Use this for initialization
	void Start () {
        //Tweener 초기화.
        HOTween.Init(true, true, true);
        //transform을 타겟으로 4초동안 내 위치를 목표 위치로 트위닝 시킵니다.
        //HOTween.To(transform, 4, "position", new Vector3(-3.0f, 6.0f, 0.0f));
        //이 클래스를 타겟으로 2초동안 클래스 멤버변수인 AnimString의 값을 변화시키는데
        //무한반복(-1)을 시키면서 반복되는 타입은 요요타입.
        HOTween.To(this, 2, new TweenParms()
            .Prop("AnimString", "Hello World,Show me the money!!")
            .Loops(-1, LoopType.Yoyo));
        //이 클래스를 타겟으로 2초동안 클래스 멤버변수인 AnimFloat의 값을 변화시키는데
        //무한반복(-1)을 하면서 Restart(재시작)타입으로 반복.
        HOTween.To(this, 2, new TweenParms()
            .Prop("AnimFloat", 0.0f)
            .Loops(-1, LoopType.Restart));
        //트랜스폼을 타겟으로 3초동안 트랜스폼 멤버변수인 위치와 회전값을 변화시키는데
        //EaseInQuart라는 수식을 이용해서 한 트위닝이 끝났을때마다 TweenCompleted라는 함수를 호출.
        //HOTween.To(transform, 3, new TweenParms()
        //  .Prop("position", new Vector3(0, 4, 0), true)
        //  .Prop("rotation", new Vector3(0, 720, 0), true)
        //  .Ease(EaseType.EaseInQuart)
        //  .OnStepComplete(TweenCompleted)
        //  .Loops(-1, LoopType.YoyoInverse));

        Sequence sequence = new Sequence(new SequenceParms().Loops(-1, LoopType.Yoyo));
        Tweener tweener1 = HOTween.To(transform, 1, new TweenParms()
            .Prop("position", new Vector3(-4, 0, 0), true));
        Tweener tweener2 = HOTween.To(transform, 1, new TweenParms()
            .Prop("rotation", new Vector3(0, 720, 0), true));
        Tweener tweener3 = HOTween.To(transform, 1, new TweenParms()
            .Prop("position", new Vector3(0, 3, 0), true));
        Tweener tweener4 = HOTween.To(transform, 1, new TweenParms()
            .Prop("localScale", new Vector3(0, 2, 0), true));
        sequence.Append(tweener1);
        sequence.Append(tweener2);
        sequence.Append(tweener3);
        sequence.Append(tweener4);
        Color colorTo = GetComponent<MeshRenderer>().material.color; //white
        colorTo.a = 0.0f;
        Tweener tweener5 = HOTween.To(GetComponent<MeshRenderer>().material,
            sequence.duration * 0.5f, new TweenParms().Prop("color", colorTo));
        sequence.Insert(sequence.duration * 0.5f, tweener5);
        //sequence.Append(tweener5);
        sequence.Play();



	}

    void TweenCompleted()
    {
        Debug.Log("Tween Complete!!");
    }

    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(20, 20, 500, 100));
        GUILayout.Label("string:" + AnimString);
        GUILayout.Label("float:" + AnimFloat);
        GUILayout.EndArea();
    }

}
