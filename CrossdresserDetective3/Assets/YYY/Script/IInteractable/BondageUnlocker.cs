using UnityEngine;

public class BondageUnlocker : MonoBehaviour
{
    private bool pickedUp;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (pickedUp)
            return;

        PlayerController player =
            other.GetComponentInParent<PlayerController>();

        if (player == null || !player.isBondage ||
            player.isCaptured || player.isDead)
            return;

        pickedUp = true;

        player.ExitBondageState();

        // 由 UIManager 清除当前解锁器引用并销毁物体
        UIManager.instance.RemoveBondageUnlocker();


        player.attackType = player.attackTypeBeforeBondage;
        player.isWalking = false;

        player.RefreshPlayerSkin();
        player.RefreshCurrentWeapon();

        player.CancelInvoke(nameof(player.UI_anim_Change));
        player.UI_anim_Change();

        player.playerAnimation.ExitBondageIdle();

        player.frameEvent.BondageUnlocker();//去掉绳子


        GameManager.instance.HideBondageUnlockText();//任务更新
    }
}