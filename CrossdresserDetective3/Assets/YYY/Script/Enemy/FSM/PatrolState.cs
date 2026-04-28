using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : EnemyBaseState
{
    public override void EnterState(EnemyController enemy)
    {
        enemy.animState = 0;
        enemy.SwitchPoint();//距离目标最近的时候切换远目标

        isWalk = false;
        waitTimer = 2f;
        wallWait = false;
    }

    float waitTimer = 2f;
    bool isWalk = false;
    bool wallWait;

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.attackList.Count > 0)
        {
            enemy.TransitionToState(enemy.attackState);

            return;
        }//只要索敌列表不为0就进入攻击状态


        // 等待中
        if (!isWalk)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                isWalk = true;

                // 如果是撞墙等待结束，就切目标
                if (wallWait)
                {
                    enemy.SwitchToOtherPoint();
                    wallWait = false;
                }
            }

            return;
        }


        // 前方碰墙：开始等待，不立刻转身
        if (enemy.IsWallAhead())
        {
            enemy.animState = 0;
            isWalk = false;
            wallWait = true;
            waitTimer = 2f;
            return;
        }

        enemy.animState = 1;
        enemy.MoveToTarget();

        // 到达AB点：正常重进巡逻，保留原本“站一会再走”
        if (Mathf.Abs(enemy.targetPoint.position.x - enemy.transform.position.x) < 0.01f)
        {
            enemy.TransitionToState(enemy.patrolState);
            return;
        }



    }




   
}
