using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameEvent : MonoBehaviour
{


    public EnemyController enemyController;


    #region 吹灭炸弹
    public void SetOff()
    {

        if (enemyController == null)
            return;

        if (enemyController.targetPoint == null)
            return;

        Bomb bomb = enemyController.targetPoint.GetComponent<Bomb>();

        if (bomb == null)
            return;

        bomb.TurnOff();

        enemyController.attackList.Remove(enemyController.targetPoint);
        enemyController.targetPoint = null;


        //if (enemyController.targetPoint.GetComponent<Bomb>() != null)
        //{
        //    enemyController.targetPoint.GetComponent<Bomb>().TurnOff();
        //}


    }
    #endregion


    #region 丢掉炸弹
    [Header("丢掉炸弹")]
    public Transform pickupPoint;

    public void PickUpBomb()
    {
        if (enemyController == null) return;
        if (enemyController.targetPoint == null) return;
        if (enemyController.hasBomb) return;

        if (enemyController.targetPoint.CompareTag("Bomb"))
        {
            Transform bomb = enemyController.targetPoint;

            bomb.position = pickupPoint.position;
            bomb.SetParent(pickupPoint);

            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = Vector2.zero;
            }

            enemyController.heldBomb = bomb;
            enemyController.hasBomb = true;
        }




        // 🔴 第一层挡板：targetPoint 已经被销毁
       //if (enemyController.targetPoint == null) return;
       //
       //// 🔴 第二层挡板：已经拿了炸弹
       //if (enemyController.hasBomb) return;
       //
       //if (enemyController.targetPoint.CompareTag("Bomb")&&!enemyController.hasBomb)
       //{
       //    //Debug.Log("捡起炸弹");
       //
       //
       //    enemyController.targetPoint.gameObject.transform.position = pickupPoint.position;
       //    enemyController.targetPoint.SetParent(pickupPoint);
       //    enemyController.targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
       //    enemyController.hasBomb = true;
       //
       //
       //}//如果是炸弹，就捡起来移到对应位置（成为子集）,同时刚体为（移动平台）
    }

    public float power;
    public void ThrowAway() 
    {

        if (enemyController == null) return;
        if (!enemyController.hasBomb) return;
        if (enemyController.heldBomb == null)
        {
            enemyController.hasBomb = false;
            return;
        }

        Transform bomb = enemyController.heldBomb;
        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            enemyController.hasBomb = false;
            enemyController.heldBomb = null;
            return;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        bomb.SetParent(transform.parent.parent);

        PlayerController player = FindFirstObjectByType<PlayerController>();

        float dir = 1f;
        if (player != null)
        {
            dir = player.transform.position.x - transform.position.x < 0 ? -1f : 1f;
        }

        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(dir, 1f) * power, ForceMode2D.Impulse);

        enemyController.hasBomb = false;
        enemyController.heldBomb = null;


        //if (enemyController.hasBomb) 
        //{
        //    enemyController.targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        //    enemyController.targetPoint.SetParent(transform.parent.parent);
        //
        //    if (FindFirstObjectByType<PlayerController>().gameObject.transform.position.x-transform.position.x<0) 
        //    {
        //        enemyController.targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * power, ForceMode2D.Impulse);
        //    }
        //    else
        //    {
        //        enemyController.targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * power, ForceMode2D.Impulse);
        //    }
        //    enemyController.hasBomb = false;
        //
        //}//设置为父级的父级（最外面），并刚体重新物理化，丢到玩家方向

    }

    #endregion


    #region 吞下炸弹
    [Header("吞下炸弹")]
    public float scale;
    public void Swalow() 
    {

        /////////挡板
        if (enemyController == null)
            return;

        if (enemyController.targetPoint == null)
            return;

        Bomb bomb = enemyController.targetPoint.GetComponent<Bomb>();
        if (bomb == null)
            return;
        /////////挡板



        enemyController.targetPoint.GetComponent<Bomb>().TurnOff();
        enemyController.targetPoint.gameObject.SetActive(false);


        /////////吞下炸弹目标清空
        enemyController.targetPoint = null;
        enemyController.attackList.Clear();
        /////////





        enemyController.transform.localScale *= scale;//吞下炸弹变大
    }
    #endregion


    #region 爆炸/野猪死后消灭本体
    [Header("死后消灭本体")]
    public GameObject TargetDestory;
    public void DestroyAfterAnimation()
    {
        Destroy(TargetDestory);
    }
    #endregion


    #region 攻击动画触发
    [Header("攻击动画触发")]
    public GameObject AttackArea_1;
    public GameObject AttackArea_2;
    public GameObject AttackArea_3;
    public void Attack_1()
    {
        StartCoroutine(AttackRoutine(AttackArea_1));
    }
    public void Attack_2()
    {
        StartCoroutine(AttackRoutine(AttackArea_2));
    }
    public void Attack_3()
    {
        StartCoroutine(AttackRoutine(AttackArea_3));
    }

    IEnumerator AttackRoutine(GameObject area)
    {
        if (area == null) yield break;

        area.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        area.SetActive(false);
    }
    #endregion

    #region 落地特效触发
    [Header("落地特效触发")]
    public PlayerController playerController;

    public void LandFX()
    {
        //跳落第一帧触发
        playerController.LandFX();
    }
    #endregion
}
