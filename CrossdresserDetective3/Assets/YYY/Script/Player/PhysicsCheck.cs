using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsCheck : MonoBehaviour
{

    [Header("地面检测")]
    public Transform groundCheck;//检测中心
    public float checkRadius;//检测半径
    public LayerMask groundLayer;//对象图层
    public bool isGround;//是否在地面

    private void Update()
    {
        Checked();
    }

    void Checked() 
    {
        isGround = Physics2D.OverlapCircle(groundCheck.transform.position, checkRadius, groundLayer);

        //if (isGround) { isJump = false; }//跳跃结束
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }//显示检测圆环(不需要调用)
}
