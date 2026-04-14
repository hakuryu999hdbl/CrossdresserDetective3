using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator anim;
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("speed", Mathf.Abs(playerController.rb.velocity.x));//往←走变成负数，需要绝对值
        anim.SetFloat("velocityY", playerController.rb.velocity.y);//每帧检测是否下落
        anim.SetBool("jump", playerController.isJump);//每帧检测是否跳跃中
        anim.SetBool("ground", playerController.isGround);//每帧检测是位于地面
    }

    public void LandFX() 
    {
        //跳落第一帧触发
        playerController.LandFX();
    }
}
