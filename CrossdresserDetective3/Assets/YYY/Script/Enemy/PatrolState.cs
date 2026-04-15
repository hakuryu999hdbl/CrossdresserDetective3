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
    }

    float waitTimer = 2f;
    bool isWalk = false;

    public override void OnUpdate(EnemyController enemy)
    {
        if (!isWalk)
        {
            waitTimer -= Time.deltaTime;

            if (waitTimer <= 0)
            {
                isWalk = true;
            }

            return;
        }



        //var info = enemy.anim.GetCurrentAnimatorStateInfo(0);
        //Debug.Log(info.IsName("Idle"));

        if (isWalk)
        {
            enemy.animState = 1;
            enemy.MoveToTarget();
        }

        if (Mathf.Abs(enemy.targetPoint.position.x - enemy.transform.position.x) < 0.01f) 
        { 
            enemy.TransitionToState(enemy.patrolState);//一进入巡逻状态，先运行Start里写的Idle

        }//距离目标最近的时候切换远目标







        if (enemy.attackList.Count>0) 
        {
            enemy.TransitionToState(enemy.attackState);
        }//只要索敌列表不为0就进入攻击状态
      
    }
}
