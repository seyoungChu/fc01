//For Lecture

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AniControllerNormalUnit : tk2dAniController
{

    public Dictionary<AnimationType, string[]> animationNameList; //애니메이션 클립 리스트.

    public AnimationType currAnimation; //현재 애니메이션

    /// <summary>
    /// AnimationList,animationNameList를 각 객체에 맞게 세팅한다.
    /// </summary>
    public override void Init(EntityCategory category)
    {
        base.Init(category);

        animationNameList = new Dictionary<AnimationType, string[]>();

        //애니메이션이름을 보관할 리스트 - walk,attack
        string[] walklist = new string[8];
        string[] attacklist = new string[8];

        for (int i = 0; i < 8; i++)
        {
            walklist[i] = "lv" + currLevel.ToString() + "_idle_" + ((Direction8Way)i).ToString();
            attacklist[i] = "lv" + currLevel.ToString() + "_attack_" + ((Direction8Way)i).ToString();
        }

        animationNameList.Add(AnimationType.Walk, walklist);
        animationNameList.Add(AnimationType.Attack, attacklist);

        //현재방향,애니메이션 지정
        currDirection = (int)Direction8Way.se;
    }


    /// <summary>
    /// 애니메이션 재생 - [현재애니메이션타입][현재방향]으로 구성된 문자열을 가지고 tk2d 애니메이터에서 클립을 재생한다.
    /// </summary>
    public override void PlayAnimation(AnimationType animType, bool bLoop = true)
    {
        if (animator_main == null)
        {
            Debug.Log(gameObject.name + "의 animator_main이 null입니다.");
            return;
        }
        animator_main.Stop();
        animator_main.Play(animationNameList[animType][currDirection]);

        base.PlayAnimation(animType, bLoop);

    }
}
