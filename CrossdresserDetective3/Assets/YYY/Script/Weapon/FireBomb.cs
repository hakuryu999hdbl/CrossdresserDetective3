using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireBomb : ThrowableEffectBase
{
    [Header("爆开")]
    public float radius = 3f;
    public float bombForce = 5f;
    public LayerMask targetLayer;//推飞敌人层
    public GameObject fireSparkPrefab;//产生火焰火星

    public UnityEvent<Transform> OnBlast;//爆炸抖动相机

    [Header("火星散射")]
    public int fireSparkCount = 12;
    public float minForce = 4f;
    public float maxForce = 30f;
    public float spreadX = 10f;
    public float spawnRandomRadius = 0.3f;




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
        //Debug.Log("爆开");

        PushTargets();//像炸弹一样把周边东西推出去

        for (int i = 0; i < fireSparkCount; i++)
        {
            Vector2 spawnOffset = Random.insideUnitCircle * spawnRandomRadius;

            GameObject spark = Instantiate(
                fireSparkPrefab,
                (Vector2)transform.position + spawnOffset,
                Quaternion.identity
            );

            Rigidbody2D rb = spark.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                float x = Random.Range(-spreadX, spreadX);
                float y = Random.Range(0.4f, 1.2f);

                Vector2 dir = new Vector2(x, y).normalized;
                float force = Random.Range(minForce, maxForce);

                rb.velocity = dir * force;
            }
        }

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
