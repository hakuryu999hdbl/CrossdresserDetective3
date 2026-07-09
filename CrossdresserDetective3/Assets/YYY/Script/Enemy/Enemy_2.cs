using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : EnemyController
{
    public override void EnterBattleState()
    {   

        if (useAimThrowSkill)
        {
            TransitionToState(aimThrowSkillState);
        }
        else
        {
            TransitionToState(attackState);
        }


    }
}
