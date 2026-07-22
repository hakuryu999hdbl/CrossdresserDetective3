using UnityEngine;

public class BlockState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        Debug.Log("正式进入 BlockState");
        enemy.checkArea.SetBlockColor();//设置视野范围

        enemy.rb.velocity = Vector2.zero;

        enemy.anim.ResetTrigger("attack");

        // 和 ChargeSkillState 一样，修改 animState
        enemy.animState = 4;
        enemy.anim.SetInteger("skillState", 1);

        if (enemy is Enemy_4 blockEnemy)
        {
            blockEnemy.isBlocking = true;
            blockEnemy.isCountering = false;

            blockEnemy.SetSpark();
        }
    }

    public override void OnUpdate(EnemyController enemy)
    {

        if (enemy.isDizzy)
        {
            enemy.TransitionToState(enemy.hitState);
            return;
        }


        // 防御、反击期间固定技能状态，不允许移动
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);

        // 防止别的地方意外修改
        enemy.animState = 4;
    }

    public override void ExitState(EnemyController enemy)
    {
        enemy.anim.SetInteger("skillState", 0);
        enemy.animState = 0;

        if (enemy is Enemy_4 blockEnemy)
        {
            blockEnemy.isBlocking = false;
            blockEnemy.isCountering = false;
        }
    }
}