using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SmokeBomb : ThrowableEffectBase
{

    public GameObject smokeAreaPrefab;
    public LayerMask targetLayer;
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
        Instantiate(smokeAreaPrefab, transform.position, Quaternion.identity);
        Instantiate(flashFX, transform.position, Quaternion.identity);

        OnBlast?.Invoke(transform);//相机震动
    }

    public void DestoryThis()
    {
        Destroy(gameObject);
    }

}
