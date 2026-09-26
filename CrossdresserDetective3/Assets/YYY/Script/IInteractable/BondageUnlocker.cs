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

        // ﾓﾉ UIManager ﾇ蟲ｵｱﾇｰｽ簍ﾓﾃｲ｢ﾏ忞ﾙﾎ・・
        UIManager.instance.RemoveBondageUnlocker();


        player.attackType = player.attackTypeBeforeBondage;
        player.isWalking = false;

        player.RefreshPlayerSkin();
        player.RefreshCurrentWeapon();

        player.CancelInvoke(nameof(player.UI_anim_Change));
        player.UI_anim_Change();

        player.playerAnimation.ExitBondageIdle();

        player.frameEvent.BondageUnlocker();//ﾈ･ｵﾗﾓ
        player.frameEvent_Audio._Voice_StopLoop();//循环音关闭

        GameManager.instance.HideBondageUnlockText();//ﾈﾎﾎ・ﾂ
    }
}