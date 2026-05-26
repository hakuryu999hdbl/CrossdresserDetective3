using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableWeapon : MonoBehaviour
{
    public enum ThrowableState
    {
        Flying,
        Dropped
    }

    [Header("状态")]
    public ThrowableState state = ThrowableState.Flying;

    [Header("组件")]
    public Rigidbody2D rb;
    public GameObject AttackArea;
    public int WeaponType;//1匕首 2武士刀 3尼泊尔军刀

    [Header("落地设置")]
    public LayerMask groundLayer;
    public float droppedGravityScale = 1f;

    private bool hasLanded;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
       
    }

    private void Update()
    {
        if (state == ThrowableState.Flying)
        {
            transform.Rotate(0f, 0f, -720 * Time.deltaTime);
        }
    }

    public void Init()
    {          
        SetFlyingState();

    }//玩家/敌人扔出武器后的传入数据

    private void SetFlyingState()
    {
        state = ThrowableState.Flying;
        hasLanded = false;



        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 1f;
        }
    }//飞行状态

    private void SetDroppedState()
    {
        if (hasLanded) return;
        hasLanded = true;

        state = ThrowableState.Dropped;

        AttackArea.SetActive(false);

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = droppedGravityScale;
        }

        //防止变成敌人目标，落地后改tag（改了之后敌人没法捡起了）
        gameObject.layer = LayerMask.NameToLayer("Environment");

        // 落地后摆正一点，不想摆正可以删掉
        //transform.rotation = Quaternion.identity;
    }//落地状态


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (state != ThrowableState.Dropped) return;

        PlayerController player = collision.GetComponentInParent<PlayerController>();

        if (player != null)
        {

            if (player.weaponType == 0)
            {
                if (player.isCrouch || player.isSlide)
                {
                    player.SetWeapon(WeaponType);

                    player.frameEvent_Audio._Attack_bomb_bounce();

                    Destroy(gameObject);
                    return;
                }
            }//玩家没有武器的情况下捡起武器

        }

       //EnemyController enemy = collision.GetComponentInParent<EnemyController>();
       //
       //if (enemy != null)
       //{
       //    Debug.Log("敌人捡起");
       //    if (enemy.weaponType == 0)
       //    {
       //        enemy.SetWeapon(WeaponType);
       //
       //        enemy.frameEvent_Audio._Attack_bomb_bounce();
       //
       //        Destroy(gameObject);
       //        return;
       //    }
       //}//敌人没有武器的情况下捡起武器


    }//拾取范围

    [Header("碰撞地板墙壁弹跳发出声音")]
    public FrameEvent_Audio frameEvent_Audio;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (state != ThrowableState.Flying) return;

        if (((1 << collision.gameObject.layer) & groundLayer) != 0)
        {
            SetDroppedState();
            frameEvent_Audio._Attack_sword_clash();
        }

    }//物理碰撞/阻挡/弹开

}
