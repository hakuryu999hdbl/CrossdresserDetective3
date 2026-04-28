using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Captain : EnemyController
{



    SpriteRenderer sprite;

    public override void Init()
    {
        base.Init();//在原有的基础之上

        sprite = transform.GetChild(1).GetComponentInChildren<SpriteRenderer>();
    }

    public override void Update() 
    {
        base.Update();

        if(animState == 0)
        {
            sprite.flipX = false;
        }//只要是处于巡逻状态，这个翻转就恢复

    }

    public override void SkillAction() 
    {
        base.SkillAction();//在原有的基础之上

        if (anim.GetCurrentAnimatorStateInfo(1).IsName("Captain_Skill"))//处于动画内会一直往方向跑
        {

            sprite.flipX = true;//转身逃跑

            if (transform.position.x > targetPoint.position.x)
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.right, speed * 4 * Time.deltaTime);//看见炸弹以2倍的速度往反方向跑
            }
            else 
            {
                transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.left, speed * 4 * Time.deltaTime);//看见炸弹以2倍的速度往反方向跑
            }
        }
        else
        {
            sprite.flipX = false;
        }

    }//子集覆盖原有

}
