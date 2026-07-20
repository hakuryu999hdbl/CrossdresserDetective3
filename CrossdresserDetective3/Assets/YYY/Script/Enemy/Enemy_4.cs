using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy_4 : EnemyController
{
    [Header("格挡系统")]
    public float maxBlockValue = 100f;
    public float currentBlockValue = 100f;

    [Tooltip("每次受到攻击消耗的基础格挡值")]
    public float blockCostMultiplier = 35f;



    [Header("格挡UI")]
    public GameObject blockBarRoot;
    public Image blockBarFill;

    [Header("格挡状态")]
    public bool isBlocking;
    public bool isCountering;

    public GameObject SparkEffect;//格挡火花

    public override void Init()
    {
        base.Init();

        currentBlockValue = maxBlockValue;
        UpdateBlockUI();
    }

    public override void Update()
    {
        base.Update();

        UpdateBlockRecovery();
    }

    public override bool TryHandleIncomingAttack(Attack attack)
    {
        if (attack == null)
        {
            //Debug.Log("格挡失败：attack为空");
            return false;
        }

        if (isDead)
            return false;


        // 眩晕期间不能防御、不能反击
        if (isDizzy)
            return false;

        //防御火花
        GameObject spark = Instantiate(SparkEffect, transform.position, Quaternion.identity);
        Destroy(spark, 1f);


        // 新增：格挡之前先记录攻击者
        AcquireAttackerAsTarget(attack);

        currentBlockValue -= attack.damage;
        currentBlockValue = Mathf.Max(0f, currentBlockValue);

        UpdateBlockUI();

        if (currentBlockValue <= 0f)
        {
            isBlocking = false;
            isCountering = false;

            anim.SetInteger("skillState", 0);

            return false;
        }

       

        TransitionToState(blockState);
        return true;
    }

    private void AcquireAttackerAsTarget(Attack attack)
    {
        if (attack == null)
            return;

        Transform attacker = null;

        // 优先读取子弹、近战攻击的主人
        if (attack.owner != null)
        {
            attacker = attack.owner.transform;
        }
        else
        {
            // 没有 owner 时，退回攻击物自身的位置
            attacker = attack.transform;
        }

        if (attacker == null)
            return;

        targetPoint = attacker;
        lastKnownTargetPos = attacker.position;

        if (!attackList.Contains(attacker))
        {
            attackList.Add(attacker);
        }

        FaceToPosition(attacker.position);
    }//受到远程攻击后

    private void UpdateBlockRecovery()
    {
        if (isDead )
            return;

        if (isBlocking || isCountering)
            return;



        if (currentBlockValue >= maxBlockValue)
        {
            currentBlockValue = maxBlockValue;

            UpdateBlockUI();
            return;
        }



        currentBlockValue =
            Mathf.Min(currentBlockValue, maxBlockValue);


        UpdateBlockUI();
    }

    private void UpdateBlockUI()
    {
        blockBarFill.fillAmount =
                 currentBlockValue / maxBlockValue;

        if (currentBlockValue <= 0f)
        {
            blockBarRoot.SetActive(false);
            return;

        }//只要没防御就隐藏

        blockBarRoot.SetActive(
                 currentBlockValue < maxBlockValue ||
                 isBlocking
             );
    }

    // 防御动画事件：
    // 举起武器后开始反击
    public void StartCounterAttack()
    {
        if (isDead )
            return;

        rb.velocity = Vector2.zero;

        isBlocking = false;
        isCountering = true;


        animState = 4;
        anim.SetInteger("skillState", 2);

        if (targetPoint != null)
            FaceToPosition(targetPoint.position);
    }

    // 反击动画结束事件
    public void EndCounterAttack()
    {
        isBlocking = false;
        isCountering = false;

        anim.SetInteger("skillState", 0);

        if (isDead)
            return;

        if (attackList.Count > 0)
        {
            EnterBattleState();
        }
        else
        {
            TransitionToState(patrolState);
        }
    }
}
