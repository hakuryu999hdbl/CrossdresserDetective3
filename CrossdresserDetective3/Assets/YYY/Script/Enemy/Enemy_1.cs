using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : EnemyController
{
    public override void EnterBattleState()
    {
        // 太近时不要冲撞，直接普通近战
        //if (targetPoint != null)
        //{
        //    float distance = Vector2.Distance(transform.position, targetPoint.position);
        //
        //    if (useChargeSkill &&
        //        distance >= chargeMinDistance &&
        //        Time.time >= lastChargeTime + chargeCooldown &&
        //        Random.value < 0.5f)
        //    {
        //        TransitionToState(chargeSkillState);
        //        return;
        //    }
        //}
        //
        //TransitionToState(attackState);



        //点位赋值
        if (attackList.Count > 0)
        {
            targetPoint = attackList[0];
        }

        if (targetPoint == null)
        {
            TransitionToState(patrolState);
            return;
        }


        if (useChargeSkill)
        {
            TransitionToState(chargeSkillState);
        }
        else 
        {
            TransitionToState(attackState);
        }

        
    }



    public void SetEnemy_Clothes_01()
    {
        frameEvent.Story_Clothes_Man_01();
    }//设置男性小偷服装

    public void SetEnemy_Walk()
    {
        anim.Play("Story_Walk", 0, 0f);
        anim.Update(0f);
    }

    public void SetEnemy_Idle()
    {
        anim.Play("Story_Idle", 0, 0f);
        anim.Update(0f);
    }

    public void SetEnemy_Turn()
    {
        transform.localScale = new Vector3(
             -transform.localScale.x,
             transform.localScale.y,
             transform.localScale.z
         );
    }

    public void SetEnemy_TurnCrouch()
    {
        anim.Play("Story_TurnCrouch", 0, 0f);
        anim.Update(0f);
    }

    public void SetEnemy_CatchYYY()
    {
        anim.Play("Story_CatchYYY", 0, 0f);
        anim.Update(0f);

        anim.gameObject.GetComponent<FrameEvent_Audio>()._YYY_duzui();//暂时先用这播放
    }

    public void SetEnemy_RapeYYY()
    {
        anim.Play("Story_RapeYYY_1", 0, 0f);
        anim.Update(0f);
    }

    public void SetEnemy_Next()
    {
        anim.SetTrigger("next");

    }
}
