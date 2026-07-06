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

        //StartIdle(enemy);

        switch (enemy.patrolMode)
        {
            case EnemyPatrolMode.Guard:
                enemy.FaceStartDirection();
                isWalking = false;
                timer = 999999f;
                break;

            case EnemyPatrolMode.ContinuousPatrol:
                isWalking = true;
                timer = 999999f;
                break;

            case EnemyPatrolMode.RandomPatrol:
                StartIdle(enemy);
                break;
        }
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.attackList.Count > 0)
        {
            enemy.TransitionToState(enemy.attackState);
            return;
        }

        switch (enemy.patrolMode)
        {
            case EnemyPatrolMode.Guard:
                UpdateGuard(enemy);
                break;

            case EnemyPatrolMode.ContinuousPatrol:
                UpdateContinuousPatrol(enemy);
                break;

            case EnemyPatrolMode.RandomPatrol:
                UpdateRandomPatrol(enemy);
                break;
        }
    }

    private void UpdateGuard(EnemyController enemy)
    {
        enemy.animState = 0;
        enemy.rb.velocity = new Vector2(0f, enemy.rb.velocity.y);
    }

    private void UpdateContinuousPatrol(EnemyController enemy)
    {
        enemy.animState = 1;

        if (enemy.IsWallAheadByDir())
        {
            enemy.TurnAround();
            return;
        }

        enemy.MovePatrol();
    }

    private void UpdateRandomPatrol(EnemyController enemy)
    {
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

        if (Random.value < 0.35f)
        {
            enemy.TurnAround();
        }//一定几率回头

        timer = Random.Range(enemy.minWalkTime, enemy.maxWalkTime);
    }
}