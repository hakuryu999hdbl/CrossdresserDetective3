using UnityEngine;

public class Catch : MonoBehaviour
{
    public EnemyController enemy;

    private bool hasCaught;//抓住期间不再触发



    private void OnTriggerStay2D(Collider2D other)
    {
        if (enemy == null || hasCaught) return;

        if (enemy.isDead || enemy.isCatching || enemy.IsSuspended)
            return;

        // 受击中的敌人不能通过残留碰撞体抓人
        if (enemy.isDizzy || enemy.currentState == enemy.hitState)
            return;

        if (Time.time < enemy.nextCatchTime) return;
        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        if (player.isDead ||
            player.isHurt ||
            player.isCaptured ||
            player.isStruggling ||
            player.isSlide ||
            player.isTeleporting ||
            player.isInCutscene)
            return;

        enemy.StartCatchPlayer(player);

        // 只有双方成功建立抓取关系，才锁住本碰撞体
        hasCaught = enemy.isCatching &&
                    player.isCaptured &&
                    player.catchingEnemy == enemy;
    }

    public void ResetCatch()
    {
        hasCaught = false;
    }
}