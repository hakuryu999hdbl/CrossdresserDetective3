using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private float timer;
    private bool isWalking;

    public override void EnterState(EnemyController enemy)
    {
        enemy.checkArea.SetPatrolColor();//设置视野范围绿色
        enemy.animState = 0;

        // 第一次进入时随机方向
        if (enemy.patrolDir == 0)
        {
            enemy.patrolDir = Random.value < 0.5f ? -1 : 1;
        }

        StartIdle(enemy);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.attackList.Count > 0)
        {
            enemy.TransitionToState(enemy.attackState);
            return;
        }

        timer -= Time.deltaTime;

        if (isWalking)
        {
            enemy.animState = 1;

            if (enemy.IsWallAheadByDir())
            {
                enemy.TurnAround();
                StartIdle(enemy);
                return;
            }

            enemy.MovePatrol();

            if (timer <= 0)
            {
                StartIdle(enemy);
                return;
            }
        }
        else
        {
            enemy.animState = 0;

            if (timer <= 0)
            {
                StartWalk(enemy);
                return;
            }
        }
    }

    private void StartIdle(EnemyController enemy)
    {
        isWalking = false;
        enemy.animState = 0;
        timer = Random.Range(enemy.minIdleTime, enemy.maxIdleTime);
    }

    private void StartWalk(EnemyController enemy)
    {
        isWalking = true;
        enemy.animState = 1;

        // 每次开始走，有一定概率换方向
        if (Random.value < 0.35f)
        {
            enemy.TurnAround();
        }

        timer = Random.Range(enemy.minWalkTime, enemy.maxWalkTime);
    }
}