using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameEvent : MonoBehaviour
{


    public EnemyController enemyController;

    public void SetOff()
    {
        if (enemyController.targetPoint.GetComponent<Bomb>() != null)
        {
            enemyController.targetPoint.GetComponent<Bomb>().TurnOff();
        }
        
      
    }






    public Transform pickupPoint;

    public void PickUpBomb()
    {
        if (enemyController.targetPoint.CompareTag("Bomb")&&!enemyController.hasBomb)
        {
            //Debug.Log("捡起炸弹");


            enemyController.targetPoint.gameObject.transform.position = pickupPoint.position;
            enemyController.targetPoint.SetParent(pickupPoint);
            enemyController.targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
            enemyController.hasBomb = true;


        }//如果是炸弹，就捡起来移到对应位置（成为子集）,同时刚体为（移动平台）
    }

    public float power;
    public void ThrowAway() 
    {
        if (enemyController.hasBomb) 
        {
            enemyController.targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
            enemyController.targetPoint.SetParent(transform.parent.parent);

            if (FindFirstObjectByType<PlayerController>().gameObject.transform.position.x-transform.position.x<0) 
            {
                enemyController.targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * power, ForceMode2D.Impulse);
            }
            else
            {
                enemyController.targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * power, ForceMode2D.Impulse);
            }
            enemyController.hasBomb = false;

        }//设置为父级的父级（最外面），并刚体重新物理化，丢到玩家方向

    }






    public float scale;


    public void Swalow() 
    {

        enemyController.targetPoint.GetComponent<Bomb>().TurnOff();
        enemyController.targetPoint.gameObject.SetActive(false);

        enemyController.transform.localScale *= scale;//吞下炸弹变大
    }




}
