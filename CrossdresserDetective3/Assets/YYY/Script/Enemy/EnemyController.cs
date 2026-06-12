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
    int jumpLayer;
    int deadLayer;




    public GameObject alarmSign;

    [Header("基础属性")]
    public bool isDead = false;
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




    [Header("敌人攻击")]
    public float attackRate;//攻击冷却
    public float attackRange, skillRange;//攻击范围
    float nextAttack = 0;

    public List<Transform> attackList = new List<Transform>();

    public PatrolState patrolState = new PatrolState();//巡逻状态
    public AttackState attackState = new AttackState();//攻击状态
    public SearchState searchState = new SearchState();//搜索状态

    public virtual void Init() 
    {
        //别找了我直接赋值，这样后面加东西一改排序就出问题
        //anim = transform.GetChild(1).GetComponentInChildren<Animator>();//我把敌人动画放下面了第二个物体
        //alarmSign = transform.GetChild(0).gameObject;//所有敌人都有这个感叹号标识，抓下面第一个物体


        //抓层，让死亡的时候把别的层权重关掉
        attackLayer = anim.GetLayerIndex("Attack Layer");
        jumpLayer = anim.GetLayerIndex("Jump Layer");
        deadLayer = anim.GetLayerIndex("Dead Layer");




        physicsCheck = GetComponent<PhysicsCheck>();

        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<CapsuleCollider2D>();

        

    }//敌人子类会各自在开始的时候收进父级不需要的东西（虚类）

    private void Awake()
    {
        Init();
    }
    void Start()
    {

        frameEvent.FadeIn(0.4f);//所有Spine都淡入

        TransitionToState(patrolState);//一开始进入巡逻状态

        RandomizeZ();


        RefreshPlayerSkin();//初始更新皮肤

        SetWeapon();//初始随机武器
    }

    public virtual void Update()
    {

        anim.SetBool("dead", isDead);
        if (isDead)
        {
            // 关闭其他动作层权重
            anim.SetLayerWeight(attackLayer, 0f);
            anim.SetLayerWeight(jumpLayer, 0f);

            // 打开死亡层
            anim.SetLayerWeight(deadLayer, 1f);


            animState = 3;//防止不停触发Run



         
           
            return;
        }




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







        currentState.OnUpdate(this);//每帧执行状态
        anim.SetInteger("state", animState);



        anim.SetBool("isGround", physicsCheck != null && physicsCheck.isGround);
        anim.SetFloat("yVelocity", rb != null ? rb.velocity.y : 0f);



        if (questionSign == null) return;

        float parentDir = transform.localScale.x >= 0 ? 1f : -1f;

        questionSign.transform.localScale = new Vector3(
            Mathf.Abs(questionSign.transform.localScale.x) * parentDir,
            Mathf.Abs(questionSign.transform.localScale.y),
            Mathf.Abs(questionSign.transform.localScale.z)
        );


    }

    public void TransitionToState(EnemyBaseState  state) 
    {
        currentState = state;
        currentState.EnterState(this);
    }//切换状态




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
        anim.ResetTrigger("kick");
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
    /// 搜索状态
    /// </summary>
    #region

    [Header("搜索状态")]
    public float searchTime = 3f;        // 继续追寻几秒
    public float lookTime = 1.2f;        // 左右看的时间
    public Vector3 lastKnownTargetPos;   // 最后看到目标的位置

    public GameObject questionSign;      // 问号标记，可选
    #endregion


    /// <summary>
    /// 攻击状态
    /// </summary>
    #region
    [Header("攻击状态")]
    public Transform heldBomb;//扔炸弹方向
    public void MoveToTarget()
    {
        //transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        //FilpDirection();


        if (targetPoint == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            new Vector2(targetPoint.position.x, transform.position.y),
            speed * Time.deltaTime
        );//只追 X

        FilpDirection();

        //TryJumpObstacle();//暂时关闭跳跃


    }//前往目标


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







    //bool inAttackRangeLastFrame = false;


    public void AttackAction()
    {

        if (Vector2.Distance(transform.position, targetPoint.position) < attackRange)
        {
            if (Time.time > nextAttack)
            {
                // 播放攻击动画
                switch (attackType) 
                {
                    case 0:
                        anim.SetTrigger("kick");
                        break;
                    case 1:
                        anim.SetTrigger("attack");
                        break;

                }
               

                //Debug.Log("普通攻击");
                nextAttack = Time.time + attackRate;
            }
        }

       // float distance = Vector2.Distance(transform.position, targetPoint.position);
       // bool inRange = distance < attackRange;
       //
       // // 👉 刚进入攻击范围
       // if (inRange && !inAttackRangeLastFrame)
       // {
       //     nextAttack = Time.time + attackRate;
       // }
       //
       // if (inRange)
       // {
       //     if (Time.time > nextAttack)
       //     {
       //         anim.SetTrigger("attack");
       //         Debug.Log("普通攻击");
       //
       //         nextAttack = Time.time + attackRate;
       //     }
       // }
       //
       // inAttackRangeLastFrame = inRange;

    }//攻击

    public virtual void SkillAction()
    {

        //Debug.Log("这是炸弹，技能攻击");
        if (Vector2.Distance(transform.position, targetPoint.position) < skillRange)
        {
            if (Time.time > nextAttack)
            {
                // 播放攻击动画
                anim.SetTrigger("skill");
                Debug.Log("普通攻击");
                nextAttack = Time.time + attackRate;
            }
        }

    }// 对炸弹使用技能

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





    #endregion


    /// <summary>
    /// CheckArea视野范围调用
    /// </summary>
    #region

    [Header("视野范围")]
    public EnemyCheckArea checkArea;


    public void OnCheckAreaStay(Collider2D collision)
    {
        if (!attackList.Contains(collision.transform)&&!hasBomb&&!isDead && !GameManager.instance.gameOver) 
        {

            attackList.Add(collision.transform);

        }//只要不是新的，就装进去(如果持有炸弹/自己死亡/玩家死亡，不需要再添加新的进去)


        if (collision.CompareTag("Player") || collision.CompareTag("Bomb"))
        {
            lastKnownTargetPos = collision.transform.position;
        }//记录最后看到的目标位置（搜索状态使用）


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
    int beltIndex = 1;
    int clothesIndex = 1;
    int glovesIndex = 1;
    int pantiesIndex = 1;
    int shoesIndex = 1;
    int skirtIndex = 1;
    int stockingsIndex = 1;

    [Header("武器与攻击方式")]
    public int meleeType;//0空手 1匕首 2武士刀 3尼泊尔军刀
    public int pistolType;//0空手 1柯尔特M1911 2沙鹰手枪 3格洛克手枪
    public int rifleType;//0空手 1步枪M4A1 2步枪AK47
    public int throwType;//0空手 1手榴弹 2烟雾弹 3闪光弹 4飞刀
    public int attackType;//-2步枪射击  -1手枪射击 0踢击 1挥砍



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
           meleeType,
           pistolType,
           rifleType
       );

    }//更新外观

    public void SetWeapon()
    {
        meleeType = Random.Range(1, 4);
        pistolType = Random.Range(1, 4);
        //attackType = Random.Range(-1, 2);

        RefreshPlayerSkin();
    }//捡起的武器调用这里（暂时先不做捡起）


    #endregion



    /// <summary>
    /// 受伤死亡
    /// </summary>
    #region
    [Header("受伤死亡")]
    public Rigidbody2D rb;//我发现这个Enemy居然是transform移动驱动的
    public CapsuleCollider2D coll;
    public FrameEvent_Audio frameEvent_Audio;
    public GameObject Effect_Blood;

  

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



        Character attackerCharacter = attack.owner;//一旦受伤，立刻读取Attack的主人

        bool hitFromBehind = IsHitFromBehind(attack.transform.position);

        // 👉 攻击从背后打来 = 直接死
        if (hitFromBehind)
        {
            frameEvent_Audio._Attack_largeSword();//暂时先把暗杀声音写在这
            OnDie();
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






    public void OnDie() 
    {


        PlayBloodEffect();

        isDead = true;
        gameObject.layer = LayerMask.NameToLayer("Environment");
        CheckArea?.SetActive(false);

        //防止尸体堵在门前
        //coll.enabled = false;
        //rb.bodyType = RigidbodyType2D.Static;

     
       
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
