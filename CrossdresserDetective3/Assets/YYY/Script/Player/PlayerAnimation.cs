using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("联系玩家脚本")]
    public Animator anim;
    public PlayerController playerController;
    public Animator anim_UI;
    [Header("地面检测")]
    public PhysicsCheck physicsCheck;

    // Start is called before the first frame update
    void Start()
    {
        //anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {


        anim.SetFloat("velocityX", Mathf.Abs(playerController.rb.velocity.x));//每帧检测横向速度绝对值
        anim.SetFloat("velocityY", playerController.rb.velocity.y);//每帧检测是否位于空中下落
        anim.SetBool("isGround", physicsCheck.isGround || playerController.isOnElevator);//每帧检测是位于地面
        anim.SetBool("isCrouch", playerController.isCrouch);//每帧检测是否下蹲
        anim.SetBool("isDead", playerController.isDead);//每帧检测是活着
        anim.SetBool("isAttack", playerController.isAttack);//每帧检测玩家是否处于攻击
        anim.SetBool("onWall", physicsCheck.onWall);//每帧检测玩家是否处于贴墙
        anim.SetBool("isSlide", playerController.isSlide);//每帧检测玩家是滑铲
        anim.SetInteger("attackType", playerController.attackType);//每帧检测玩家武器
        anim_UI.SetInteger("attackType", playerController.attackType);

        anim.SetBool("isWallCling", playerController.isWallCling);//每帧检测玩家是滑铲


    }

    public void PlayHurt() 
    {
        anim.SetInteger("hurtType", Random.Range(1,3));
        anim.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
   
        anim.SetTrigger("attack");
    }
    public void PlayReload()
    {


        anim.SetTrigger("reload");
    }

    public void PlayThrow()
    {

        anim.SetTrigger("throw");
    }

    public void PlayAirKick()
    {
        anim.SetTrigger("airKick");
    }

    public void EnterCutsceneIdle()
    {
        // 清理移动参数
        anim.SetFloat("velocityX", 0f);
        anim.SetFloat("velocityY", 0f);

        // 清理状态参数
        anim.SetBool("isCrouch", false);
        anim.SetBool("isGround", true);

        // 根据你动画器里的实际参数补充
        anim.SetBool("isAttack", false);

        // 强制进入Idle
        anim.Play("Idle", 0, 0f);
        anim.Update(0f);
    }

    public void PlayWashing() 
    {
        //Debug.Log("立刻播放洗澡动画");

        anim.Play("Story_Washing", 0, 0f);
        anim.Update(0f);
    }
    public void PlayUndressing()
    {
        //Debug.Log("立刻播放脱衣动画");

        anim.Play("Story_Undressing", 0, 0f);
        anim.Update(0f);
    }

    public void PlayWalking()
    {
        //Debug.Log("立刻播放走路动画");

        anim.Play("Story_Walk", 0, 0f);
        anim.Update(0f);
    }
}
