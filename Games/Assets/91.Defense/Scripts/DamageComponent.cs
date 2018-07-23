using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageComponent : MonoBehaviour {

    public int Damage = 100;
    //데미지를 발생시킨 게임오브젝ㅌ의 ID 저장
    public int OwnerID = -1;

    // 데미지 지정
    public void SetDamage(int newDamage)
    {
        Damage = newDamage;
    }

    //지정한 데미지 얻어오기
    public int GetDamage()
    {
        return Damage;    
    }
}
