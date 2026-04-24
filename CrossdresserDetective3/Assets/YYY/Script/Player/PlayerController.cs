using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour, IDamageable
{
    [Header("基础属性")]

    public Rigidbody2D rb;
    public float speed;
    float walkSpeed => speed/2.5f;//拉姆达表达式会导致每次调用都执行
    float runSpeed;

    [Header("碰撞体与下蹲")]
    public CapsuleCollider2D coll;
    public bool isCrouch;
    Vector2 originalOffset;
    Vector2 originalSize;


    [Header("地面检测与跳跃")]
    public float jumpForce;
    public PhysicsCheck physicsCheck;

    //[Header("跳跃相关")]
    //public bool isJump;//是否位于跳跃中


    [Header("攻击")]
    public bool isAttack;

    [Header("物理材质")]
    public PhysicsMaterial2D normal;//在地面的材质防止滑动
    public PhysicsMaterial2D wall;//防止卡墙移动


    [Header("生命值")]
    public float health;
    public bool isDead = false;
    public PlayerAnimation playerAnimation;





    [Header("特效")]
    public GameObject jumpFX;
    public GameObject landFX;

    [Header("炸弹")]
    public GameObject bombPrefab;
    public float nextAttack = 0;//攻击冷却
    public float attackRate;//攻击频率

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();

        coll= GetComponent<CapsuleCollider2D>();

        originalOffset = coll.offset;
        originalSize = coll.size;




        GameManager.instance.IsPlayer(this);


        health = GameManager.instance.LoadHealth();
        UIManager.instance.UpdateHealth(health);
    }






    public void Update()
    {
        //playerAnimation.anim.SetBool("dead", isDead);
        //if (isDead){ return; }






        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();


    }//输入用Update（听）

    public void FixedUpdate()
    {
        if (isDead)
        { 
            rb.velocity = Vector2.zero;
            return;
        }//死亡后不能滑行



        //_Jump();

        if (!isHurt&&!isAttack) { Move(); }

        CheckState();//如果在地上就是有摩擦力，在空中就没有防止卡墙

    }//每帧执行动作用FixedUpdate（做）


    void Move() 
    {
        if (!isCrouch)
        {
            rb.velocity = new Vector2(inputDirection.x * speed, rb.velocity.y);
        }


        //翻转
        int faceDir = (int)transform.localScale.x;
        if (inputDirection.x > 0)
        {
            faceDir = 1;
            //transform.eulerAngles = new Vector3(0, 0, 0);
        }
        if (inputDirection.x < 0)
        {
            faceDir = -1;
            //transform.eulerAngles = new Vector3(0, 180, 0);
        }
        transform.localScale = new Vector3(faceDir, 1, 1);

        //下蹲
        isCrouch = inputDirection.y < -0.5f && physicsCheck.isGround;

        if (isCrouch)
        {
            //降低碰撞体高度
            originalOffset = new Vector2(0f, -0.1f);
            coll.size = new Vector2(0.8f, 1.5f);

        }
        else
        {
            //还原原先状态
            coll.offset = originalOffset;
            coll.size = originalSize;
        }

    }





    public void LandFX()//动画帧时间触发
    {
        landFX.SetActive(true);
        landFX.transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }




    //public void Attack()
    //{
    //    if (Time.time > nextAttack)
    //    {
    //        Instantiate(bombPrefab, transform.position, bombPrefab.transform.rotation);
    //
    //        nextAttack = Time.time + attackRate;
    //    }
    //}






    public void GetHit(float damage)
    {
        if (!playerAnimation.anim.GetCurrentAnimatorStateInfo(1).IsName("player_hit"))
        {
            health -= damage;

            if (health < 1)
            {
                health = 0;
                isDead = true;
            }
            playerAnimation.anim.SetTrigger("hit");

            UIManager.instance.UpdateHealth(health);

        }//在Hit动画状态中，不会受伤


    
    }

    [Header("受伤反弹死亡")]
    public bool isHurt = false;
    public float hurtForce;

    public void GetHurt(Transform attacker)
    {
        isHurt = true;//主要用于屏蔽输入

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
        rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }
    public void PlayerDead() 
    {
        isDead = true;
        inputControl.Gameplay.Disable();//通过直接禁用来做（但是防止4层多端输入，在上方也禁止）
    }

    public void CheckState() 
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;//简写如果在地面就使用有摩擦力的这一版，没有就不是
    }



    /// <summary>
    /// 多端输入
    /// </summary>
    #region
    public PlayerInputControl inputControl;
    public Vector2 inputDirection;
    private void Awake()
    {
        inputControl = new PlayerInputControl();

        inputControl.Gameplay.Jump.started += Jump;

        #region 强制走路
        runSpeed = speed;//最开始把跑步速度设置为速度
        inputControl.Gameplay.WalkButton.performed += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = walkSpeed;
            }//在地面的时候才能切换走或跑
        };//检测按住
        inputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround)
            {
                speed = runSpeed;
            }//在地面的时候才能切换走或跑
        };//检测松开
        #endregion

        inputControl.Gameplay.Attack.started += PlayerAttack;
    }

    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }


    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround) 
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            jumpFX.SetActive(true);
            jumpFX.transform.position = transform.position + new Vector3(0, -0.45f, 0);
        }
        
    }

    void PlayerAttack(InputAction.CallbackContext obj) 
    {
        playerAnimation.PlayAttack();
        isAttack = true;

    }
   


    #endregion
}
