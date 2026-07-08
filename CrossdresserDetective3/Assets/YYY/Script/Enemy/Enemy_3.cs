using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_3 : EnemyController
{
    public override void EnterBattleState()
    {


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


        TransitionToState(attackState);

    }
}
