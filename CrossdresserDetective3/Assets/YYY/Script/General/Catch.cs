using UnityEngine;

public class Catch : MonoBehaviour
{
    public EnemyController enemy;

    private bool hasCaught;//抓住期间不再触发



    private void OnTriggerStay2D(Collider2D other)
    {
        if (enemy == null) return;



        if (hasCaught) return;
        if (enemy.isDead) return;
        if (enemy.isCatching) return;
        if (Time.time < enemy.nextCatchTime) return;

        if (!other.CompareTag("Player")) return;

        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) 
        {
            Debug.Log("抓空了！回撤动画！");
            enemy.OnCatchMissCheck();
            return;
        } 


        if (player.isDead || player.isHurt || player.isCaptured || player.isSlide)
        {
            Debug.Log("无法抓住！回撤动画！");
            enemy.OnCatchMissCheck();

            return;
        }
          

        hasCaught = true;
        enemy.StartCatchPlayer(player);
    }

    public void ResetCatch()
    {
        hasCaught = false;
    }
}