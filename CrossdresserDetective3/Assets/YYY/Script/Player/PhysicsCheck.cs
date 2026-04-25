using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{

    [Header("地面检测")]
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    public bool isGround;//是否在地面
    public bool touchLeftWall;//左侧是否为空
    public bool touchRightWall;//右侧是否为空
    public bool onWall;//是否贴在墙上




    public float checkRadius;//检测半径
    public LayerMask groundLayer;//对象图层

    [Header("最左最右由碰撞体调整")]
    public bool manual;//手动调整最左最右
    public bool isPlayer;//手动调整是不是玩家身上的
    CapsuleCollider2D coll;//所有移动物体的胶囊碰撞体
    PlayerController playerController;
    Rigidbody2D rb;
    public void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
        }

        if (isPlayer) 
        {
            playerController = GetComponent<PlayerController>();
        }
    }


    private void Update()
    {
        Checked();
    }

    void Checked() 
    {
        //地面检测
        isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRadius, groundLayer);
        // isGround = Physics2D.OverlapCircle((Vector2)transform.position + new Vector2(bottomOffset.x*transform.localScale.x,bottomOffset.y), checkRadius, groundLayer);


        //if (isGround) { isJump = false; }//跳跃结束

        //墙体检测
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);

        //在墙上(下落状态，但是碰到了左墙和右墙的同时方向键也按住)
        if (isPlayer)
        {
            onWall = (touchLeftWall&&playerController.inputDirection.x<0f || touchRightWall && playerController.inputDirection.x > 0f) && rb.velocity.y<0;
        }
        
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);

    }//显示检测圆环(不需要调用)




  
  




}
