using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
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
        if (playerController.isInCutscene)
            return;//只要位于过场动画中，关闭自由触发


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

        anim.SetBool("isBoki", playerController.isBoki);//每帧检测玩家是勃起
    }

    public void PlayHurt()
    {
        anim.SetInteger("hurtType", Random.Range(1, 3));
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

        // 清除可能残留的动作
        anim.ResetTrigger("attack");
        anim.ResetTrigger("hurt");
        anim.ResetTrigger("reload");
        anim.ResetTrigger("throw");
        anim.ResetTrigger("airKick");


        // 清理移动参数
        anim.SetFloat("velocityX", 0f);
        anim.SetFloat("velocityY", 0f);

        // 清理状态参数
        anim.SetBool("isCrouch", false);
        anim.SetBool("isGround", true);

        // 强制伪装成地面静止状态
        anim.SetBool("isAttack", false);
        anim.SetBool("isSlide", false);
        anim.SetBool("onWall", false);
        anim.SetBool("isWallCling", false);

        // 强制进入Idle
        anim.Play("Idle", 0, 0f);
        anim.Update(0f);
    }


    public void ExitCutsceneIdle()
    {

        Invoke(nameof(ForceIdle), 0.1f);
    }

    void ForceIdle()
    {
        Debug.Log("强制进入Idle");

        // 强制进入Idle
        anim.Play("Idle", 0, 0f);
        anim.Update(0f);
    }




    public void EnterBondageIdle()
    {
        anim.ResetTrigger("attack");
        anim.ResetTrigger("reload");
        anim.ResetTrigger("throw");
        anim.ResetTrigger("airKick");
        anim.ResetTrigger("hurt");

        anim.SetBool("isAttack", false);
        anim.SetBool("isCrouch", false);
        anim.SetFloat("velocityX", 0f);
        anim.SetFloat("velocityY", 0f);

        anim.Play("Idle_Bondage", 0, 0f);
        anim.Update(0f);
    }

    public void ExitBondageIdle()
    {
        anim.SetBool("isCrouch", false);
        anim.SetFloat("velocityX", 0f);

        anim.Play("Idle", 0, 0f);
        anim.Update(0f);
    }




    public void PlayWashing()
    {
        anim.Play("Story_Washing", 0, 0f);
        anim.Update(0f);
    }
    public void PlayUndressing()
    {
        anim.Play("Story_Undressing", 0, 0f);
        anim.Update(0f);
    }

    public void PlaySelfBondage()
    {
        anim.Play("Story_SelfBondage_1", 0, 0f);
        anim.Update(0f);
    }


    public void PlayWalking()
    {
        anim.Play("Story_Walk", 0, 0f);
        anim.Update(0f);
    }

    public void PlayRuning()
    {
        anim.Play("Story_Run", 0, 0f);
        anim.Update(0f);
    }

    public void PlayIdle()
    {
        anim.Play("Story_Idle", 0, 0f);
        anim.Update(0f);
    }

    public void PlayCrouch()
    {
        anim.Play("Story_Crouch_Start", 0, 0f);
        anim.Update(0f);
    }


    public void SetPlayer_Turn()
    {
        transform.localScale = new Vector3(
             -transform.localScale.x,
             transform.localScale.y,
             transform.localScale.z
         );
    }

    public void SetPlayer_Next()
    {
        anim.SetTrigger("Next");
    }



    public void PlayRapeYYY() 
    {

        playerController.frameEvent.SetPlayer_Clothes_01();

        anim.Play("Story_RapeYYY", 0, 0f);
        anim.Update(0f);
    }//战败调用
}

