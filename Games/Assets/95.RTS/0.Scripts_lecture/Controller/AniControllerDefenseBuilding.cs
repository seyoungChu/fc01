using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniControllerDefenseBuilding : tk2dAniController
{
    public Dictionary<AnimationType, string> animationNameList; //애니메이션 클립 리스트.

    /// <summary>
    /// AnimationList,animationNameList를 각 객체에 맞게 세팅한다.
    /// </summary>
    public override void Init(EntityCategory category)
    {
        base.Init(category);

        this.animationNameList = new Dictionary<AnimationType, string>();

        this.animationNameList.Add(AnimationType.Attack, "lv"+currLevel.ToString()+ "_attack");

    }

    /// <summary>
    /// 애니메이션 재생 - [현재애니메이션타입][현재방향]으로 구성된 문자열을 가지고 tk2d 애니메이터에서 클립을 재생한다.
    /// </summary>
    public override void PlayAnimation(AnimationType animType, bool bLoop = true)
    {
        this.animator_main.Stop();
        this.animator_main.Play(animationNameList[animType]);

        base.PlayAnimation(animType, bLoop);
    }
}
