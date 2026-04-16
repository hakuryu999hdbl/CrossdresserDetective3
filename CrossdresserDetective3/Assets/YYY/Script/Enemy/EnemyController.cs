using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    EnemyBaseState currentState;//当前状态
    public Animator anim;
    public int animState;

    [Header("敌人巡逻")]
    public float speed;
    public Transform pointA, pointB;
    public Transform targetPoint;


    [Header("敌人攻击")]
    public float attackRate;//攻击频率
    public float attackRange, skillRange;
    float nextAttack = 0;

    public List<Transform> attackList = new List<Transform>();

    public PatrolState patrolState = new PatrolState();//巡逻状态
    public AttackState attackState = new AttackState();//攻击状态


    public virtual void Init() 
    {
        anim = GetComponentInChildren<Animator>();//我把动画放下面了

    }//敌人子类会各自在开始的时候收进父级不需要的东西（虚类）

    private void Awake()
    {
        Init();
    }
    void Start()
    {
        TransitionToState(patrolState);//一开始进入巡逻状态
    }

    void Update()
    {

        currentState.OnUpdate(this);//每帧执行状态
        anim.SetInteger("state", animState);
    }

    public void TransitionToState(EnemyBaseState  state) 
    {
        currentState = state;
        currentState.EnterState(this);
    }//切换状态





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
                nextAttack = Time.time + attackRange;
            }
        }



    }//攻击

    public void SkillAction() 
    {

        Debug.Log("这是炸弹，技能攻击");
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






    //CheckArea调用
    public void OnCheckAreaEnter(Collider2D collision)
    {
        if (!attackList.Contains(collision.transform)) 
        {
            attackList.Add(collision.transform);
        }//只要不是新的，就装进去
      
    }//只要持续处于范围之中
    public void OnCheckAreaExit(Collider2D collision)
    {
        attackList.Remove(collision.transform);
    }//离开视野范围
}
