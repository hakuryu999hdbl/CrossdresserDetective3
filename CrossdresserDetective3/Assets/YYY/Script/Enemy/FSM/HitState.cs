using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.checkArea.SetHitColor();//设置视野范围白色

        enemy.hitTimer = enemy.stunTime;
        enemy.animState = 0;

        enemy.anim.ResetTrigger("attack");
        enemy.anim.ResetTrigger("skill");
        //enemy.anim.SetTrigger("hit");

        if (enemy.rb != null)
            enemy.rb.velocity = Vector2.zero;
    }

    public override void OnUpdate(EnemyController enemy)
    {
        enemy.hitTimer -= Time.deltaTime;

        if (enemy.hitTimer > 0)
            return;

        enemy.isDizzy = false;

        if (enemy.attackList.Count > 0)
            enemy.TransitionToState(enemy.attackState);
        else
            enemy.TransitionToState(enemy.patrolState);
    }
}
