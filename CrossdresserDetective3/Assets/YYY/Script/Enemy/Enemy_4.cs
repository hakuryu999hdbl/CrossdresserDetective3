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
    [SerializeField] private AudioClip blockBreakSound;

    private const float BlockBreakDuration = 0.35f;
    private Image blockBarFrame;
    private Color blockBarFrameColor;
    private Vector3 blockBarScale;
    private float blockBreakStartTime;
    private bool isPlayingBlockBreak;

    [Header("格挡状态")]
    public bool isBlocking;
    public bool isCountering;

    public GameObject SparkEffect;//格挡火花

    public void SetSpark()
    {
        //防御火花
        GameObject spark = Instantiate(SparkEffect, transform.position, Quaternion.identity);
        Destroy(spark, 1f);

    }



    public override void Init()
    {
        base.Init();

        ResetBlockBreakFeedback();
        currentBlockValue = maxBlockValue;
        UpdateBlockUI();
    }

    public override void Update()
    {
        UpdateBlockBreakFeedback();
        if (IsSuspended) return;
        base.Update();

        UpdateBlockRecovery();
    }

    public override bool TryHandleIncomingAttack(Attack attack)
    {
        if (IsSuspended) return true;
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

   

        // 新增：格挡之前先记录攻击者
        AcquireAttackerAsTarget(attack);

        bool hadBlock = currentBlockValue > 0f;
        currentBlockValue = attack.IsPlayerKatanaStrike
            ? 0f
            : Mathf.Max(0f, currentBlockValue - attack.damage);

        if (hadBlock && currentBlockValue <= 0f)
            PlayBlockBreakFeedback();

        UpdateBlockUI();

        if (currentBlockValue <= 0f)
        {
            isBlocking = false;
            isCountering = false;

            anim.SetInteger("skillState", 0);

            return false;
        }

        // 已经处于防御或反击过程
        // 只扣格挡值，不要重新进入BlockState、不要重播动画
        if (isBlocking || isCountering)
        {
            return true;
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
        if (isPlayingBlockBreak)
            return;

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

    private void PlayBlockBreakFeedback()
    {
        if (blockBreakSound != null && frameEvent_Audio != null && frameEvent_Audio.audioS != null)
            frameEvent_Audio.PlayClip(blockBreakSound);

        if (blockBarRoot == null || !blockBarRoot.TryGetComponent(out blockBarFrame))
            return;

        blockBarFrameColor = blockBarFrame.color;
        blockBarScale = blockBarRoot.transform.localScale;
        blockBreakStartTime = Time.time;
        isPlayingBlockBreak = true;

        blockBarFill.fillAmount = 0f;
        blockBarFrame.color = Color.white;
        blockBarRoot.SetActive(true);
    }

    private void UpdateBlockBreakFeedback()
    {
        if (!isPlayingBlockBreak)
            return;

        float progress = (Time.time - blockBreakStartTime) / BlockBreakDuration;
        if (progress >= 1f)
        {
            ResetBlockBreakFeedback();
            UpdateBlockUI();
            return;
        }

        float fade = Mathf.InverseLerp(0.2f, 1f, progress);
        Color color = Color.Lerp(Color.white, new Color(1f, 0.4f, 0.1f), fade);
        color.a = 1f - fade;
        blockBarFrame.color = color;
        blockBarRoot.transform.localScale = Vector3.Scale(
            blockBarScale, new Vector3(1f - 0.35f * fade, 1f - 0.85f * fade, 1f));
    }

    private void ResetBlockBreakFeedback()
    {
        if (!isPlayingBlockBreak)
            return;

        isPlayingBlockBreak = false;
        blockBarRoot.SetActive(false);
        blockBarRoot.transform.localScale = blockBarScale;
        blockBarFrame.color = blockBarFrameColor;
    }

    // 防御动画事件：
    // 举起武器后开始反击
    public void StartCounterAttack()
    {
        if (IsSuspended) return;
        if (isDead )
            return;

        if (isCountering)
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
        if (IsSuspended) return;
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
            // 看不到玩家 → 去最后已知位置搜索
            TransitionToState(searchState);
        }
    }


    public override void OnDie()
    {
        if (isDead)
            return;

        // 破防和死亡发生在同一击时，保留破防提示。
        if (blockBarRoot != null && !isPlayingBlockBreak)
        {
            blockBarRoot.SetActive(false);
        }

        isBlocking = false;
        isCountering = false;

        // 再执行父类死亡流程
        base.OnDie();
    }


    public override bool CanBeAssassinated()
    {
        // 防御值没破：即使巡逻也不能暗杀
        if (currentBlockValue > 0f)
            return false;

        // 破防以后：遵守普通敌人的暗杀规则
        return base.CanBeAssassinated();
    }
}
