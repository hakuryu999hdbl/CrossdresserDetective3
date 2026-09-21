using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : MonoBehaviour
{

    public enum PickupType
    {
        Health,        // 回血
        CurrentAmmo,   // 当前弹匣子弹
        Magazine,      // 增加备用弹匣
        Throwable,     // 增加投掷品
        Money          // 金钱
    }

    [Header("道具类型")]
    public PickupType pickupType;

    [Header("增加数量")]
    public int value = 1;
    public FrameEvent_Audio frameEvent_Audio;


    private bool hasPickedUp;


    void Start() 
    {
        PlayerController player = GameManager.instance.player;

        if (player == null)
            return;

        rb = GetComponent<Rigidbody2D>();

        switch (pickupType)
        {
            // 子弹：没枪就销毁
            case PickupType.CurrentAmmo:
                if (player.attackType >= 0)
                {
                    Destroy(gameObject);
                }
                break;

            // 弹夹：没枪就销毁
            case PickupType.Magazine:
                if (player.attackType >= 0)
                {
                    Destroy(gameObject);
                }
                break;

            // 投掷补充：没装备投掷品就销毁
            case PickupType.Throwable:
                if (player.throwType == 0)
                {
                    Destroy(gameObject);
                }
                break;

            // 金钱：掉落2秒后自动飞向玩家
            case PickupType.Money:
                StartCoroutine(StartMoneyFly());
                break;
        }
    }



    private void OnTriggerStay2D(Collider2D collision)
    {
        if (hasPickedUp)
            return;

        PlayerController player =
            collision.GetComponentInParent<PlayerController>();

        if (player == null)
            return;

        TryPickup(player);
    }

    private void TryPickup(PlayerController player)
    {
        bool pickupSucceeded = false;

        switch (pickupType)
        {
            case PickupType.Health:
                pickupSucceeded = TryPickupHealth(player);
                break;

            case PickupType.CurrentAmmo:
                pickupSucceeded = TryPickupCurrentAmmo(player);
                break;

            case PickupType.Magazine:
                pickupSucceeded = TryPickupMagazine(player);
                break;

            case PickupType.Throwable:
                pickupSucceeded = TryPickupThrowable(player);
                break;

            case PickupType.Money:
                pickupSucceeded = TryPickupMoney();
                break;
        }

        if (!pickupSucceeded)
            return;

        hasPickedUp = true; // 先锁住，避免同一物理帧被多个碰撞体重复触发


        PlayPickupSound();


        Destroy(gameObject);
    }


    private void PlayPickupSound()
    {
        if (AudioManager.Instance == null)
            return;

        switch (pickupType)
        {
            case PickupType.Health:
                AudioManager.Instance.PlayFX(
                    AudioManager.Instance.SE_Yanxia
                );
                break;

            case PickupType.CurrentAmmo:
                AudioManager.Instance.PlayFX(
                    AudioManager.Instance.Bullet_SD_Load
                );
                break;

            case PickupType.Magazine:
                AudioManager.Instance.PlayFX(
                    AudioManager.Instance.Bullet_Pistol_Load
                );
                break;

            case PickupType.Throwable:
                AudioManager.Instance.PlayFX(
                    AudioManager.Instance.Bullet_OutOfBullet
                );
                break;

            case PickupType.Money:
                AudioManager.Instance.PlayFX(
                    AudioManager.Instance.SE_Reji_1
                );
                break;
        }
    }




    private bool TryPickupHealth(PlayerController player)
    {
       //满血也能捡起

       // if (player.character.currentHealth== player.character.maxHealth)
       //     return false;

        player.character.Heal(value);//回复生命值
        return true;
    }

    private bool TryPickupCurrentAmmo(PlayerController player)
    {
        // 没有装备枪械
        if (player.attackType >= 0)
            return false;

        // 当前弹匣已经满了，所以变成增加一个弹夹
        if (player.currentAmmo >= player.maxAmmo)
        {
            player.AddMagazine(1);
            return true;
        }


        player.ChangeAmmo(value);//增加子弹
        return true;
    }

    private bool TryPickupMagazine(PlayerController player)
    {
        // 没有装备枪械
        if (player.attackType >= 0)
            return false;

        // 可以根据需要增加备用弹匣上限判断
        player.AddMagazine(value);//增加弹夹
        return true;
    }

    private bool TryPickupThrowable(PlayerController player)
    {
        if (player.throwCount >= player.maxThrowCount)
            return false;

        player.AddThrowCount(value);//增加投掷品数量
        return true;
    }

    private bool TryPickupMoney()
    {
        if (BalanceManager.instance == null)
            return false;

        BalanceManager.instance.ChangeMoney(value);

        return true;
    }









    [Header("金钱自动吸附")]
    public float moneyFlyDelay = 2f;
    public float moneyFlySpeed = 10f;

    private PlayerController targetPlayer;
    private bool isFlyingToPlayer;
    private Rigidbody2D rb;

    private IEnumerator StartMoneyFly()
    {
        yield return new WaitForSeconds(moneyFlyDelay);

        if (hasPickedUp)
            yield break;

        // 玩家可能在生成金钱时还没取得
        if (targetPlayer == null && GameManager.instance != null)
        {
            targetPlayer = GameManager.instance.player;
        }

        if (targetPlayer == null)
            yield break;

        // 开始吸附后不再受重力影响
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0f;
        }

        isFlyingToPlayer = true;
    }

    private void Update()
    {
        if (!isFlyingToPlayer || hasPickedUp || targetPlayer == null)
            return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPlayer.transform.position,
            moneyFlySpeed * Time.deltaTime
        );

        // 足够接近后直接拾取，不依赖碰撞器
        if (Vector2.Distance(
            transform.position,
            targetPlayer.transform.position
        ) <= 0.15f)
        {
            TryPickup(targetPlayer);
        }
    }
}
