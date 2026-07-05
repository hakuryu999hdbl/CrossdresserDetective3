using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SmokeBomb : ThrowableEffectBase
{
    [Header("爆开")]
    public float radius = 3f;
    public float bombForce = 5f;
    public LayerMask targetLayer;//推出敌人


    public GameObject smokeAreaPrefab;

    public GameObject flashFX;//产生火花

    public UnityEvent<Transform> OnBlast;//爆炸抖动相机

    Animator anim;
    Collider2D coll;
    Rigidbody2D rb;

    [Header("计时")]
    public float startTime;
    public float waitTime = 1.5f;
    private bool hasExploded;

    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
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
        PushTargets();

        GameObject smoke = Instantiate(smokeAreaPrefab, transform.position, Quaternion.identity);

        Destroy(smoke, 10f);

        Instantiate(flashFX, transform.position, Quaternion.identity);

        OnBlast?.Invoke(transform);//相机震动
    }

    void PushTargets()
    {
        Collider2D[] aroundObjects = Physics2D.OverlapCircleAll(
            transform.position,
            radius,
            targetLayer
        );

        foreach (var item in aroundObjects)
        {

            //摇晃垂下物体
            SwingOnBlast swing = item.GetComponentInParent<SwingOnBlast>();
            if (swing != null)
            {
                swing.OnBlastHit(transform.position);
            }

            //触发可掉落物体
            FallOnExplosion fall = item.GetComponent<FallOnExplosion>();
            if (fall != null)
            {
                fall.OnBlastHit(transform.position);
            }

            //推远物体
            Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
            if (itemRb == null) continue;

            Vector3 pos = transform.position - item.transform.position;
            itemRb.AddForce((-pos + Vector3.up) * bombForce, ForceMode2D.Impulse);
        }
    }

    public void DestoryThis()
    {
        Destroy(gameObject);
    }

}
