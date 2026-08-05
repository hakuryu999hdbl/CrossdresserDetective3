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
        Throwable      // 增加投掷品
    }

    [Header("道具类型")]
    public PickupType pickupType;

    [Header("增加数量")]
    public int value = 1;
    public FrameEvent_Audio frameEvent_Audio;


    private bool hasPickedUp;

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
        }
    }




    private bool TryPickupHealth(PlayerController player)
    {
        if (player.character.currentHealth== player.character.maxHealth)
            return false;

        player.character.Heal(value);//回复生命值
        return true;
    }

    private bool TryPickupCurrentAmmo(PlayerController player)
    {
        // 没有装备枪械
        if (player.attackType >= 0)
            return false;

        // 当前弹匣已经满了
        if (player.currentAmmo >= player.maxAmmo)
            return false;

        player.ChangeAmmo(value);//增加子弹
        return true;
    }

    private bool TryPickupMagazine(PlayerController player)
    {
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
}
