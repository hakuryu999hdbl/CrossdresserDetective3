using UnityEngine;

public class PatrolState : EnemyBaseState
{
    private float timer;
    private bool isWalking;

    // 前方障碍确认
    private bool isCheckingObstacle;
    private float obstacleCheckTimer;

    // 第一次碰到障碍后，等待门打开
    private const float ObstacleConfirmTime = 0.25f;

    public override void EnterState(EnemyController enemy)
    {
        enemy.checkArea.SetPatrolColor();
        enemy.animState = 0;

        isCheckingObstacle = false;
        obstacleCheckTimer = 0f;

        // 理论上 SetFirstDirection 已经设置过
        // 这里只是防止 patrolDir 意外为0
        if (enemy.patrolDir == 0)
        {
            enemy.patrolDir = enemy.startFaceDir >= 0 ? 1 : -1;
        }

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

            case EnemyPatrolMode.RandomGuard:
                enemy.FaceStartDirection();
                isWalking = false;
                ResetGuardTurnTimer(enemy);
                break;

        }
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.attackList.Count > 0)
        {
            enemy.EnterBattleState();
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

            case EnemyPatrolMode.RandomGuard:
                UpdateRandomGuard(enemy);
                break;
        }
    }

    private void UpdateGuard(EnemyController enemy)
    {
        enemy.animState = 0;
        enemy.rb.velocity =
            new Vector2(0f, enemy.rb.velocity.y);
    }

    private void UpdateRandomGuard(EnemyController enemy)
    {
        enemy.animState = 0;

        if (enemy.rb != null)
        {
            enemy.rb.velocity =
                new Vector2(0f, enemy.rb.velocity.y);
        }

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            enemy.TurnAround();
            ResetGuardTurnTimer(enemy);
        }
    }

    private void ResetGuardTurnTimer(EnemyController enemy)
    {
        timer = Random.Range(
            enemy.minGuardTurnTime,
            enemy.maxGuardTurnTime
        );
    }

    private void UpdateContinuousPatrol(EnemyController enemy)
    {
        // 正在确认门或墙时，不继续移动
        if (CheckObstacle(enemy))
            return;

        enemy.animState = 1;
        enemy.MovePatrol();
    }

    private void UpdateRandomPatrol(EnemyController enemy)
    {
        if (isWalking)
        {
            // 正在确认门或墙时暂停走路计时
            if (CheckObstacle(enemy))
                return;

            enemy.animState = 1;
            enemy.MovePatrol();

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                StartIdle(enemy);
            }
        }
        else
        {
            enemy.animState = 0;
            enemy.rb.velocity =
                new Vector2(0f, enemy.rb.velocity.y);

            timer -= Time.deltaTime;

            if (timer <= 0f)
            {
                StartWalk(enemy);
            }
        }
    }

    /// <summary>
    /// 检测前方究竟是门还是墙。
    /// 第一次碰到时短暂停顿：
    /// 障碍消失 = 门已经打开，继续前进；
    /// 障碍仍存在 = 实墙，转身。
    /// </summary>
    private bool CheckObstacle(EnemyController enemy)
    {
        bool obstacleAhead = enemy.IsWallAheadByDir();

        // 前面已经没有障碍：
        // 如果刚才撞到的是门，说明门已经打开
        if (!obstacleAhead)
        {
            isCheckingObstacle = false;
            obstacleCheckTimer = 0f;
            return false;
        }

        // 第一次检测到障碍，开始短暂等待
        if (!isCheckingObstacle)
        {
            isCheckingObstacle = true;
            obstacleCheckTimer = ObstacleConfirmTime;

            StopForObstacle(enemy);
            return true;
        }

        // 第二阶段：等待门打开
        obstacleCheckTimer -= Time.deltaTime;
        StopForObstacle(enemy);

        if (obstacleCheckTimer > 0f)
            return true;

        // 等待后障碍依然存在，确定是墙
        if (enemy.IsWallAheadByDir())
        {
            enemy.TurnAround();
        }

        isCheckingObstacle = false;
        obstacleCheckTimer = 0f;

        // 本帧不移动，下一帧朝反方向继续走
        return true;
    }

    private void StopForObstacle(EnemyController enemy)
    {
        enemy.animState = 0;

        if (enemy.rb != null)
        {
            enemy.rb.velocity =
                new Vector2(0f, enemy.rb.velocity.y);
        }
    }

    private void StartIdle(EnemyController enemy)
    {
        isWalking = false;
        enemy.animState = 0;

        timer = Random.Range(
            enemy.minIdleTime,
            enemy.maxIdleTime
        );
    }

    private void StartWalk(EnemyController enemy)
    {
        isWalking = true;
        enemy.animState = 1;

        // 删除原本35%概率随机回头
        // 停止结束后继续沿原方向走

        timer = Random.Range(
            enemy.minWalkTime,
            enemy.maxWalkTime
        );
    }
}