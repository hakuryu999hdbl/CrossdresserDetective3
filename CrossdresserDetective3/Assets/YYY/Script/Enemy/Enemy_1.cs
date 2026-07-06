using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : EnemyController
{
    public override void EnterBattleState()
    {
        // 太近时不要冲撞，直接普通近战
        //if (targetPoint != null)
        //{
        //    float distance = Vector2.Distance(transform.position, targetPoint.position);
        //
        //    if (useChargeSkill &&
        //        distance >= chargeMinDistance &&
        //        Time.time >= lastChargeTime + chargeCooldown &&
        //        Random.value < 0.5f)
        //    {
        //        TransitionToState(chargeSkillState);
        //        return;
        //    }
        //}
        //
        //TransitionToState(attackState);



        //点位赋值
        if (attackList.Count > 0)
        {
            targetPoint = attackList[0];
        }

        if (targetPoint == null)
        {
            TransitionToState(patrolState);
            return;
        }


        if (useChargeSkill)
        {
            TransitionToState(chargeSkillState);
        }
        else 
        {
            TransitionToState(attackState);
        }

        
    }
}
