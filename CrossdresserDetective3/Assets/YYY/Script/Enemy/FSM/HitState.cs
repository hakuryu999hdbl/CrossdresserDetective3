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




        // 清除技能层内部状态
        enemy.anim.SetInteger("skillState", 0);
        if (enemy is Enemy_4 blockEnemy)
        {
            blockEnemy.isBlocking = false;
            blockEnemy.isCountering = false;
        }



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
            enemy.EnterBattleState();//虚类进入战斗
        else
            enemy.TransitionToState(enemy.patrolState);
    }
}
