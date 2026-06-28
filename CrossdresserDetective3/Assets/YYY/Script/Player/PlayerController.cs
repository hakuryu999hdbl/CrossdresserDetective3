using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("基础属性")]
    public Character character;
    public Rigidbody2D rb;
    public float speed;

    //float walkSpeed => speed / 2.5f;//拉姆达表达式会导致每次调用都执行
    //float runSpeed;
    //据说手柄爬墙会产生半速这样问题
    public float runSpeed = 5f;
    public float walkSpeed = 2f;
    private bool isWalking;

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



        OnReloadAnimationEnd();//更新子弹

        frameEvent.FadeIn(0.4f);//所有Spine都淡入

        ReadCurrentGame();//初始读取

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
        CheckThrow();//蓄力投掷

    }//每帧执行动作用FixedUpdate（做）


    void Move()
    {
        float currentSpeed = isWalking ? walkSpeed : runSpeed;


        if (!isCrouch && !wallJump)
        {
            //rb.velocity = new Vector2(inputDirection.x * speed, rb.velocity.y);

            rb.velocity = new Vector2(inputDirection.x * currentSpeed, rb.velocity.y);

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
        //飞踢优先，避免进入爬墙
        if (isAirKick)
        {
            isWallCling = false;
            coll.sharedMaterial = wall;

            if ((physicsCheck.touchLeftWall && transform.localScale.x < 0f) ||
                (physicsCheck.touchRightWall && transform.localScale.x > 0f) ||
                physicsCheck.isGround)
            {
                isAirKick = false;
                isAttack = false;
            }

            return;
        }


        // 默认材质
        if (physicsCheck.isGround)
        {
            isWallCling = false;
            coll.sharedMaterial = normal;

            character.StopPowerRecover = false;//体力恢复   



            wallJump = false;//好像手柄会产生导致无法移动状态所以增加


            if (isAirKick)
            {
                isAirKick = false;
                isAttack = false;
            }//落地飞踢强制结束


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


        //if (physicsCheck.isGround && physicsCheck.isOnStair && !isDead)
        //{
        //
        //    if (Mathf.Abs(rb.velocity.x) < 0.3f)
        //    {
        //        rb.velocity = new Vector2(0f, rb.velocity.y);
        //    }
        //
        //}//玩家位于楼梯不会产生在地面较大滑动（但是开启会阻碍斜坡移动）


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
    public int beltIndex;
    public int hairIndex;
    public int clothesIndex;
    public int glovesIndex;
    public int pantiesIndex;
    public int shoesIndex;
    public int skirtIndex;
    public int stockingsIndex;

    public int Girl_hairIndex;
    public int Girl_clothesIndex;
    public int Girl_glovesIndex;
    public int Girl_underwearIndex;
    public int Girl_shoesIndex;
    public int Girl_stockingsIndex;
    public int Girl_hatIndex;
    public int Girl_maskIndex;


    [Header("武器与攻击方式")]
    public int meleeType;//0空手 1匕首 2武士刀 3尼泊尔军刀
    public int pistolType;//0空手 1柯尔特M1911 2沙鹰手枪 3格洛克手枪
    public int rifleType;//0空手 1步枪M4A1 2步枪AK47
    public int throwType;//0空手 1手榴弹 2烟雾弹 3闪光弹 4燃烧弹  5震撼弹  6飞刀
    public int attackType;//-2步枪射击  -1手枪射击 0踢击 1挥砍


    public void RefreshPlayerSkin()
    {

        frameEvent.ShowCurrentAll(
            beltIndex,
            hairIndex,
            clothesIndex,
            glovesIndex,
            pantiesIndex,
            shoesIndex,
            skirtIndex,
            stockingsIndex,

            Girl_hairIndex,
            Girl_clothesIndex,
            Girl_glovesIndex,
            Girl_underwearIndex,
            Girl_shoesIndex,
            Girl_stockingsIndex,
            Girl_hatIndex,
            Girl_maskIndex,

            meleeType,
            pistolType,
            rifleType,
            throwType
        );

        frameEvent_UI.ShowCurrentAll(
            beltIndex,
            hairIndex,
            clothesIndex,
            glovesIndex,
            pantiesIndex,
            shoesIndex,
            skirtIndex,
            stockingsIndex,

            Girl_hairIndex,
            Girl_clothesIndex,
            Girl_glovesIndex,
            Girl_underwearIndex,
            Girl_shoesIndex,
            Girl_stockingsIndex,
            Girl_hatIndex,
            Girl_maskIndex,

            meleeType,
            pistolType,
            rifleType,
            throwType
      );


    }//更新外观

    public Animator UI_anim;//每次换装提醒一下

    public void ChangeEquip(GameFlowData.EquipPart part, int index)
    {



        switch (part)
        {
            //case GameFlowData.EquipPart.Belt:
            //    beltIndex = index;
            //    break;

            case GameFlowData.EquipPart.Clothes:
                clothesIndex = index;

                if (clothesIndex == 3)
                    skirtIndex = 0;//兔女郎装不能穿裙子

                break;

            case GameFlowData.EquipPart.Gloves:
                glovesIndex = index;
                break;

            case GameFlowData.EquipPart.Panties:
                pantiesIndex = index;

                if (pantiesIndex != 3 && stockingsIndex == 3)
                {
                    stockingsIndex = 0;
                }//在裤袜的情况下，更换内裤会把丝袜换掉

                break;

            case GameFlowData.EquipPart.Shoes:
                shoesIndex = index;
                break;

            case GameFlowData.EquipPart.Skirt:
                skirtIndex = index;

                if (skirtIndex != 0 && clothesIndex == 3)
                    clothesIndex = 0;//裙子自动脱兔女郎装

                break;

            case GameFlowData.EquipPart.Stockings:
                stockingsIndex = index;


                if (stockingsIndex == 3)
                {
                    pantiesIndex = 3;

                }
                else
                {
                    pantiesIndex = 0;

                    if (clothesIndex == 3)
                        clothesIndex = 0;//兔女郎装的情况下也一并脱掉

                }//裤袜固定部件




                break;

            case GameFlowData.EquipPart.Melee:
                meleeType = index;

                if (index == 0)
                {
                    //为徒手

                    attackType = 0;
                    meleeSlot = 0;
                }
                if (index != 0)
                {
                    //为匕首攻击方式

                    attackType = 1;
                    meleeSlot = 1;
                }
                Slot = 0;//为近战



                break;

            case GameFlowData.EquipPart.Pistol:
                pistolType = index;

                if (index == 0)
                {
                    //为徒手

                    attackType = 0;
                    rangedSlot = 0;

                    Debug.Log("为徒手");
                }
                if (index != 0)
                {
                    //为手枪的攻击方式

                    attackType = -1;
                    rangedSlot = -1;
                }
                Slot = 1;//为远程

                break;

            case GameFlowData.EquipPart.Rifle:
                rifleType = index;

                if (index == 0)
                {
                    //为徒手

                    attackType = 0;
                    rangedSlot = 0;

                    Debug.Log("为徒手");
                }
                if (index != 0)
                {
                    //为步枪枪的攻击方式

                    attackType = -2;
                    rangedSlot = -2;
                }
                Slot = 1;//为远程


                break;

            case GameFlowData.EquipPart.Throw:
                throwType = index;

                if (index == 0)
                {
                    //为徒手

                    attackType = 0;

                    Debug.Log("为徒手");
                }
                break;
        }

        if (stockingsIndex == 1 && pantiesIndex != 0)
        {
            beltIndex = 1;
        }
        else
        {
            beltIndex = 0;

        } //吊带袜裤袜固定有内裤的情况下吊带出现


        RefreshPlayerSkin();//换装界面调用

        RefreshCurrentWeapon();//换装界面调用

        SaveCurrentGame();//换装界面记录

    }//换装口子

    public void SaveCurrentGame()
    {
        SaveData data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);


        data.clothesIndex = clothesIndex;
        data.glovesIndex = glovesIndex;
        data.pantiesIndex = pantiesIndex;
        data.shoesIndex = shoesIndex;
        data.skirtIndex = skirtIndex;
        data.stockingsIndex = stockingsIndex;

        data.meleeType = meleeType;
        data.pistolType = pistolType;
        data.rifleType = rifleType;
        data.throwType = throwType;


        data.meleeSlot = meleeSlot;
        data.rangedSlot = rangedSlot;
        data.Slot = Slot;

        SaveManager.SaveGame(data);
    }

    public void ReadCurrentGame()
    {
        SaveData data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);

        clothesIndex = data.clothesIndex;
        glovesIndex = data.glovesIndex;
        pantiesIndex = data.pantiesIndex;
        shoesIndex = data.shoesIndex;
        skirtIndex = data.skirtIndex;
        stockingsIndex = data.stockingsIndex;

        meleeType = data.meleeType;
        pistolType = data.pistolType;
        rifleType = data.rifleType;
        throwType = data.throwType;

        meleeSlot = data.meleeSlot;
        rangedSlot = data.rangedSlot;
        Slot = data.Slot;

        RefreshPlayerSkin();//初始更新皮肤

        RefreshCurrentWeapon();//初始更新武器动作和UI

    }


    #endregion


    /// <summary>
    /// 受伤死亡
    /// </summary>
    #region
    [Header("受伤死亡")]
    public float hurtForce;
    public GameObject RedScreen;
    public FrameEvent_Audio frameEvent_Audio;
    public GameObject Effect_Blood;//受伤特效
    public GameObject Strike_Effect;//剑光特效
    public GameObject Hit_Effect;//打击特效





    public void OnTakeDamage(Attack attack)
    {

        if (attack == null)
            return;


        PlayBloodEffect();

        switch (attack.hitEffectType)
        {
            case 0:
                // 打击特效
                Hit_Effect.SetActive(true);
                break;

            case 1:
                // 斩击特效
                Strike_Effect.transform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-45f, 45f));
                Strike_Effect.SetActive(true);
                break;
        }





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


    }

    void PlayBloodEffect()
    {
        GameObject blood = Instantiate(
            Effect_Blood,
            transform.position,
            Quaternion.identity
        );

        frameEvent_Audio._Attack_blood();

        RedScreen.SetActive(true);

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
                //speed = walkSpeed;
                isWalking = true;


            }//在地面的时候才能切换走或跑
        };//检测按住
        inputControl.Gameplay.WalkButton.canceled += ctx =>
        {
            if (physicsCheck.isGround)
            {
                //speed = runSpeed;
                isWalking = false;

            }//在地面的时候才能切换走或跑
        };//检测松开
        #endregion

        //inputControl.Gameplay.Attack.started += PlayerAttack;
        inputControl.Gameplay.Attack.started += OnAttackStarted;
        inputControl.Gameplay.Attack.canceled += OnAttackCanceled;


        inputControl.Gameplay.Slide.started += Slide;

        inputControl.Gameplay.Throw.started += OnThrowStart;
        inputControl.Gameplay.Throw.canceled += OnThrowCanceled;


        inputControl.Gameplay.Pause.started += OnPause;

        inputControl.Gameplay.ZoomCamera.started += OnZoomCamera;

        inputControl.Gameplay.Reload.started += OnReload;


        //UI等所有多端输入由PlayerController管理
        inputControl.UI.Cancel.started += OnCancel;
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


    #region  攻击
    [Header("攻击触发/炸弹")]


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
            //蓄力攻击

        }
        else
        {
            PlayerAttack(ctx);//单按一下
        }
    }

    void PlayerAttack(InputAction.CallbackContext obj)
    {

        //if (!physicsCheck.isGround) { return; }//空中无法攻击

        if (!physicsCheck.isGround)
        {
            AirKick();
            return;
        }//空中触发飞踢


        isAttack = true;


        if (attackType < 0 && currentAmmo <= 0)//如果是枪械类 ,子弹不够就会触发换单
        {
            frameEvent_Audio._Bullet_OutOfBullet();
            Reload();//子弹没了换
        }
        else
        {
            playerAnimation.PlayAttack();
        }


    }

    
    #endregion


    #region  飞踢
    [Header("飞踢")]
    public bool isAirKick;
    public float airKickSpeedX = 8f;
    public float airKickSpeedY = -6f;
    public float airKickDuration = 0.18f;
    private Coroutine airKickCoroutine;


    void AirKick()
    {
        if (isAirKick) return;
        if (isAttack || isHurt || isDead || isTeleporting ) return;

        // 关键：贴墙、挂墙、墙跳期间不允许飞踢
        if (physicsCheck.onWall || isWallCling || wallJump)
            return;

        airKickCoroutine = StartCoroutine(AirKickRoutine());
    }

    IEnumerator AirKickRoutine()
    {
        isAttack = true;
        isAirKick = true;



        playerAnimation.PlayAirKick();

        float timer = 0f;
        float dir = transform.localScale.x;

        while (timer < airKickDuration)
        {
            yield return new WaitForFixedUpdate();

            if (physicsCheck.isGround)
                break;

            if ((physicsCheck.touchLeftWall && dir < 0f) ||
      (physicsCheck.touchRightWall && dir > 0f))
                break;//飞踢期间碰墙，立刻结束飞踢

            rb.velocity = new Vector2(dir * airKickSpeedX, airKickSpeedY);

            timer += Time.fixedDeltaTime;
        }

        isAirKick = false;
        isAttack = false;


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


    #region  投掷与更换武器
    [Header("投掷与更换武器")]
    public GameObject throwableWeaponPrefab;
    public GameObject bombPrefab;//手榴弹
    public GameObject smokePrefab;//烟雾弹
    public GameObject flashPrefab;//闪光弹
    public GameObject incendiaryPrefab;//燃烧弹
    public GameObject shockPrefab;//震撼弹
    public GameObject knifePrefab;//飞刀

    // float throwForce = 16f;//投掷炸弹力度
    float nextThrow = 0;//投掷冷却
    float ThrowRate = 0.5f;//投掷频率

    private float throwPressTime;

    [Header("投掷蓄力")]
    public bool isHoldingThrow;
    public float throwCharge;              // UI读取 0~1
    private float throwChargeOnRelease;    // 松手时保存
    float maxThrowChargeTime = 1.2f;
    float minThrowForce = 8f;
    float maxThrowForce = 30f;



    void CheckThrow() 
    {
        if (isHoldingThrow)
        {
            throwCharge = Mathf.Clamp01((Time.time - throwPressTime) / maxThrowChargeTime);
        }
        else
        {
            throwCharge = 0f;
        }
    }//蓄力UI

    public void OnThrowStart(InputAction.CallbackContext obj)
    {
        throwPressTime = Time.time;
        isHoldingThrow = true;
    }
    private void OnThrowCanceled(InputAction.CallbackContext ctx)
    {
        float holdTime = Time.time - throwPressTime;

        throwChargeOnRelease = throwCharge;
        isHoldingThrow = false;

        if (holdTime >= chargeThreshold)
        {
            Throw();
        }
        else
        {
            ChangeWeapon();
        }
    }


    public void Throw()
    {
        if (!physicsCheck.isGround) { return; }//空中无法投掷

        playerAnimation.PlayThrow();
        isAttack = true;
    }



    public void ThrowWeapon()
    {

        if (throwType == 0) { return; }

        switch (throwType)
        {
            case 1:
                throwableWeaponPrefab = bombPrefab;
                break;
            case 2:
                throwableWeaponPrefab = smokePrefab;
                break;
            case 3:
                throwableWeaponPrefab = flashPrefab;
                break;
            case 4:
                throwableWeaponPrefab = incendiaryPrefab;
                break;
            case 5:
                throwableWeaponPrefab = shockPrefab;
                break;
            case 6:
                throwableWeaponPrefab = knifePrefab;
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
                float finalForce = Mathf.Lerp(minThrowForce, maxThrowForce, throwChargeOnRelease);

                Vector2 dir = new Vector2(transform.localScale.x * 0.6f, 1f).normalized;
                rb.velocity = dir * finalForce;
            }


            nextThrow = Time.time + ThrowRate;
        }


    }




    [Header("武器槽")]
    public int meleeSlot = 0;   // 0空手 1匕首类
    public int rangedSlot = 0;   // 0空手 -1手枪类  -2步枪类
    public int Slot = 0;// 0近战插槽  1手枪插槽

    public Attack[] playerAttacks;//每当武器切换了之后，attack伤害效果也要更换

    public void ChangeWeapon()
    {

        Slot = Slot == 0 ? 1 : 0;

        RefreshCurrentWeapon();//局内更换武器
        SaveCurrentGame();//每次局内切换武器记录

    }
    public void RefreshCurrentWeapon()
    {
        if (Slot == 0)
        {

            frameEvent_Audio._Attack_katana_draw();

            if (meleeType == 0)
            {
                attackType = 0; // 踢击

            }
            else
            {
                attackType = 1; // 挥砍
            }

            SetHitEffectType(attackType);//暂时这么写
        }
        else
        {
            frameEvent_Audio._Attack_katana_in();


            if (rangedSlot == -1 && pistolType != 0)
                attackType = -1; // 手枪射击
            else if (rangedSlot == -2 && rifleType != 0)
                attackType = -2; // 步枪射击
            else
                attackType = 0; // 没远程武器时回到踢击
        }


        UIManager.instance.RefreshWeaponSlotUI(this);
 

    }//更换玩家当前装备显示与UI层显示


    public void SetHitEffectType(int Type) 
    {
        foreach (Attack attack in playerAttacks)
        {
            if (attack == null) continue;

            attack.hitEffectType = Type;
        }
    }//集体更换攻击特效


    #endregion

    #region  射击
    [Header("射击")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform firePoint_Crouch_Pistol;
    public Transform firePoint_Crouch_Rifle;
    Vector3 spawnPos;

    public float bulletSpeed = 20f;
    public float bulletLifeTime = 2f;

    [Header("枪械精度")]
    public float spreadAngle = 3f; // 误差角度，0 = 完全精准

    [Header("枪械弹药")]
    public int maxAmmo = 10;
    public int currentAmmo = 10;
    public int magazineCount;//弹夹数

    [Header("弹壳")]
    GameObject magazinePrefab;
    public GameObject magazinePrefab_Pistol;
    public GameObject magazinePrefab_Rifle;
    public float magazineForceX = 2f;
    public float magazineForceY = 4f;


    private void SpawnMagazine()
    {
        if (attackType == -1) { magazinePrefab = magazinePrefab_Pistol; }
        if (attackType == -2) { magazinePrefab = magazinePrefab_Rifle; }

        if (magazinePrefab == null) return;

        if (isCrouch)
        {
            if (attackType == -1) { spawnPos = firePoint_Crouch_Pistol.position; }
            if (attackType == -2) { spawnPos = firePoint_Crouch_Rifle.position; }
        }
        else
        {
            spawnPos = firePoint.position;
        }

        GameObject mag = Instantiate(
            magazinePrefab,
            spawnPos,
            Quaternion.identity
        );

        Rigidbody2D rb = mag.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            float dirX = -Mathf.Sign(transform.localScale.x); // 往角色后方弹

            Vector2 force = new Vector2(
                dirX * UnityEngine.Random.Range(magazineForceX * 0.7f, magazineForceX * 1.3f),
                UnityEngine.Random.Range(magazineForceY * 0.7f, magazineForceY * 1.3f)
            );

            rb.AddForce(force, ForceMode2D.Impulse);
            rb.AddTorque(UnityEngine.Random.Range(-180f, 180f));
        }
    }//弹壳飞舞
    public void Shoot()
    {
        if (bulletPrefab == null) return;

        if (isCrouch)
        {
            if (attackType == -1) { spawnPos = firePoint_Crouch_Pistol.position; }
            if (attackType == -2) { spawnPos = firePoint_Crouch_Rifle.position; }
        }
        else
        {
            spawnPos = firePoint.position;
        }

        GameObject bullet = Instantiate(
            bulletPrefab,
            spawnPos,
            Quaternion.identity
        );

        float dirX = transform.localScale.x >= 0 ? 1f : -1f;

        Vector2 dir = new Vector2(dirX, 0f);

        // 准头误差
        float randomAngle = UnityEngine.Random.Range(-spreadAngle, spreadAngle);
        dir = Quaternion.Euler(0f, 0f, randomAngle) * dir;

        Bullet bulletScript = bullet.GetComponent<Bullet>();

        if (bulletScript != null)
        {
            bulletScript.Init(dir, bulletSpeed, bulletLifeTime);
        }

        //把远程伤害来源告诉子弹
        Attack attack = bullet.GetComponentInChildren<Attack>();

        if (attack != null)
        {
            attack.owner = GetComponent<Character>();
        }



        if (attackType == -1)
        {
            switch (pistolType)
            {
                case 1:
                    frameEvent_Audio._Bullet_Pistol_1();
                    break;
                case 2:
                    frameEvent_Audio._Bullet_Pistol_2();
                    break;
                case 3:
                    frameEvent_Audio._Bullet_Pistol_3();
                    break;
            }
        }
        else
        {
            switch (rifleType)
            {
                case 1:
                    frameEvent_Audio._Bullet_M4a1();
                    break;
                case 2:
                    frameEvent_Audio._Bullet_AK();
                    break;

            }
        }





        ChangeAmmo(-1);//削减子弹数
        SpawnMagazine();//弹壳飞舞




       
    }



    public void ChangeAmmo(int value)
    {
        currentAmmo += value;

        currentAmmo = Mathf.Clamp(
            currentAmmo,
            0,
            maxAmmo
        );

        UIManager.instance.RefreshAmmoUI(currentAmmo, maxAmmo, magazineCount);

    }//更改子弹数
    private void OnReload(InputAction.CallbackContext obj)
    {
        Debug.Log("换单");
        Reload();//主动换单
    }
    public void Reload()
    {

        if (!physicsCheck.isGround) return; // 空中禁止换弹
        if (attackType >= 0) return; // 不是枪


        if (magazineCount <= 0) 
        {
            AudioManager.Instance.PlayFX(AudioManager.Instance.Attack_bomb_bounce_1);
            isAttack = false;

            return;
        }//没有弹夹可以换了

        isAttack = true;

        playerAnimation.PlayReload();
    }


    public void OnReloadAnimationEnd()
    {

        if (magazineCount > 0) { magazineCount--; }//开局也要触发
      


        currentAmmo = maxAmmo;

        isAttack = false;

        UIManager.instance.RefreshAmmoUI(currentAmmo, maxAmmo, magazineCount);
    }//换单结束帧事件触发

    public void AddMagazine(int value)
    {
        magazineCount += value;
        magazineCount = Mathf.Clamp(magazineCount, 0, 99);

        UIManager.instance.RefreshAmmoUI(currentAmmo, maxAmmo, magazineCount);
    }//捡弹夹

   
    #endregion

    #region  UI层
    [Header("打开暂停菜单隐藏交互碰撞体")]
    public GameObject Sign;
    private void OnPause(InputAction.CallbackContext ctx)
    {
        UIManager.instance.TogglePause();
    }
    public void OnCancel(InputAction.CallbackContext ctx)
    {
        //if (UIManager.instance.isPaused)
        //{
        //    UIManager.instance.ClosePause();
        //}
        UIManager.instance.OnCancel();
    }
    #endregion


    #region 放大缩小镜头
    [Header("放大缩小镜头")]
    public CameraControl cameraControl;

    private void OnZoomCamera(InputAction.CallbackContext ctx)
    {
        frameEvent_Audio._UI_Click();//切换相机发出按钮声

        cameraControl.ToggleZoom();
    }
    #endregion



    #endregion
}
