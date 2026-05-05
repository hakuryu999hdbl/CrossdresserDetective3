using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("基础属性")]
    public Character character;
    public Rigidbody2D rb;
    public float speed;
    float walkSpeed => speed/2.5f;//拉姆达表达式会导致每次调用都执行
    float runSpeed;

    [Header("碰撞体与下蹲")]
    public CapsuleCollider2D coll;

    Vector2 originalOffset;
    Vector2 originalSize;


    [Header("地面检测与跳跃滑铲")]
    public float jumpForce;
    public PhysicsCheck physicsCheck;
    public float wallJumpForce;

    public float slideDistance;//滑铲距离
    public float slideSpeed;//滑铲速度

    //[Header("跳跃相关")]
    //public bool isJump;//是否位于跳跃中


   

    [Header("物理材质")]
    public PhysicsMaterial2D normal;//在地面的材质防止滑动
    public PhysicsMaterial2D wall;//防止卡墙移动


    [Header("死亡判定")]
    public PlayerAnimation playerAnimation;


    [Header("特效")]
    public GameObject jumpFX;
    public GameObject landFX;


    [Header("状态")]
    public bool isAttack;
    public bool isCrouch;
    public bool isHurt = false;
    public bool isDead = false;
    public bool wallJump;//蹬墙跳出期间X横向暂时不受方向键控制
    public bool isSlide;
    public bool isTeleporting = false;//是否传送




    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();

        coll= GetComponent<CapsuleCollider2D>();

        originalOffset = coll.offset;
        originalSize = coll.size;




        GameManager.instance.IsPlayer(this);

    }






    public void Update()
    {

        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();


    }//输入用Update（听）

    public void FixedUpdate()
    {
        if (isDead)
        { 
            rb.velocity = Vector2.zero;
            return;
        }//死亡后不能滑行

        if (!isHurt&&!isAttack&& !isTeleporting) { Move(); }

        CheckState();//如果在地上就是有摩擦力，在空中就没有防止卡墙

    }//每帧执行动作用FixedUpdate（做）


    void Move() 
    {
        if (!isCrouch&&!wallJump)
        {
            rb.velocity = new Vector2(inputDirection.x * speed, rb.velocity.y);
        }//下蹲和非蹬墙跳期间才可以获取左右


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
            //originalOffset = new Vector2(0f, 0.78f);
            //coll.size = new Vector2(0.8f, 1.9f);

        }
        else
        {
            //还原原先状态
            coll.offset = originalOffset;
            coll.size = originalSize;
        }

    }

    public void CheckState()
    {
        coll.sharedMaterial = physicsCheck.isGround ? normal : wall;//简写如果在地面就使用有摩擦力的这一版，没有就不是

        if (physicsCheck.onWall)
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);//贴在墙上下落速度减慢

        }
        else
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }

        if (wallJump && rb.velocity.y < 0f)
        {
            wallJump = false;
        }//蹬墙跳出去的时候，下落状态下可以左右移动
    }


    public void LandFX()//动画帧时间触发
    {
        landFX.SetActive(true);
        landFX.transform.position = transform.position + new Vector3(0, -0.75f, 0);
    }




    /// <summary>
    /// 受伤死亡
    /// </summary>
    #region
    [Header("受伤死亡")]
    public float hurtForce;
    public FrameEvent_Audio frameEvent_Audio;
    public GameObject Effect_Blood;

    #region  旧击退
    public void GetHurt(Transform attacker)
    {
       // isHurt = true;//主要用于屏蔽输入
       //
       // rb.velocity = Vector2.zero;
       // Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
       // rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    }
    #endregion

    public void OnTakeDamage(Attack attack)
    {

        if (attack == null)
            return;

        isHurt = true;//主要用于屏蔽输入

        playerAnimation.PlayHurt();


        if (attack.clearVelocity)
        {
            rb.velocity = Vector2.zero;
        }

        float dir = transform.position.x >= attack.transform.position.x ? 1f : -1f;

        rb.AddForce(
            new Vector2(dir * attack.knockbackX, attack.knockbackY),
            ForceMode2D.Impulse
        );

        PlayBloodEffect();
    }

    void PlayBloodEffect()
    {
        GameObject blood = Instantiate(
            Effect_Blood,
            transform.position,
            Quaternion.identity
        );

        frameEvent_Audio._Attack_blood();


        Destroy(blood, 1f); // 1秒后销毁
    }



    public void PlayerDead() 
    {
        isDead = true;
        inputControl.Gameplay.Disable();//通过直接禁用来做（但是防止4层多端输入，在上方也禁止）
    }


    #endregion




    /// <summary>
    /// 多端输入
    /// </summary>
    #region

    [Header("多端输入")]
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

        //inputControl.Gameplay.Attack.started += PlayerAttack;
        inputControl.Gameplay.Attack.started += OnAttackStarted;
        inputControl.Gameplay.Attack.canceled += OnAttackCanceled;


        inputControl.Gameplay.Slide.started +=Slide;


        inputControl.Gameplay.Pause.started += OnPause;

    }

  
    private void OnEnable()
    {
        inputControl.Enable();
    }
    private void OnDisable()
    {
        inputControl.Disable();
    }
    public void EnableGameplayInput()
    {
        inputControl.Gameplay.Enable();
    }
    public void DisableGameplayInput()
    {
        inputControl.Gameplay.Disable();
    }

   
    private void Jump(InputAction.CallbackContext obj)
    {
        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            StopAllCoroutines();//一旦跳跃打断所有协程（滑铲）
            isSlide = false;


            //跳跃特效
            jumpFX.SetActive(true);
            jumpFX.transform.position = transform.position + new Vector3(0, -0.45f, 0);
        }
        else if (physicsCheck.onWall) 
        {
            rb.AddForce(new Vector2(-inputDirection.x, 2f) * wallJumpForce, ForceMode2D.Impulse);//蹬墙跳，给与反方向的力
            wallJump = true;
        }
        
    }


    #region  攻击触发
    [Header("攻击触发/炸弹")]
    public GameObject bombPrefab;
    public float nextAttack = 0;//攻击冷却
    public float attackRate;//攻击频率
    private float attackPressTime;
    private float chargeThreshold = 0.35f;
    private void OnAttackStarted(InputAction.CallbackContext ctx)
    {
        attackPressTime = Time.time;
    }

    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
        float holdTime = Time.time - attackPressTime;

        if (holdTime >= chargeThreshold)
        {
            ThrowBomb();//蓄力攻击
        }
        else
        {
            PlayerAttack(ctx);//单按一下
        }
    }
    public void ThrowBomb()
    {
        if (Time.time > nextAttack)
        {
            Instantiate(bombPrefab, transform.position, bombPrefab.transform.rotation);

            nextAttack = Time.time + attackRate;
        }
    }
    void PlayerAttack(InputAction.CallbackContext obj) 
    {
    
        if (!physicsCheck.isGround) { return; }//空中无法攻击
    
        playerAnimation.PlayAttack();
        isAttack = true;
    
    }
    #endregion




    [Header("滑铲的体力消耗")]
    public int slidePowerCost;

    private void Slide(InputAction.CallbackContext obj)
    {
        if (!isSlide&&physicsCheck.isGround&&character.currentPower>=slidePowerCost)//在地上才能滑铲需要体力值
        {
            isSlide = true;//非滑铲的情况下才能滑铲
            var targetPos = new Vector3(transform.position.x + slideDistance * transform.localScale.x,transform.position.y);//获得滑铲目标点

            gameObject.layer = LayerMask.NameToLayer("NPC");//滑铲过程中保持无敌
            StartCoroutine(TriggerSlide(targetPos));


            //每次滑铲消耗体力
            character.OnSlide(slidePowerCost);
        }

       
    }

    IEnumerator TriggerSlide(Vector3 target) 
    {
        do
        {
            yield return null;

            if (!physicsCheck.isGround)
            {
                  break;//脱离地面停止
            }
              
            if(physicsCheck.touchLeftWall&&transform.localScale.x<0f|| physicsCheck.touchRightWall && transform.localScale.x > 0f)
            {
                isSlide = false;
                break;//撞墙停止
            }
            rb.MovePosition(new Vector2(transform.position.x + transform.localScale.x*slideSpeed,transform.position.y));

        }while(MathF.Abs(target.x - transform.position.x) > 0.1f);//直到到达目标点之前不停做

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");//滑铲结束
    }

    private void OnPause(InputAction.CallbackContext ctx)
    {
        UIManager.instance.TogglePause();
    }
    #endregion
}
