using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShockBomb : ThrowableEffectBase
{
    [Header("范围")]
    float radius = 20f;
    float stunTime = 2f;
    float bombForce = 2f;
    public LayerMask targetLayer;
    public GameObject shockFX;

    public UnityEvent<Transform> OnBlast;

    Animator anim;
    public float startTime;
    float waitTime = 1.5f;
    private bool hasExploded;

    void Start()
    {
        anim = GetComponent<Animator>();
        startTime = Time.time;
    }

    void Update()
    {
        if (hasExploded) return;

        if (Time.time > startTime + waitTime)
        {
            hasExploded = true;
            anim.Play("bomb_explotion");
        }
    }

    public void Explotion()
    {
       
        Instantiate(shockFX, transform.position, Quaternion.identity);
        UIManager.instance.ShockScreen.SetActive(true);

        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);

        foreach (var item in targets)
        {

            //摇晃垂下物体
            SwingOnBlast swing = item.GetComponentInParent<SwingOnBlast>();
            if (swing != null)
            {
                swing.OnBlastHit(transform.position);
            }

            //推远敌人
            EnemyController enemy = item.GetComponent<EnemyController>();
            if (enemy != null)
            {

                enemy.Shock(stunTime);
            }

            int layer = item.gameObject.layer;

            if (layer == LayerMask.NameToLayer("Bomb") ||
                layer == LayerMask.NameToLayer("Environment"))
            {
                Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();

                if (itemRb != null)
                {
                    Vector3 pos = transform.position - item.transform.position;
                    itemRb.AddForce((-pos + Vector3.up) * bombForce, ForceMode2D.Impulse);
                }
            }
        }

        OnBlast?.Invoke(transform);
    }

    public void DestoryThis()
    {
        Destroy(gameObject);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
