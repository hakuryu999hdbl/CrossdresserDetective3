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
        //anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

        //anim.SetBool("jump", playerController.isJump);//每帧检测是否跳跃中


        anim.SetFloat("velocityX", Mathf.Abs(playerController.rb.velocity.x));//每帧检测横向速度绝对值
        anim.SetFloat("velocityY", playerController.rb.velocity.y);//每帧检测是否位于空中下落
        anim.SetBool("isGround", physicsCheck.isGround);//每帧检测是位于地面
        anim.SetBool("isCrouch", playerController.isCrouch);//每帧检测是否下蹲
        anim.SetBool("isDead", playerController.isDead);//每帧检测是活着
        anim.SetBool("isAttack", playerController.isAttack);//每帧检测玩家是否处于攻击
        anim.SetBool("onWall", physicsCheck.onWall);//每帧检测玩家是否处于贴墙
        anim.SetBool("isSlide", playerController.isSlide);//每帧检测玩家是滑铲

       
    }

    public void PlayHurt() 
    {
        anim.SetTrigger("hurt");
    }

    public void PlayAttack()
    {
        anim.SetTrigger("attack");
    }

    public void LandFX() 
    {
        //跳落第一帧触发
        playerController.LandFX();
    }


    /// <summary>
    /// 帧事件调用
    /// </summary>
    public GameObject AttackArea_1, AttackArea_2, AttackArea_3;
    public void Attack_1() 
    {
        StartCoroutine(AttackRoutine(AttackArea_1));
    }
    public void Attack_2()
    {
        StartCoroutine(AttackRoutine(AttackArea_2));
    }
    public void Attack_3()
    {
        StartCoroutine(AttackRoutine(AttackArea_3));
    }

    IEnumerator AttackRoutine(GameObject area)
    {
        if (area == null) yield break;

        area.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        area.SetActive(false);
    }
}
