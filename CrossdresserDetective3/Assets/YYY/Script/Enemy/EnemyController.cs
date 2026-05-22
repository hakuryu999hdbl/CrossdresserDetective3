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

        GameManager.instance.IsEnemy(this);

        TransitionToState(patrolState);//一开始进入巡逻状态

        RandomizeZ();
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



            GameManager.instance.EnemyDead(this);//死后移除列表
           
            return;
        }




        if (GameManager.instance != null && GameManager.instance.gameOver)
        {
            attackList.Clear();
            targetPoint = null;
            anim.ResetTrigger("attack");
            anim.ResetTrigger("skill");
            animState = 0;
            anim.SetInteger("state", animState);
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
                anim.SetTrigger("attack");
                Debug.Log("普通攻击");
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




    /// <summary>
    /// 受伤死亡
    /// </summary>
    #region
    [Header("受伤死亡")]
    //Transform attacker;
    //public bool isHurt = false;
    //public float hurtForce=4.5f;
    public Rigidbody2D rb;//我发现这个Enemy居然是transform移动驱动的
    public CapsuleCollider2D coll;
    public FrameEvent_Audio frameEvent_Audio;
    public GameObject Effect_Blood;

    #region 旧击退
    // public void OnTakeDamage(Transform attackTrans) 
    // {
    //
    //     Debug.Log("敌人受伤");
    //
    //     attacker = attackTrans;
    //
    //     //转身
    //     //if (attackTrans.position.x - transform.position.x > 0) 
    //     //{
    //     //    transform.localScale = new Vector3(1,1,1);
    //     //}
    //     //if (attackTrans.position.x - transform.position.x < 0)
    //     //{
    //     //    transform.localScale = new Vector3(-1, 1, 1);
    //     //}
    //
    //     //受伤被击退
    //     isHurt = true;//主要用于停止移动
    //     anim.SetTrigger("hit");
    //
    //
    //     rb.velocity = Vector2.zero;
    //     Vector2 dir = new Vector2((transform.position.x - attacker.position.x), 0).normalized;
    //     rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    //
    //     isHurt = false;//主要用于停止移动
    //
    //
    //
    //
    //     // 如果之前有协程，先停掉（保险）
    //     if (hurtCoroutine != null)
    //     {
    //         StopCoroutine(hurtCoroutine);
    //     }
    //     
    //     hurtCoroutine = StartCoroutine(OnHurt(dir));
    // }
    // private Coroutine hurtCoroutine;
    //
    // private IEnumerator OnHurt(Vector2 dir)
    // {
    //     // 清空当前速度（防止叠加）
    //     rb.velocity = Vector2.zero;
    //
    //     // 击退
    //     rb.AddForce(dir * hurtForce, ForceMode2D.Impulse);
    //
    //     // 硬直时间
    //     yield return new WaitForSeconds(0.45f);
    //
    //     isHurt = false;
    // }

    #endregion



    public void OnTakeDamage(Attack attack)
    {

        if (attack == null)
            return;



        // 👉 巡逻状态被偷袭 = 直接死
        if (currentState == patrolState)
        {
            OnDie();
            return;
        }



        //一旦受伤立刻把Attack的根物体的character所在物体立为目标
        Character attackerCharacter = attack.GetComponentInParent<Character>();

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
