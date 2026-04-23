using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("联系玩家脚本")]
    public Animator anim;
    public PlayerController playerController;
    [Header("地面检测")]
    public PhysicsCheck physicsCheck;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //anim.SetFloat("speed", Mathf.Abs(playerController.rb.velocity.x));//往←走变成负数，需要绝对值
      
        //anim.SetBool("jump", playerController.isJump);//每帧检测是否跳跃中


        anim.SetFloat("velocityX", Mathf.Abs(playerController.rb.velocity.x));//每帧检测横向速度绝对值
        anim.SetFloat("velocityY", playerController.rb.velocity.y);//每帧检测是否位于空中下落
        anim.SetBool("isGround", physicsCheck.isGround);//每帧检测是位于地面
        anim.SetBool("isCrouch", playerController.isCrouch);//每帧检测是否下蹲
        anim.SetBool("isDead", playerController.isDead);//每帧检测是活着

    }

    public void PlayHurt() 
    {
        anim.SetTrigger("hurt");
    }



    public void LandFX() 
    {
        //跳落第一帧触发
        playerController.LandFX();
    }
}
