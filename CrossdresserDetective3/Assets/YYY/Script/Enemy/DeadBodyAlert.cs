using System.Collections;
using System.Drawing;
using UnityEngine;

public class DeadBodyAlert : MonoBehaviour
{

    [Header("DeadBody_All")]
    public float reactivateTime = 5f;
    public GameObject DeadBody_All;
    private bool coolingDown;

    private void OnTriggerStay2D(Collider2D other)
    {
        EnemyController enemy =
         other.GetComponentInParent<EnemyController>();

        if (enemy == null)
            return;

        if (enemy.isDead)
            return;

        if (coolingDown)
            return;

        Invoke(nameof(_AlertCooldown), 0.2f);
    }

    void _AlertCooldown() 
    {
        StartCoroutine(AlertCooldown());
    }

    public GameObject color;

    private IEnumerator AlertCooldown()
    {
        coolingDown = true;

        // 暂时从敌人视野目标中消失
        DeadBody_All.tag = "Untagged";
        DeadBody_All.layer = LayerMask.NameToLayer("Environment");
        color.SetActive(false);
        yield return new WaitForSeconds(reactivateTime);

        // 再次成为敌人可以发现的目标
        DeadBody_All.tag = "Bomb";
        DeadBody_All.layer = LayerMask.NameToLayer("Bomb");
        color.SetActive(true);
        coolingDown = false;
    }
}