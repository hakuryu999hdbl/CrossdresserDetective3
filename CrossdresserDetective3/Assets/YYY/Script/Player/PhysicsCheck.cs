using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{

    [Header("地面检测")]
    public Vector2 bottomOffset;
    public Vector2 leftOffset;
    public Vector2 rightOffset;

    public bool touchLeftWall;//左侧是否为空
    public bool touchRightWall;//右侧是否为空

    public float checkRadius;//检测半径
    public LayerMask groundLayer;//对象图层
    public bool isGround;//是否在地面

    [Header("最左最右由碰撞体调整")]
    public bool manual;//手动调整最左最右
    CapsuleCollider2D coll;//所有移动物体的胶囊碰撞体

    public void Awake()
    {
        coll = GetComponent<CapsuleCollider2D>();

        if (!manual)
        {
            rightOffset = new Vector2((coll.bounds.size.x + coll.offset.x) / 2, coll.bounds.size.y / 2);
            leftOffset = new Vector2(-rightOffset.x, rightOffset.y);
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
        //if (isGround) { isJump = false; }//跳跃结束

        //墙体检测
        touchLeftWall = Physics2D.OverlapCircle((Vector2)transform.position + leftOffset, checkRadius, groundLayer);
        touchRightWall = Physics2D.OverlapCircle((Vector2)transform.position + rightOffset, checkRadius, groundLayer);
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, checkRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, checkRadius);

    }//显示检测圆环(不需要调用)




  
  




}
