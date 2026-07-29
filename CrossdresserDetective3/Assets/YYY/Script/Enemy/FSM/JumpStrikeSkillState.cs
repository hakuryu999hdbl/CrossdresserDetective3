using UnityEngine;

public class JumpStrikeSkillState : EnemyBaseState
{
    private enum Phase
    {
        Jump,
        Aim,
        Fall,
        Attack
    }

    private Phase phase;
    private float timer;

    private Enemy_5 enemy5;

    public override void EnterState(EnemyController enemy)
    {
        if (enemy.attackList.Count > 0)
        {
            enemy.targetPoint = enemy.attackList[0];
        }
        else
        {
            enemy.targetPoint = null;
        }//追寻攻击状态下的目标

        enemy5 = enemy as Enemy_5;

        if (enemy5 == null)
        {
            enemy.TransitionToState(enemy.patrolState);
            return;
        }


      
        enemy.FaceToPosition(enemy.targetPoint.position);  // 进入跳跃攻击时朝向目标


        enemy.checkArea.SetJumpStrikeSkillColor();//设置视野范围显示

        //enemy.StopMove();

        // 和 ChargeSkillState 一样，修改 animState
        enemy.animState = 4;

        phase = Phase.Jump;

        enemy.anim.SetInteger("skillState", 1);

        if (enemy5.jumpAimTarget != null)
            enemy5.jumpAimTarget.gameObject.SetActive(false);

        if (enemy5.jumpStrikeCollider != null)
            enemy5.jumpStrikeCollider.SetActive(false);
    }

    public override void OnUpdate(EnemyController enemy)
    {
        if (enemy.isDead)
            return;

        //if (enemy.isHurt)
        //{
        //    CancelSkill(enemy);
        //    return;
        //}

        switch (phase)
        {
            case Phase.Jump:
                //enemy.StopMove();
                break;

            case Phase.Aim:
                UpdateAim(enemy);
                break;

            case Phase.Fall:
                //enemy.StopMove();
                break;

            case Phase.Attack:
                //enemy.StopMove();
                break;
        }
    }

    private void UpdateAim(EnemyController enemy)
    {
        //enemy.StopMove();

        timer -= Time.deltaTime;

        if (enemy.targetPoint != null)
        {
            enemy5.jumpStrikeTargetPos =
                (Vector2)enemy.targetPoint.position +
                enemy5.landingOffset;

            if (enemy5.jumpAimTarget != null)
            {
                enemy5.jumpAimTarget.position =
                    enemy5.jumpStrikeTargetPos;
            }
        }

        if (timer <= 0f)
        {
            LockTargetAndFall(enemy);
        }
    }

    private void LockTargetAndFall(EnemyController enemy)
    {
        phase = Phase.Fall;



        if (enemy5.jumpAimTarget != null)
            enemy5.jumpAimTarget.gameObject.SetActive(false);

        enemy.transform.position =
            enemy5.jumpStrikeTargetPos;


        // 落地出现时朝向玩家
        if (enemy.targetPoint != null)
            enemy.FaceToPosition(enemy.targetPoint.position);


        enemy.anim.SetInteger("skillState", 3);

        enemy.frameEvent.ShowSkeleton();//进入显示状态
    }

    public void StartAim(EnemyController enemy)
    {


        if (enemy5 == null)
            enemy5 = enemy as Enemy_5;

        if (enemy5 == null)
            return;

        enemy5.isJumpStrikeUntargetable = true;//进入无敌状态

        if (enemy.targetPoint != null)
        {

            enemy5.jumpAimTarget.position =
                enemy5.jumpStrikeTargetPos;
        }

        phase = Phase.Aim;
        timer = enemy5.jumpAimTime;

        enemy5.jumpAimTarget.gameObject.SetActive(true);

        enemy.anim.SetInteger("skillState", 2);

        enemy.frameEvent.HideSkeleton();//进入透明状态

        enemy.checkArea.SetHitColor();//设置视野范围透明
    }

    public void StartAttack(EnemyController enemy)
    {
        phase = Phase.Attack;

        //enemy.StopMove();

        enemy.anim.SetInteger("skillState", 4);

        enemy.checkArea.SetJumpStrikeSkillColor();//设置视野范围显示
    }

    public void EndSkill(EnemyController enemy)
    {
        if (enemy5 == null)
            enemy5 = enemy as Enemy_5;

        if (enemy5 == null)
            return;

        enemy5.isJumpStrikeUntargetable = false;//离开无敌状态



        if (enemy5.jumpAimTarget != null)
            enemy5.jumpAimTarget.gameObject.SetActive(false);

        if (enemy5.jumpStrikeCollider != null)
            enemy5.jumpStrikeCollider.SetActive(false);



        enemy.anim.SetInteger("skillState", 0);

        if (enemy.isDead)
            return;

        if (enemy.attackList.Count > 0)
            enemy5.TryEnterJumpStrikeOrAttack();//随机是否进入再度技能状态
        else
            enemy.TransitionToState(enemy.patrolState);
    }

    private void CancelSkill(EnemyController enemy)
    {
        if (enemy5 != null)
        {
            if (enemy5.jumpAimTarget != null)
                enemy5.jumpAimTarget.gameObject.SetActive(false);

            if (enemy5.jumpStrikeCollider != null)
                enemy5.jumpStrikeCollider.SetActive(false);


        }

        enemy5.isJumpStrikeUntargetable = false;//离开无敌状态


        enemy.anim.SetInteger("skillState", 0);
        enemy.TransitionToState(enemy.hitState);
    }

    public override void ExitState(EnemyController enemy)
    {
        //enemy.StopMove();

        if (enemy5 != null &&
            enemy5.jumpAimTarget != null)
        {
            enemy5.jumpAimTarget.gameObject.SetActive(false);
        }
    }
}