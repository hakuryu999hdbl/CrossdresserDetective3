using UnityEngine;

public class ChargeSkillState : EnemyBaseState
{
    private enum Phase
    {
        Ready,
        Move,
        Hit
    }

    private Phase phase;
    private float timer;

    public override void EnterState(EnemyController enemy)
    {
        enemy.checkArea.SetChargeSkillColor();//设置视野范围颜色



        //点位赋值
        if (enemy.targetPoint == null)
        {
            if (enemy.attackList.Count > 0)
                enemy.targetPoint = enemy.attackList[0];
        }

        if (enemy.targetPoint == null)
        {
            enemy.TransitionToState(enemy.patrolState);
            return;
        }

        //记录目标的x位置
        Vector2 _dir = enemy.targetPoint.position - enemy.transform.position;

        enemy.chargeDir = _dir.x >= 0 ? Vector2.right : Vector2.left;
        enemy.chargeTargetX = enemy.targetPoint.position.x;

        //Debug.Log("Charge Target X = " + enemy.chargeTargetX);





        phase = Phase.Ready;
        timer = enemy.chargeReadyTime;

        enemy.rb.velocity = Vector2.zero;

        SetskillInvulnerable_Off(enemy);

        enemy.anim.SetInteger("skillState", 1);
        enemy.animState = 4;

        if (enemy.targetPoint != null)
        {
            Vector2 dir = enemy.targetPoint.position - enemy.transform.position;

            // 横版冲撞只取 X 方向
            enemy.chargeDir = dir.x >= 0 ? Vector2.right : Vector2.left;

            Vector3 scale = enemy.transform.localScale;
            scale.x = Mathf.Abs(scale.x) * enemy.chargeDir.x;
            enemy.transform.localScale = scale;
        }

        //if (enemy.aimUI != null)
        //{
        //    enemy.aimUI.gameObject.SetActive(true);
        //}


        //Debug.Log(enemy.targetPoint.position);
        //Debug.Log(enemy.chargeDir);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.isDead)
        {
            //enemy.animState = 3;//State为3为死亡
            return;
        }// || enemy.isHurt
           

        if (enemy.attackList == null || enemy.attackList.Count <= 0)
        {
            enemy.TransitionToState(enemy.patrolState);
            return;
        }

        enemy.targetPoint = enemy.attackList[0];

        switch (phase)
        {
            case Phase.Ready:
                UpdateReady(enemy);
                break;

            case Phase.Move:
                UpdateMove(enemy);
                break;

            case Phase.Hit:
                UpdateHit(enemy);
                break;
        }
    }

    private void UpdateReady(EnemyController enemy)
    {
        enemy.rb.velocity = Vector2.zero;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            //if (enemy.aimUI != null)
            //{
            //    enemy.aimUI.SetTrigger("Flash");
            //}

            phase = Phase.Move;
            timer = enemy.chargeMaxTime;

            SetskillInvulnerable_On(enemy);

            enemy.anim.SetInteger("skillState", 2);
            enemy.animState = 4;
        }
    }

    private void UpdateMove(EnemyController enemy)
    {
        timer -= Time.deltaTime;

        enemy.rb.velocity = new Vector2(
            enemy.chargeDir.x * enemy.chargeSpeed,
            enemy.rb.velocity.y
        );

        bool hitWall = enemy.IsWallAheadByDir();

        bool reachedTargetX =
            enemy.chargeDir.x > 0
                ? enemy.transform.position.x >= enemy.chargeTargetX
                : enemy.transform.position.x <= enemy.chargeTargetX;

        if (hitWall || reachedTargetX || timer <= 0f)
        {
            StartHit(enemy);
        }
    }

    private void StartHit(EnemyController enemy)
    {
        enemy.rb.velocity = Vector2.zero;

        phase = Phase.Hit;
        timer = enemy.chargeRecoveryTime;

        SetskillInvulnerable_Off(enemy);

        enemy.lastChargeTime = Time.time;

        enemy.anim.SetInteger("skillState", 3);
        enemy.animState = 4;
    }

    private void UpdateHit(EnemyController enemy)
    {
        enemy.rb.velocity = Vector2.zero;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            enemy.anim.SetInteger("skillState", 0);
            enemy.animState = 0;

            enemy.EnterBattleState();//虚类进入战斗
        }
    }

    public override void ExitState(EnemyController enemy)
    {
        enemy.rb.velocity = Vector2.zero;

        SetskillInvulnerable_Off(enemy);

        enemy.anim.SetInteger("skillState", 0);
        enemy.animState = 0;

        //if (enemy.aimUI != null)
        //{
        //    enemy.aimUI.gameObject.SetActive(false);
        //}
    }



    void SetskillInvulnerable_On(EnemyController enemy) 
    {
        enemy.character.skillInvulnerable = true;//技能无敌
        enemy.InvulnerableSign.SetActive(true);
        enemy.frameEvent.HalfShowSkeleton();
    }

    void SetskillInvulnerable_Off(EnemyController enemy)
    {
        enemy.character.skillInvulnerable = false;//技能无敌关闭
        enemy.InvulnerableSign.SetActive(false);
        enemy.frameEvent.ShowSkeleton();
    }
}