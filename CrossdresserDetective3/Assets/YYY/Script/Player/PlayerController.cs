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
    float walkSpeed => speed / 2.5f;//拉姆达表达式会导致每次调用都执行
    float runSpeed;


    [Header("碰撞体与下蹲")]
    public CapsuleCollider2D coll;

    Vector2 originalOffset;
    Vector2 originalSize;
    [SerializeField] private Vector2 crouchSize = new Vector2(0.06f, 1.77f);

    [Header("地面检测与跳跃滑铲")]
    public float jumpForce;
    public PhysicsCheck physicsCheck;
    public float wallJumpForce;

    public float slideDistance;//滑铲距离
    public float slideSpeed;//滑铲速度

    [Header("挂墙")]
    public int wallPowerCost = 1;
    public float wallPowerCostInterval = 0.4f;
    public float wallPowerTimer;
    public float wallSlideSpeed = -1.5f;




    [Header("物理材质")]
    public PhysicsMaterial2D normal;//在地面的材质防止滑动
    public PhysicsMaterial2D wall;//防止卡墙移动
    public PhysicsMaterial2D wall_Stop;//在墙上停住

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
    public bool isWallCling;   // 是否抓/蹬在墙上停住



    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<Character>();
        rb = GetComponent<Rigidbody2D>();
        physicsCheck = GetComponent<PhysicsCheck>();

        coll = GetComponent<CapsuleCollider2D>();

        originalOffset = coll.offset;
        originalSize = coll.size;




        GameManager.instance.IsPlayer(this);


        RefreshPlayerSkin();//初始更新皮肤
    }






    public void Update()
    {

        inputDirection = inputControl.Gameplay.Move.ReadValue<Vector2>();


    }//输入用Update（听）

    public void FixedUpdate()
    {
        if (isDead)
        {
            if (physicsCheck.isGround)
            {
                rb.velocity = Vector2.zero;//死亡后不能滑行
            }
            else
            {
                rb.velocity = new Vector2(0f, rb.velocity.y);//死亡后在空中的话落地
            }
            return;
        }

        if (!isHurt && !isAttack && !isTeleporting) { Move(); }

        CheckState();//如果在地上就是有摩擦力，在空中就没有防止卡墙

    }//每帧执行动作用FixedUpdate（做）


    void Move()
    {
        if (!isCrouch && !wallJump)
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
            //originalOffset = new Vector2(0f, 0.1f);
            //coll.size = new Vector2(0.8f, 2.1f);

            float heightDiff = originalSize.y - crouchSize.y;

            coll.size = crouchSize;
            coll.offset = new Vector2(
                originalOffset.x,
                originalOffset.y - heightDiff / 2f
            );

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
        // 默认材质
        if (physicsCheck.isGround)
        {
            isWallCling = false;
            coll.sharedMaterial = normal;

            character.StopPowerRecover = false;//体力恢复   
        }
        else if (physicsCheck.onWall && character.currentPower > 0)
        {
            // 在墙上，并且还有体力：停住
            isWallCling = true;
            coll.sharedMaterial = wall_Stop;

            //rb.velocity = new Vector2(0, 0);
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);//贴在墙上下落速度减慢


            //为了一格一格扣体力
            wallPowerTimer += Time.fixedDeltaTime;
            if (wallPowerTimer >= wallPowerCostInterval)
            {
                wallPowerTimer = 0;


                character.OnSlide(wallPowerCost);
                character.StopPowerRecover = true;//墙上扣体力不恢复     
            }


        }
        else if (physicsCheck.onWall && character.currentPower <= 0)
        {
            // 体力耗尽：缓慢滑落
            isWallCling = false;
            coll.sharedMaterial = wall;

            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);//贴在墙上下落速度减慢
            //rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, wallSlideSpeed));

            character.StopPowerRecover = true;//墙上扣体力不恢复   
        }
        else
        {
            // 普通空中
            isWallCling = false;
            coll.sharedMaterial = wall;
            wallPowerTimer = 0;
        }







        //coll.sharedMaterial = physicsCheck.isGround ? normal : wall;//简写如果在地面就使用有摩擦力的这一版，没有就不是
        //
        //if (physicsCheck.onWall)
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y / 2);//贴在墙上下落速度减慢
        //
        //
        //}
        //else
        //{
        //    rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y);
        //
        //
        //}



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
    /// Spine外观
    /// </summary>
    #region
    [Header("Spine外观")]
    public FrameEvent frameEvent;
    public FrameEvent frameEvent_UI;//大的放大层也需要
    int beltIndex = 1;
    int clothesIndex = 1;
    int glovesIndex = 1;
    int pantiesIndex = 1;
    int shoesIndex = 1;
    int skirtIndex = 1;
    int stockingsIndex = 1;

    [Header("武器与攻击方式")]
    public int weaponType;//0空手 1匕首 2武士刀 3尼泊尔军刀
    public int attackType;//0踢击 1挥砍



    public void RefreshPlayerSkin()
    {
        if (frameEvent == null) return;

        frameEvent.ShowCurrentAll(
            beltIndex,
            clothesIndex,
            glovesIndex,
            pantiesIndex,
            shoesIndex,
            skirtIndex,
            stockingsIndex,
            weaponType
        );

        frameEvent_UI.ShowCurrentAll(
          beltIndex,
          clothesIndex,
          glovesIndex,
          pantiesIndex,
          shoesIndex,
          skirtIndex,
          stockingsIndex,
          weaponType
      );


    }//更新外观

    public void SetWeapon(int newWeaponType)
    {
        weaponType = newWeaponType;

        if (weaponType == 0)
            attackType = 0;//赤手空拳
        else
            attackType = 1;



        RefreshPlayerSkin();
    }//捡起的武器调用这里


    #endregion


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

        PlayBloodEffect();

        isDead = true;
        inputControl.Gameplay.Disable();//通过直接禁用来做（但是防止4层多端输入，在上方也禁止）


        UIManager.instance.GameOverUI();
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


        inputControl.Gameplay.Slide.started += Slide;

        inputControl.Gameplay.Throw.started += Throw;


        inputControl.Gameplay.Pause.started += OnPause;

        inputControl.Gameplay.ZoomCamera.started += OnZoomCamera;

   

        //UI等所有多端输入由PlayerController管理
        //inputControl.UI.Cancel.started += OnCancel;
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
        inputControl.UI.Disable();
        Sign.SetActive(true);
    }
    public void DisableGameplayInput()
    {
        inputControl.Gameplay.Disable();
        inputControl.UI.Enable();
        Sign.SetActive(false);
    }


    private void Jump(InputAction.CallbackContext obj)
    {
        if (isAttack) { return; }//如果攻击已经开始不能跳跃


        if (physicsCheck.isGround)
        {
            rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);

            //StopAllCoroutines();//一旦跳跃打断所有协程（滑铲）
            //isSlide = false;

            if (slideCoroutine != null)
            {
                StopCoroutine(slideCoroutine);
                EndSlide();
            }

            //跳跃特效
            jumpFX.SetActive(true);
            jumpFX.transform.position = transform.position + new Vector3(0, -1f, 0);
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
    float throwForce = 16f;//投掷炸弹力度



   
    public float nextAttack = 0;//炸弹攻击冷却
    public float attackRate;//炸弹攻击频率
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
            //Instantiate(bombPrefab, transform.position, bombPrefab.transform.rotation);

            //将炸弹扔出去
            GameObject bomb =
           Instantiate(
               bombPrefab,
               transform.position,
               Quaternion.identity
           );

            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 dir =
                    new Vector2(transform.localScale.x * 0.6f, 1f).normalized;

                rb.velocity = dir * throwForce;
            }




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


    #region  冲刺
    [Header("冲刺")]
    public bool isDashAttack;//这个暂时没有产生作用，但是留着看看
    public float dashAttackSpeed = 16f;
    public float dashAttackDuration = 0.2f;

    public void Dash() 
    {
        StartCoroutine(DashAttack());
    }

    IEnumerator DashAttack()
    {
        isAttack = true;
        isDashAttack = true;



        float timer = 0f;
        float dir = transform.localScale.x;

        while (timer < dashAttackDuration)
        {
            yield return new WaitForFixedUpdate();

            // 撞墙停止
            if ((physicsCheck.touchLeftWall && dir < 0f) ||
                (physicsCheck.touchRightWall && dir > 0f))
                break;

            rb.MovePosition(
                rb.position +
                new Vector2(dir * dashAttackSpeed * Time.fixedDeltaTime, 0f)
            );

            timer += Time.fixedDeltaTime;
        }

        isDashAttack = false;

    }
    #endregion


    #region  翻滚
    [Header("滑铲的体力消耗")]
    public float slideDuration = 0.35f;
    private Coroutine slideCoroutine;

    public int slidePowerCost;

    private void Slide(InputAction.CallbackContext obj)
    {
      
        if (isSlide) return;
        if (!physicsCheck.isGround) return;
        if (character.currentPower < slidePowerCost) return;
        if (isAttack || isHurt || isDead || isTeleporting) return;

        character.OnSlide(slidePowerCost);

        slideCoroutine = StartCoroutine(TriggerSlide());

    }
    IEnumerator TriggerSlide()
    {
        float timer = 0f;
        float dir = transform.localScale.x;

        isSlide = true;
        gameObject.layer = LayerMask.NameToLayer("NPC");

        while (timer < slideDuration)
        {
            yield return new WaitForFixedUpdate();

            if (!physicsCheck.isGround)
                break;

            if ((physicsCheck.touchLeftWall && dir < 0f) ||
                (physicsCheck.touchRightWall && dir > 0f))
                break;

            rb.MovePosition(rb.position + new Vector2(dir * slideSpeed * Time.fixedDeltaTime, 0f));

            timer += Time.fixedDeltaTime;
        }

        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
    }
    void EndSlide()
    {
        isSlide = false;
        gameObject.layer = LayerMask.NameToLayer("Player");
        slideCoroutine = null;
    }

    #endregion


    #region  投掷
    [Header("投掷")]
    public GameObject throwableWeaponPrefab;
    public GameObject Weapon_Melee_01;
    public GameObject Weapon_Melee_02;
    public GameObject Weapon_Melee_03;
    public void Throw(InputAction.CallbackContext obj) 
    {
        if (!physicsCheck.isGround) { return; }//空中无法投掷

        playerAnimation.PlayThrow();
        isAttack = true;
    }


    float nextThrow = 0;//投掷冷却
    float ThrowRate = 0.5f;//投掷频率
    public void ThrowWeapon()
    {
        if (weaponType == 0) return; // 空手不能扔


        switch (weaponType) 
        {
            case 1:
                throwableWeaponPrefab = Weapon_Melee_01;
                break;
            case 2:
                throwableWeaponPrefab = Weapon_Melee_02;
                break;
            case 3:
                throwableWeaponPrefab = Weapon_Melee_03;
                break;
        }




        if (Time.time > nextThrow)
        {
            GameObject obj = Instantiate(
                throwableWeaponPrefab,
                transform.position,
                Quaternion.identity
            );

            ThrowableWeapon throwable = obj.GetComponent<ThrowableWeapon>();

            if (throwable != null)
            {
                throwable.Init();
            }

            Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                Vector2 dir = new Vector2(transform.localScale.x * 0.4f, 0.2f).normalized;
                rb.velocity = dir * throwForce;
            }

            // 扔出去后变空手
            weaponType = 0;
            attackType = 0;

            RefreshPlayerSkin();

            nextThrow = Time.time + ThrowRate;
        }
    }


    #endregion


    #region  UI层
    [Header("打开暂停菜单隐藏交互碰撞体")]
    public GameObject Sign;
    private void OnPause(InputAction.CallbackContext ctx)
    {
        UIManager.instance.TogglePause();
    }
    //public void OnCancel(InputAction.CallbackContext ctx)
    //{
    //    if (UIManager.instance.isPaused)
    //    {
    //        UIManager.instance.ClosePause();
    //    }
    //
    //}
    #endregion


    #region 放大缩小镜头
    [Header("放大缩小镜头")]
    public CameraControl cameraControl;

    private void OnZoomCamera(InputAction.CallbackContext ctx)
    {

        cameraControl.ToggleZoom();

      
    }
    #endregion



    #endregion
}
