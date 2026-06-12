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
    public int beltIndex;
    public int clothesIndex;
    public int glovesIndex;
    public int pantiesIndex;
    public int shoesIndex;
    public int skirtIndex;
    public int stockingsIndex;

    [Header("武器与攻击方式")]
    public int meleeType;//0空手 1匕首 2武士刀 3尼泊尔军刀
    public int pistolType;//0空手 1柯尔特M1911 2沙鹰手枪 3格洛克手枪
    public int rifleType;//0空手 1步枪M4A1 2步枪AK47
    public int throwType;//0空手 1手榴弹 2烟雾弹 3闪光弹 4飞刀
    public int attackType;//-2步枪射击  -1手枪射击 0踢击 1挥砍



    public void RefreshPlayerSkin()
    {

        frameEvent.ShowCurrentAll(
            beltIndex,
            clothesIndex,
            glovesIndex,
            pantiesIndex,
            shoesIndex,
            skirtIndex,
            stockingsIndex,
            meleeType,
            pistolType,
            rifleType
        );

        frameEvent_UI.ShowCurrentAll(
          beltIndex,
          clothesIndex,
          glovesIndex,
          pantiesIndex,
          shoesIndex,
          skirtIndex,
          stockingsIndex,
          meleeType,
          pistolType,
          rifleType
      );


    }//更新外观

    public void SetWeapon(int newWeaponType)
    {
        meleeType = newWeaponType;

        if (meleeType == 0)
            attackType = 0;//赤手空拳
        else
            attackType = 1;



        RefreshPlayerSkin();//捡起的武器调用这里
    }//捡起的武器调用这里



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

                }//裤袜固定部件
                if (stockingsIndex == 1 && pantiesIndex != 0)
                {
                    beltIndex = 1;
                }
                else
                {
                    beltIndex = 0;

                } //吊带袜裤袜固定有内裤的情况下吊带出现


                break;

            case GameFlowData.EquipPart.Melee:
                meleeType = index;
                break;

            case GameFlowData.EquipPart.Pistol:
                pistolType = index;
                break;

            case GameFlowData.EquipPart.Rifle:
                rifleType = index;
                break;

            case GameFlowData.EquipPart.Throw:
                throwType = index;
                break;
        }
     


        RefreshPlayerSkin();

        SaveCurrentGame();
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

        data.attackType = attackType;

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

        attackType = data.attackType;

        RefreshPlayerSkin(); 
    }


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

        inputControl.Gameplay.Throw.started += OnThrowStart;
        inputControl.Gameplay.Throw.canceled += OnThrowCanceled;


        inputControl.Gameplay.Pause.started += OnPause;

        inputControl.Gameplay.ZoomCamera.started += OnZoomCamera;


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

    public bool isHoldingAttack;//是否持续按下攻击键

    private void OnAttackStarted(InputAction.CallbackContext ctx)
    {
        
      

        //if (attackType==-2 && currentAmmo >= 0 && physicsCheck.isGround)
        //{
        //    isHoldingAttack = true;
        //    isAttack = true;
        //
        //    return;
        //}//步枪可以连发

        attackPressTime = Time.time;
    }

    private void OnAttackCanceled(InputAction.CallbackContext ctx)
    {
       

        //if (attackType == -2 && currentAmmo >= 0 && physicsCheck.isGround)
        //{
        //    isHoldingAttack = false;
        //    isAttack = false;
        //
        //    return;
        //}//步枪可以连发



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

        if (!physicsCheck.isGround) { return; }//空中无法攻击

        
        isAttack = true;


        if (attackType < 0 && currentAmmo <= 0)//如果是枪械类 ,子弹不够就会触发换单
        {
            frameEvent_Audio._Bullet_OutOfBullet();
            Reload();
        }
        else 
        {
            playerAnimation.PlayAttack();
        }
       

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
    public GameObject knifePrefab;//飞刀

    float throwForce = 16f;//投掷炸弹力度
    float nextThrow = 0;//投掷冷却
    float ThrowRate = 0.5f;//投掷频率

    private float throwPressTime;

    public void OnThrowStart(InputAction.CallbackContext obj) 
    {
        throwPressTime = Time.time;
    }
    private void OnThrowCanceled(InputAction.CallbackContext ctx)
    {
        float holdTime = Time.time - throwPressTime;

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
                Vector2 dir = new Vector2(transform.localScale.x * 0.6f,1f).normalized;
                rb.velocity = dir * throwForce;
            }


            nextThrow = Time.time + ThrowRate;
        }


       //attackType = 0;
       //RefreshPlayerSkin();
    }


    

    public void ChangeWeapon() 
    {
        
        if (attackType == 0 || attackType == 1)
        {
            attackType = UnityEngine.Random.Range(-2, 0);//切换射击
            frameEvent_Audio._SE_Clothes();//暂时这么写
        }
        else 
        {
            attackType = UnityEngine.Random.Range(0,2);//切换近战
            frameEvent_Audio._Attack_katana_draw();//暂时这么写
        }

        
    }

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
    public bool isReloading;

    [Header("弹壳")]
    GameObject magazinePrefab;
    public GameObject magazinePrefab_Pistol;
    public GameObject magazinePrefab_Rifle;
    public float magazineForceX = 2f;
    public float magazineForceY = 4f;

    private void SpawnMagazine()
    {
        if (attackType == -1){ magazinePrefab = magazinePrefab_Pistol; }
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
            if (attackType == -1){ spawnPos = firePoint_Crouch_Pistol.position; }
            if (attackType == -2) { spawnPos = firePoint_Crouch_Rifle.position; }
        }
        else
        {
            spawnPos =  firePoint.position;
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

        UIManager.instance.RefreshAmmoUI(
            currentAmmo,
            maxAmmo
        );
    }//更改子弹数
    private void OnReload(InputAction.CallbackContext obj)
    {
        Reload();
    }
    public void Reload()
    {
        if (isReloading) return;
        if (currentAmmo >= maxAmmo) return;
        if (attackType >= 0) return; // 不是枪

        isReloading = true;
        isAttack = true;

        playerAnimation.PlayReload();
    }


    public void OnReloadAnimationEnd()
    {
        currentAmmo = maxAmmo;
        isReloading = false;
        isAttack = false;

        UIManager.instance.RefreshAmmoUI(currentAmmo,maxAmmo);
    }//换单结束帧事件触发

    

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
