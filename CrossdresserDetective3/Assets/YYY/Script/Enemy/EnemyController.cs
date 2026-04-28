using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyBaseState currentState;//当前状态
    public Animator anim;
    public int animState;

    public GameObject alarmSign;

    [Header("基础属性")]
    public float health;
    public bool isDead = false;
    public bool hasBomb;//是否持有炸弹
    public bool isBoss = false;


    [Header("敌人巡逻")]
    public float speed;
    public Transform pointA, pointB;
    public Transform targetPoint;
    public PhysicsCheck physicsCheck;//检测左右有墙翻转

    [Header("敌人攻击")]
    public float attackRate;//攻击冷却
    public float attackRange, skillRange;//攻击范围
    float nextAttack = 0;

    public List<Transform> attackList = new List<Transform>();

    public PatrolState patrolState = new PatrolState();//巡逻状态
    public AttackState attackState = new AttackState();//攻击状态


    public virtual void Init() 
    {
        anim = transform.GetChild(1).GetComponentInChildren<Animator>();//我把敌人动画放下面了第二个物体
        alarmSign = transform.GetChild(0).gameObject;//所有敌人都有这个感叹号标识，抓下面第一个物体

        physicsCheck = GetComponent<PhysicsCheck>();

        rb = GetComponent<Rigidbody2D>();

    }//敌人子类会各自在开始的时候收进父级不需要的东西（虚类）

    private void Awake()
    {
        Init();
    }
    void Start()
    {

        GameManager.instance.IsEnemy(this);

        TransitionToState(patrolState);//一开始进入巡逻状态

        if (isBoss) 
        {
            UIManager.instance.SetBossHealth(health);
        }
    }

    public virtual void Update()
    {
        if (isBoss)
        {
            UIManager.instance.UpdateBossHealth(health);
        }

        anim.SetBool("dead", isDead);
        if (isDead)
        {
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

       
    }

    public void TransitionToState(EnemyBaseState  state) 
    {
        currentState = state;
        currentState.EnterState(this);
    }//切换状态














   
    public bool IsWallAhead()
    {
        if (physicsCheck == null || targetPoint == null) return false;

        if (targetPoint.position.x > transform.position.x)
            return physicsCheck.touchRightWall;

        if (targetPoint.position.x < transform.position.x)
            return physicsCheck.touchLeftWall;

        return false;
    } //巡逻调用切换方向

    public void SwitchToOtherPoint()
    {
        targetPoint = targetPoint == pointA ? pointB : pointA;
    } //巡逻调用切换目标







    public void MoveToTarget()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);
        FilpDirection();
    }//前往目标

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
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }

    }//反转：追逐目标

    public void SwitchPoint()
    {
        if (Mathf.Abs(pointA.position.x - transform.position.x) > Mathf.Abs(pointB.position.x - transform.position.x))
        {
            targetPoint = pointA;
        }
        else
        {
            targetPoint = pointB;
        }
    }//距离哪个点更远目标就是哪个点








    public Transform heldBomb;//扔炸弹方向













    /// <summary>
    /// CheckArea视野范围调用
    /// </summary>
    #region
    public void OnCheckAreaStay(Collider2D collision)
    {
        if (!attackList.Contains(collision.transform)&&!hasBomb&&!isDead && !GameManager.instance.gameOver) 
        {

            attackList.Add(collision.transform);

        }//只要不是新的，就装进去(如果持有炸弹/自己死亡/玩家死亡，不需要再添加新的进去)
      
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
    Transform attacker;
    public bool isHurt = false;
    public Rigidbody2D rb;//我发现这个Enemy居然是transform移动驱动的
    public float hurtForce;

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

        isHurt = true;
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









    public void OnDie() 
    {
        isDead = true;
        //gameObject.layer = 2;//ignoreRaycast
        gameObject.layer = LayerMask.NameToLayer("Enviroment");
    }
    #endregion
}
