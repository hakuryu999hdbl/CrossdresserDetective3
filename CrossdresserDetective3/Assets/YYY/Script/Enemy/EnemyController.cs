using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("动画状态")]
    EnemyBaseState currentState;//当前状态
    public Animator anim;
    public int animState;

    int attackLayer;
    int skillLayer;
    int jumpLayer;
    int deadLayer;




    [Header("基础属性")]
    public Character character;//用于开启技能使用期间的无敌
    public bool isDead = false;
    public bool isDizzy = false;
    public bool hasBomb;//是否持有炸弹
    public GameObject CheckArea;//死后隐藏视野范围（要是敌人直接消失就不用了）

    [Header("敌人巡逻")]
    public float speed;
    public Transform targetPoint;
    public PhysicsCheck physicsCheck;//检测左右有墙翻转

    [Header("敌人跳跃")]
    public float jumpForce = 6f;
    public float jumpCooldown = 0.8f;
    private float nextJumpTime;

    [Header("敌人下蹲")]
    public bool isCrouch;


    [Header("敌人攻击")]
    public float attackRate;//攻击冷却
    public float attackRange, skillRange;//攻击范围
    float nextAttack = 0;

    public List<Transform> attackList = new List<Transform>();

    public PatrolState patrolState = new PatrolState();//巡逻状态
    public AttackState attackState = new AttackState();//攻击状态
    public SearchState searchState = new SearchState();//搜索状态
    public HitState hitState = new HitState();//受击/眩晕状态

    public ChargeSkillState chargeSkillState = new ChargeSkillState();//冲撞状态
    public AimThrowSkillState aimThrowSkillState = new AimThrowSkillState();//瞄准投掷状态
    public BlockState blockState = new BlockState();//防御状态


    public virtual void Init()
    {
        //别找了我直接赋值，这样后面加东西一改排序就出问题
        //anim = transform.GetChild(1).GetComponentInChildren<Animator>();//我把敌人动画放下面了第二个物体
        //alarmSign = transform.GetChild(0).gameObject;//所有敌人都有这个感叹号标识，抓下面第一个物体


        //抓层，让死亡的时候把别的层权重关掉
        attackLayer = anim.GetLayerIndex("Attack Layer");
        jumpLayer = anim.GetLayerIndex("Jump Layer");
        skillLayer = anim.GetLayerIndex("Skill Layer");
        deadLayer = anim.GetLayerIndex("Dead Layer");




        physicsCheck = GetComponent<PhysicsCheck>();

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();

        if (!isAreaSpawnedEnemy)
        {
            GameManager.instance.RegisterSceneEnemy(this);
        }//场景内的单个敌人登记进入GameManager

    }//敌人子类会各自在开始的时候收进父级不需要的东西（虚类）

    public bool isAreaSpawnedEnemy = false;//只登记一次

    private void Awake()
    {
        Init();
    }
    void Start()
    {

        //frameEvent.FadeIn(0.4f);//所有Spine都淡入


       
        SetFirstDirection(); //初始朝向


        TransitionToState(patrolState);//一开始进入巡逻状态

        RandomizeZ();

        RandomSkin();
        RefreshPlayerSkin();//初始更新皮肤



    }





    public virtual void Update()
    {

        anim.SetBool("dead", isDead);
        anim.SetBool("isDizzy", isDizzy);
        if (isDead)
        {
            // 关闭其他动作层权重
            anim.SetLayerWeight(attackLayer, 0f);
            anim.SetLayerWeight(skillLayer, 0f);
            anim.SetLayerWeight(jumpLayer, 0f);
         
            // 打开死亡层
            anim.SetLayerWeight(deadLayer, 1f);


            animState = 3;//防止不停触发Run





            return;
        }

        if (isCatching)
        {
            rb.velocity = Vector2.zero;
            anim.SetInteger("state", 0);

          
            return;
        }//抓取期间的锁住


        if (GameManager.instance != null && GameManager.instance.gameOver)
        {
            //attackList.Clear();
            //targetPoint = null;
            //anim.ResetTrigger("attack");
            //anim.ResetTrigger("skill");
            //animState = 0;
            //anim.SetInteger("state", animState);

            if (!gameOverStopped)
            {
                StopEnemyOnGameOver();
                gameOverStopped = true;
            }

            return;
        }//玩家死后强制停战



        if (physicsCheck.isGround && physicsCheck.isOnStair &&  !isDead)
        {

            rb.velocity = new Vector2(0f, rb.velocity.y);
        
        }//位于楼梯不会产生在地面较大滑动



        currentState.OnUpdate(this);//每帧执行状态
        anim.SetInteger("state", animState);

        anim.SetBool("isCrouch", isCrouch);

        anim.SetBool("isGround", physicsCheck != null && physicsCheck.isGround);
        anim.SetFloat("yVelocity", rb != null ? rb.velocity.y : 0f);



        if (questionSign == null) return;

        float parentDir = transform.localScale.x >= 0 ? 1f : -1f;

        questionSign.transform.localScale = new Vector3(
            Mathf.Abs(questionSign.transform.localScale.x) * parentDir,
            Mathf.Abs(questionSign.transform.localScale.y),
            Mathf.Abs(questionSign.transform.localScale.z)
        );


       AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(attackLayer);
       
       if (stateInfo.IsName("Shooting") ||
           stateInfo.IsName("Shooting_Crouch") ||
            stateInfo.IsName("Catch") ||
             stateInfo.IsName("Throw")
           )
       {
           speed = 0;
            //Debug.Log("执行射击");
       }
       else
       {
           speed = 3;
       }

        //if (stateInfo.IsName("Girl_Catch")
        //   )
        //{
        //    CheckMissCatch();//在敌人抓取的每一帧期间不停的检测玩家，只要玩家没有处于抓住就立刻停下
        //    Debug.Log("检测抓住");
        //}


    }

    public void TransitionToState(EnemyBaseState state)
    {
        // 只要从巡逻状态离开，之后就变成普通走走停停
        if (currentState == patrolState && state != patrolState)
        {
            patrolMode = EnemyPatrolMode.RandomPatrol;
        }

        //只有攻击和技能产生警告
        if (state == attackState|| state == chargeSkillState) 
        {
            checkArea.ShowAlarm();
        }



        currentState?.ExitState(this);


        currentState = state;
        currentState.EnterState(this);
    }//切换状态

    public virtual void EnterBattleState()
    {
        TransitionToState(attackState);
    }//敌人子类进入战斗状态


    private bool gameOverStopped = false;
    private void StopEnemyOnGameOver()
    {
        attackList.Clear();
        targetPoint = null;

        // 停止物理运动
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 停止攻击触发
        anim.ResetTrigger("attack");
        anim.ResetTrigger("skill");
        anim.ResetTrigger("hit");

        // 强制回到待机动画
        animState = 0;
        anim.SetInteger("state", 0);
        anim.SetBool("isGround", true);
        anim.SetFloat("yVelocity", 0f);

        // 关掉跳跃层，避免卡在跳跃动作
        anim.SetLayerWeight(jumpLayer, 0f);
    }












    /// <summary>
    /// 受击/眩晕状态
    /// </summary>
    #region
    [Header("受击/眩晕")]
    public float stunTime = 2f;
    [HideInInspector] public float hitTimer;
    [HideInInspector] public EnemyBaseState stateBeforeHit;
    public void Stun(float duration)
    {
        if (isDead) return;

        if (duration > 0.2f)
        {
            isDizzy = true;
        }


        stunTime = duration;

        TransitionToState(hitState);
    }


    public void Shock(float duration)
    {
        if (isDead) return;

        Character character = GetComponent<Character>();

        character.currentHealth = 1; // 按你的血量变量名改
        Stun(duration);
    }
    #endregion

    /// <summary>
    /// 搜索状态
    /// </summary>
    #region

    [Header("搜索状态")]
    public float searchTime = 3f;        // 继续追寻几秒
    public float lookTime = 1.2f;        // 左右看的时间
    public Vector3 lastKnownTargetPos;   // 最后看到目标的位置

    public GameObject questionSign;      // 问号标记，可选
    public GameObject alarmSign;    //警戒标记
    public GameObject InvulnerableSign;    //无敌标记
    #endregion


    /// <summary>
    /// 攻击状态
    /// </summary>
    #region
    [Header("攻击状态")]
    public Transform heldBomb;//大块头的扔炸弹方向
    public void MoveToTarget()
    {

        if (targetPoint == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetPoint.position.x, transform.position.y),
            speed * Time.deltaTime
        );//只追 X

        FilpDirection();

        //TryJumpObstacle();//暂时关闭跳跃


    }//前往目标

    public void FilpDirection()
    {
        if (transform.position.x < targetPoint.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
            //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            //transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

    }//反转：追逐目标

    private void TryJumpObstacle()
    {
        if (physicsCheck == null) return;
        if (rb == null) return;
        if (!physicsCheck.isGround) return;
        if (Time.time < nextJumpTime) return;

        bool wallAhead = false;

        if (transform.rotation.eulerAngles.y == 0)
        {
            wallAhead = physicsCheck.touchRightWall;
        }
        else
        {
            wallAhead = physicsCheck.touchLeftWall;
        }

        if (!wallAhead) return;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        nextJumpTime = Time.time + jumpCooldown;
    }





    [Header("远程类型敌人")]
    public bool isRangedEnemy;
    public float shootVerticalTolerance = 0.3f; // 玩家和敌人高度差小于这个才开枪
    public float meleeRange = 1.2f;

    [Header("楼层判断")]
    public float loseTargetYDiff = 1.2f;    // 超过这个，丢失目标
    public float regainTargetYDiff = 0.8f;  // 小于这个，重新发现目标
    public bool IsValidAttackTarget(Transform t)
    {
        if (t == null) return false;

        if (!t.CompareTag("Player") && !t.CompareTag("Bomb"))
            return false;

        if (t.CompareTag("Bomb") && t.gameObject.layer != LayerMask.NameToLayer("Bomb"))
            return false;

        // 重新发现目标时，用较小阈值，防止斜坡反复横跳
        if (t.CompareTag("Player"))
        {
            //float yDiff = Mathf.Abs(t.position.y - transform.position.y);
            //
            //if (yDiff > regainTargetYDiff)
            //    return false;

            if (!CanAcquireTargetByHeight(t))
                return false;
        }

        return true;
    }

    public void AttackAction()
    {
        //抓取期间锁住再度攻击
        if (isCatching) return;
        if (capturedPlayer != null) return;
        if (Time.time < nextCatchTime) return; // 抓取后1秒内禁止再次 attack
        if (targetPoint == null) return;


        float distance = Vector2.Distance(transform.position, targetPoint.position);
        float yDiff = Mathf.Abs(targetPoint.position.y - transform.position.y);



        if (isRangedEnemy)
        {
            // 高度差太大：不要开枪，继续追
            if (yDiff > shootVerticalTolerance)
            {
                animState = 2;
                MoveToTarget();
                return;
            }

            // 太近：可以以后切近战
            if (distance < meleeRange)
            {

                anim.SetTrigger("attack");
                return;
            }


            // 玩家比敌人低：下蹲射击
            if (GameManager.instance.player.isCrouch)
            {
                isCrouch = true;

            }
            else
            {
                isCrouch = false;
            }

            // 正常远程射击
            animState = 3;
            return;
        }




        if (Vector2.Distance(transform.position, targetPoint.position) < attackRange)
        {
            if (Time.time > nextAttack)
            {

                if (this is Enemy_3)
                {
                    anim.SetTrigger("catch");
                }
                else
                {
                    anim.SetInteger("attackType", Random.Range(1, 3));
                    anim.SetTrigger("attack");
                }


           

                nextAttack = Time.time + attackRate;
            }
        }



    }//攻击


    [Header("目标Y轴判断")]
    public TargetYMode targetYMode = TargetYMode.SameFloor;

    public bool ShouldCheckTargetY()
    {
        return targetYMode == TargetYMode.SameFloor;
    }

    public bool CanAcquireTargetByHeight(Transform target)
    {
        if (target == null)
            return false;

        // 冲锋、瞄准投掷等敌人完全忽略高度差
        if (!ShouldCheckTargetY())
            return true;

        float yDiff = Mathf.Abs(
            target.position.y - transform.position.y
        );

        return yDiff <= regainTargetYDiff;
    }

    public bool ShouldLoseTargetByHeight(Transform target)
    {
        if (target == null)
            return true;

        // 无视高度差的敌人永远不会因为Y轴丢失玩家
        if (!ShouldCheckTargetY())
            return false;

        float yDiff = Mathf.Abs(
            target.position.y - transform.position.y
        );

        return yDiff > loseTargetYDiff;
    }








    public virtual void SkillAction()
    {

        //Debug.Log("这是炸弹，技能攻击");
        if (Vector2.Distance(transform.position, targetPoint.position) < skillRange)
        {
            if (Time.time > nextAttack)
            {
                // 播放攻击动画
                anim.SetTrigger("skill");
                Debug.Log("针对炸弹技能攻击");
                nextAttack = Time.time + attackRate;
            }
        }

    }// 对炸弹使用技能

  






    #region  射击
    [Header("射击")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public Transform firePoint_Crouch;
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
        if (attackType == -1) { magazinePrefab = magazinePrefab_Pistol; }
        if (attackType == -2) { magazinePrefab = magazinePrefab_Rifle; }

        if (magazinePrefab == null) return;


        if (isCrouch)
        {
            spawnPos = firePoint_Crouch.position;
        }
        else
        {
            spawnPos = firePoint.position;
        }

        spawnPos = firePoint_Crouch.position;



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
            spawnPos = firePoint_Crouch.position;
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



        SpawnMagazine();//弹壳飞舞
    }


    #endregion









    #endregion


    /// <summary>
    /// 冲撞状态
    /// </summary>
    #region
    [Header("冲撞技能")]
    public bool useChargeSkill = false;

    public float chargeReadyTime = 0.5f;      // 瞄准时间
    public float chargeSpeed = 10f;         // 冲锋速度
    public float chargeMaxTime = 1.5f;      // 最长冲锋时间
    public float chargeRecoveryTime = 0.6f; // 后摇时间
    public float chargeCooldown = 5f;       // CD
    public float chargeMinDistance = 2.5f;  // 太近不用冲锋
    public float chargeStopDistance = 0.4f; // 碰到玩家附近停止

    [HideInInspector] public float lastChargeTime = -999f;
    [HideInInspector] public Vector2 chargeDir;
 

    [HideInInspector] public float chargeTargetX;//记录冲刺攻击的玩家
    #endregion


    /// <summary>
    /// 投掷技能
    /// </summary>
    #region
    [Header("瞄准投掷技能")]
    public bool useAimThrowSkill = false;

    public Transform throwAimTarget;// 瞄准物体
    public GameObject throwExplosionPrefab;//爆炸

    public float throwAimTime = 1f;
    public Vector2 throwTargetPos;

    public EnemyThrowObject throwObject;//投掷物

   

    public void AimThrowSpawnExplosion()
    {
        if (throwExplosionPrefab != null)
        {
            //GameObject effect = Instantiate(
            //    throwExplosionPrefab,
            //    throwTargetPos,
            //    Quaternion.identity
            //);
            //
            //Destroy(effect, 1.2f);


            throwObject.Launch(transform.position, throwTargetPos);
        }
    }

    public void AimThrowStartLaugh()
    {
        anim.SetInteger("skillState", 3);
    }

    public void AimThrowOver()
    {
        //StopMove();

        anim.SetInteger("skillState", 0);

        if (throwAimTarget != null)
            throwAimTarget.gameObject.SetActive(false);//瞄准消失

        targetPoint = null;

        TransitionToState(patrolState);
    }


    public void FaceToPosition(Vector3 targetPos)
    {
        float dir = targetPos.x - transform.position.x;

        if (dir > 0)
            transform.localScale = Vector3.one;
        else if (dir < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }//因该是有一样方法的，可以把写重复的地方引导到这里

    #endregion


    /// <summary>
    /// 抓取技能
    /// </summary>
    #region
    [Header("抓取技能")]
    public bool isCatching;//在整个抓取过程中挡住FSM
    public PlayerController capturedPlayer;
    public Catch catchCollider;

    [Header("抓取冷却")]
    public float catchCooldown = 1.0f;
    public float nextCatchTime;

    public GameObject Catch_Collider;//让动画产生，防止bug

    public void StartCatchPlayer(PlayerController player)
    {

        if (isDead) return;
        if (isCatching) return;
        if (capturedPlayer != null) return;
        if (Time.time < nextCatchTime) return;


        isCatching = true;
        capturedPlayer = player;



        //开始抓取的清理
        anim.ResetTrigger("attack");
        anim.ResetTrigger("skill");

        attackList.Clear();
        targetPoint = null;

        rb.velocity = Vector2.zero;


        frameEvent_Audio._Attack_pick();//抓取声

        ReadCurrentGame(player);//敌人读取玩家皮肤


        player.EnterCapturedState();//玩家进入透明

        character.skillInvulnerable = true;//技能无敌

        //Debug.Log("抓住玩家：" + capturedPlayer.name);


        anim.SetTrigger("catchSuccess");
    }





    public void ThrowCapturedPlayer()
    {

        //Debug.Log("投出");

        if (capturedPlayer != null)
        {
            float dir = transform.localScale.x > 0 ? 1f : -1f;
            capturedPlayer.ExitCapturedState(new Vector2(6f * dir, 4f));//玩家恢复
            capturedPlayer = null;
        }

        isCatching = false;
        nextCatchTime = Time.time + catchCooldown;


        //丢出后的清理
        attackList.Clear();
        targetPoint = null;
        anim.ResetTrigger("attack");

        catchCollider.ResetCatch();

        character.skillInvulnerable = false;//技能无敌关闭

        TransitionToState(patrolState);
    }





    public void StartPlayerStruggle()
    {
        if (!isCatching || capturedPlayer == null)
            return;

        capturedPlayer.StartStruggle(this);
    }//开启挣扎


    public void BreakFreeFromPlayer(PlayerController player)
    {
        if (!isCatching)
            return;

        if (capturedPlayer != player)
            return;

        // 先解除双方关系
        capturedPlayer = null;

        isCatching = false;
        nextCatchTime = Time.time + catchCooldown;

        if (catchCollider != null)
            catchCollider.ResetCatch();

        if (Catch_Collider != null)
            Catch_Collider.SetActive(false);

        // 玩家结束挣扎状态
        player.EndStruggle();

        // 玩家恢复显示
        player.ExitCapturedState(Vector2.zero);


        // 玩家播放挣脱攻击动画
        player.playerAnimation.anim.SetTrigger("breakFree");

        // 敌人退出 lewdmove
        anim.SetTrigger("breakFree");
        character.skillInvulnerable = false;//技能无敌关闭

    }//玩家挣扎值满了后的挣脱














    #endregion


    /// <summary>
    /// 防御技能
    /// </summary>
    #region
    public virtual bool TryHandleIncomingAttack(Attack attack)
    {
        return false;
    }


    #endregion


    /// <summary>
    /// 巡逻状态
    /// </summary>
    #region
    [Header("巡逻状态")]
    public float patrolSpeed = 2f;
    public float minWalkTime = 1.5f;
    public float maxWalkTime = 4f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 3f;

    [HideInInspector] public int patrolDir = 1;

    public void MovePatrol()
    {
        transform.position += new Vector3(patrolDir * patrolSpeed * Time.deltaTime, 0f, 0f);

        if (patrolDir > 0)
        {
            //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            //transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            transform.localScale = new Vector3(-1, 1, 1);
        }

    }

    public bool IsWallAheadByDir()
    {
        if (physicsCheck == null) return false;

        if (patrolDir > 0)
            return physicsCheck.touchRightWall;

        if (patrolDir < 0)
            return physicsCheck.touchLeftWall;

        return false;
    }

    public void TurnAround()
    {
        patrolDir *= -1;

        if (patrolDir > 0)
            transform.localScale = new Vector3(1, 1, 1);
        //transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        else
            transform.localScale = new Vector3(-1, 1, 1);
        //transform.rotation = Quaternion.Euler(0f, 180f, 0f);
    }


 

    [Header("巡逻模式")]
    public EnemyPatrolMode patrolMode = EnemyPatrolMode.RandomPatrol;

    [Header("固定站岗")]
    public int startFaceDir = 1; // 1 向右，-1 向左


    public void SetFirstDirection() 
    {

        startFaceDir = startFaceDir >= 0 ? 1 : -1;
        patrolDir = startFaceDir;

        FaceStartDirection();

        patrolDir = startFaceDir;

    }

    public void FaceStartDirection()
    {
        patrolDir = startFaceDir;

        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * startFaceDir;
        transform.localScale = scale;
    }

    #endregion


    /// <summary>
    /// CheckArea视野范围调用
    /// </summary>
    #region

    [Header("视野范围")]
    public EnemyCheckArea checkArea;


    public void OnCheckAreaStay(Collider2D collision)
    {
        //if (!attackList.Contains(collision.transform) && !hasBomb && !isDead && !GameManager.instance.gameOver)
        //{
        //
        //    attackList.Add(collision.transform);
        //
        //}//只要不是新的，就装进去(如果持有炸弹/自己死亡/玩家死亡，不需要再添加新的进去)
        //
        //
        //if (collision.CompareTag("Player") || collision.CompareTag("Bomb"))
        //{
        //    lastKnownTargetPos = collision.transform.position;
        //}//记录最后看到的目标位置（搜索状态使用）


        if (hasBomb || isDead || GameManager.instance.gameOver)
            return;

        Transform t = collision.transform;

        if (!IsValidAttackTarget(t))
            return;

        if (!attackList.Contains(t))
        {
            attackList.Add(t);
        }

        lastKnownTargetPos = t.position;


    }//只要持续处于范围之中
    public void OnCheckAreaExit(Collider2D collision)
    {
        attackList.Remove(collision.transform);
    }//离开视野范围

    #endregion


    #region  投掷

    [Header("投掷")]
    public GameObject throwableWeaponPrefab;
    public GameObject bombPrefab;//手榴弹
    public GameObject smokePrefab;//烟雾弹
    public GameObject flashPrefab;//闪光弹
    public GameObject incendiaryPrefab;//燃烧弹
    public GameObject shockPrefab;//震撼弹
    public GameObject knifePrefab;//飞刀

    float throwForce = 16f;//投掷力度

    public void ThrowWeapon()
    {
        if (meleeType == 0) return; // 空手不能扔


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


    }
    #endregion

    /// <summary>
    /// Spine外观
    /// </summary>
    #region
    [Header("Spine外观")]
    public FrameEvent frameEvent;
    public int beltIndex;
    public int hairIndex;
    public int clothesIndex;
    public int glovesIndex;
    public int pantiesIndex;
    public int shoesIndex;
    public int skirtIndex;
    public int stockingsIndex;
    public int hatIndex;
    public int maskIndex;


    public int Girl_hairIndex;
    public int Girl_clothesIndex;
    public int Girl_glovesIndex;
    public int Girl_underwearIndex;
    public int Girl_shoesIndex;
    public int Girl_stockingsIndex;
    public int Girl_hatIndex;
    public int Girl_maskIndex;

    public int Man_hairIndex;
    public int Man_clothesIndex;

    [Header("武器与攻击方式")]
    public int meleeType;//0空手 1匕首 2武士刀 3尼泊尔军刀
    public int pistolType;//0空手 1柯尔特M1911 2沙鹰手枪 3格洛克手枪
    public int rifleType;//0空手 1步枪M4A1 2步枪AK47
    public int throwType;//0空手 1手榴弹 2烟雾弹 3闪光弹 4燃烧弹  5震撼弹  6飞刀
    public int attackType;//-2步枪射击  -1手枪射击 0踢击 1挥砍

    public int bondageType;//0绳子捆绑 1锁链捆绑


    public void RandomSkin()
    {
        Girl_hairIndex = Random.Range(3, 4);

        Girl_clothesIndex = Random.Range(0, 6);
        Girl_glovesIndex = Random.Range(0, 2);

        Girl_shoesIndex = Random.Range(0, 3);


        switch (Random.Range(0, 3))
        {
            case 0:
                Girl_underwearIndex = 0;
                Girl_stockingsIndex = 0;
                break;
            case 1:
                Girl_underwearIndex = 1;
                Girl_stockingsIndex = 0;
                break;
            case 2:
                Girl_underwearIndex = 2;
                Girl_stockingsIndex = 2;
                break;
        }

        Girl_hatIndex = Random.Range(0, 2);
        Girl_maskIndex =1;


        Man_hairIndex = Random.Range(0, 3);
        Man_clothesIndex = Random.Range(1, 3);


        meleeType = Random.Range(1, 4);


        pistolType = Random.Range(1, 4);
        rifleType = Random.Range(1, 3);

        
    }




    public void RefreshPlayerSkin()
    {
        if (frameEvent == null) return;

        frameEvent.ShowCurrentAll(
            beltIndex,
            hairIndex,
            clothesIndex,
            glovesIndex,
            pantiesIndex,
            shoesIndex,
            skirtIndex,
            stockingsIndex,
            hatIndex,
            maskIndex,

            Girl_hairIndex,
            Girl_clothesIndex,
            Girl_glovesIndex,
            Girl_underwearIndex,
            Girl_shoesIndex,
            Girl_stockingsIndex,
            Girl_hatIndex,
            Girl_maskIndex,

            Man_hairIndex,
            Man_clothesIndex,

            meleeType,
            pistolType,
            rifleType,
            throwType,

            bondageType
       );

    }//更新外观

    public void ReadCurrentGame(PlayerController player)
    {
       

        clothesIndex = player.clothesIndex;
        glovesIndex = player.glovesIndex;
        pantiesIndex = player.pantiesIndex;
        shoesIndex = player.shoesIndex;
        skirtIndex = player.skirtIndex;
        stockingsIndex = player.stockingsIndex;

        RefreshPlayerSkin();

    }//敌人在抓取玩家前读取玩家皮肤

    #endregion



    /// <summary>
    /// 受伤死亡
    /// </summary>
    #region
    [Header("受伤死亡")]
    public Rigidbody2D rb;//我发现这个Enemy居然是transform移动驱动的
    public CapsuleCollider2D coll;
    public FrameEvent_Audio frameEvent_Audio;
    public GameObject Effect_Blood;//受伤特效
    public GameObject Strike_Effect;//剑光特效
    public GameObject Hit_Effect;//打击特效



    private bool IsHitFromBehind(Vector3 attackPos)
    {
        float enemyFacing = transform.localScale.x >= 0 ? 1f : -1f;

        float dirToAttack = attackPos.x - transform.position.x;

        // 攻击来自敌人背后
        return Mathf.Sign(dirToAttack) != Mathf.Sign(enemyFacing);
    }

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
                frameEvent_Audio._Attack_hit();
                break;

            case 1:
                // 斩击特效
                Strike_Effect.transform.localRotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-45f, 45f));
                Strike_Effect.SetActive(true);
                break;
        }







        Character attackerCharacter = attack.owner;//一旦受伤，立刻读取Attack的主人

        Vector3 attackSourcePos =
        attackerCharacter != null
        ? attackerCharacter.transform.position
        : attack.transform.position;//如果attack有owner，用owner位置，没有用attack位置

        bool hitFromBehind = IsHitFromBehind(attackSourcePos);//判断伤害来源是不是伤害的主人

        // 👉 攻击从背后打来 = 直接死  // 👉 眩晕 = 直接死
        if (hitFromBehind||isDizzy)
        {
            frameEvent_Audio._Attack_largeSword();//暂时先把暗杀声音写在这
            OnDie();//背后暗杀
            return;
        }



        // 👉 巡逻状态被偷袭 = 直接死
        //if (currentState == patrolState)
        //{
        //    
        //    OnDie();
        //    return;
        //}



        //一旦受伤立刻把Attack的根物体的character所在物体立为目标（这个是用于近战）
        //Character attackerCharacter = attack.GetComponentInParent<Character>();

        if (attackerCharacter != null)
        {
            Transform attacker = attackerCharacter.transform;

            targetPoint = attacker;

            if (!attackList.Contains(attacker))
            {
                attackList.Add(attacker);
            }

            TransitionToState(attackState);
        }




        //isHurt = true;

        Stun(0.2f);//为了让敌人的视野有同等反应


        anim.SetInteger("hitType", Random.Range(1, 3));
        anim.SetTrigger("hit");


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


        frameEvent.FlashRed();//受伤闪红


        Destroy(blood, 1f); // 1秒后销毁
    }






    public virtual void OnDie()
    {
        if (isDead)
            return;


        PlayBloodEffect();//不知道为啥死亡有些时候血特效出不来就先这样吧


        GameManager.instance.SceneEnemyDead(this);//从GameManager那里划走


        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("Environment");
        CheckArea?.SetActive(false);

        //防止尸体堵在门前
        //coll.enabled = false;
        //rb.bodyType = RigidbodyType2D.Static;

        if (throwAimTarget != null) 
        {
            throwAimTarget.gameObject.SetActive(false);
        }

    }


    public virtual bool IgnoreIncomingDamage()
    {
        return false;
    }//无敌状态



    public void OnBlastHit(Vector3 blastPos, float force)
    {
        if (rb == null) return;

        // 炸飞方向
        Vector2 dir = (transform.position - blastPos).normalized;
        dir += Vector2.up;

        rb.velocity = Vector2.zero;
        rb.AddForce(dir * force, ForceMode2D.Impulse);

        // 活着被炸：死亡
        if (!isDead)
        {
            OnDie();//直接炸死
            return;
        }

        // 已经是尸体：重新播一次死亡动画
        anim.Play("Dead", deadLayer, 0f);
    }


    #endregion


    public void RandomizeZ()
    {
        anim.transform.position = new Vector3(
         anim.transform.position.x,
         anim.transform.position.y,
        Random.Range(-0.2f, -0.3f)
    );
    }
}


public enum EnemyPatrolMode
{
    Guard,          // 固定站岗，不移动
    RandomPatrol,   // 走走停停
    ContinuousPatrol // 不停巡逻
}

public enum TargetYMode
{
    SameFloor,  // 攻击状态下一定要同楼层Y接近才能攻击，否则一直移动
    IgnoreHeight  // 只要进入视野范围就攻击
}