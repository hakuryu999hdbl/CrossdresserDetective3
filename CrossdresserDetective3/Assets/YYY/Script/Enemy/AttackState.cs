using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        //Debug.Log("发现敌人！！！！");
        enemy.targetPoint = enemy.attackList[0];//列表第一个作为追踪目标（最近的目标）
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.attackList.Count <= 0)
        {
            enemy.TransitionToState(enemy.patrolState);//一旦列表为0恢复巡逻
        }


        //出现多个目标的时候，最近的优先
        if (enemy.attackList.Count > 1) 
        {

            for(int i = 0; i < enemy.attackList.Count; i++)
            {
                if (Mathf.Abs(enemy.transform.position.x - enemy.attackList[i].position.x) < Mathf.Abs(enemy.transform.position.x - enemy.targetPoint.position.x)) 
                {

                    enemy.targetPoint = enemy.attackList[i];

                }//距离当前目标之间的每一个目标的X差距绝对值

            }

        }


        //根据tag来判断不同动作
        if (enemy.targetPoint.CompareTag("Player"))
            enemy.AttackAction();
        if (enemy.targetPoint.CompareTag("Bomb"))
            enemy.SkillAction();



        enemy.MoveToTarget();
    }
}
