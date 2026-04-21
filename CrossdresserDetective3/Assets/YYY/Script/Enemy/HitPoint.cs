using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour
{
    int dir;//弹炸弹方向
    public bool bombAvilable;//是否可以弹

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (transform.position.x > other.transform.position.x)
        {
            dir = -1;//如果在右侧，往左边弹
        }
        else 
        {
            dir = 1;
        }


        if (other.CompareTag("Player"))
        {
            other.GetComponent<IDamageable>().GetHit(1);

            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1) * 10, ForceMode2D.Impulse);//击飞
        }

        if (other.CompareTag("Bomb")&& bombAvilable)
        {
            other.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(dir, 1) * 10, ForceMode2D.Impulse);//击飞
        }
    }
}
