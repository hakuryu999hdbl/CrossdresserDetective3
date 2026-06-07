using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public Character owner;

    [Header("伤害数值")]
    public int damage;
    //public float attackRange;
    //public float attackRate;

    [Header("击飞参数")]
    public float knockbackX = 5f;
    public float knockbackY = 0f;
    public float hurtTime = 0.2f;
    public bool clearVelocity = true;

    [Header("击飞炸弹")]
    public bool bombAvailable;
    int dir;//弹炸弹方向

    private HashSet<IDamageable> hitTargets = new HashSet<IDamageable>();


    private void OnEnable()
    {
        hitTargets.Clear();
    }
   
    private void OnTriggerStay2D(Collider2D other)
    {
        IDamageable target = other.GetComponentInParent<IDamageable>();

        if (target != null && !hitTargets.Contains(target))
        {
            hitTargets.Add(target);
            target.TakeDamage(this);
        }

        // 炸弹不走 IDamageable，暂时单独处理
        if (bombAvailable && other.CompareTag("Bomb"))
        {
        
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float dir = transform.position.x > other.transform.position.x ? -1f : 1f;
            
                if (clearVelocity)
                {
                    rb.velocity = Vector2.zero;
                }
            
                rb.AddForce(
                    new Vector2(dir * knockbackX, knockbackY),
                    ForceMode2D.Impulse
                );
            }
        }
    }

   
}

