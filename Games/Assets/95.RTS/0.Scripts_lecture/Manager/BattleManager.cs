using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : SingletonMonobehaviour<BattleManager>
{

    public void AttackEntity(Entity attacker, Entity target, int damage, Vector3 hittingPosition)
    {
        if (attacker == null)
        {
            Debug.Log("Attack is null");
            return;
        }
        if (target == null)
        {
            Debug.Log("Target is null");
            return;
        }
        if (target.IsDead() == true)
        {
            return;
        }

        target.hp -= damage;
        target.OnDamaged(damage);

        //target.SendMessage("ColorChangeTrigger", Color.red, SendMessageOptions.DontRequireReceiver);

        //behit effect

        if(target.IsDead() == true)
        {
            target.DestoryEntity();
            attacker.OnTargetDestroy();

            //ShakeCamera

            Debug.Log(attacker.name + "이 " + target.name + "을 공격하여 " + damage.ToString() + "의 피해를 입히고 파괴시켰습니다." + " 잔여 HP:" + target.hp.ToString());

        }else
        {
            Debug.Log(attacker.name + "이 " + target.name + "을 공격하여 " + damage.ToString() + "의 피해를 입혔습니다." + " 잔여 HP:" + target.hp.ToString());
        }

    }

}
