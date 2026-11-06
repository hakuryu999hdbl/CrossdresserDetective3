using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("组件")]
    public Rigidbody2D rb;

    [Header("碰撞层")]
    public LayerMask hitLayer;   // Enemy
    public LayerMask wallLayer;  // Ground / Map / Wall

    [Header("设置")]
    public bool destroyOnEnemyHit = true;
    public bool destroyOnWallHit = true;

    private float lifeTimer;

    public GameObject SparkEffect;

   
    [Header("掩体与烟雾")]
    [Tooltip("穿过烟雾时关闭伤害。敌人子弹开启，玩家子弹关闭。")]
    public bool disableAttackOnObstacle = false;
    public GameObject Attack; // 子物体 AttackArea
    public LayerMask obstacleLayer;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();
    }

    public void Init(Vector2 direction, float speed, float lifeTime)
    {
        lifeTimer = lifeTime;

        if (rb != null)
        {
            rb.velocity = direction.normalized * speed;
        }

        // 子弹朝向
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;

        if (lifeTimer <= 0f)
        {
            Destroy(gameObject);
        }
    }

    [Header("延后销毁")]
    public float destroyDelay = 0.03f;
    private bool isDestroying;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDestroying) return;


        int hitLayerBit = 1 << collision.gameObject.layer;
        bool hitObstacle = (hitLayerBit & obstacleLayer) != 0;
        if (hitObstacle && collision.TryGetComponent<SmokeArea>(out _))
        {
            if (disableAttackOnObstacle && Attack != null)
                Attack.SetActive(false);

            return;
        }

        if (hitObstacle || (hitLayerBit & wallLayer) != 0)
        {
            if (destroyOnWallHit)
            {
                if (hitObstacle && Attack != null)
                    Attack.SetActive(false);

                GameObject spark = Instantiate(SparkEffect,transform.position, Quaternion.identity);
                Destroy(spark, 1f);

                StartCoroutine(DestroyDelay());
            }

            return;
        }//打到墙上

        if ((hitLayerBit & hitLayer) != 0)
        {
            if (destroyOnEnemyHit)
            {
                StartCoroutine(DestroyDelay());
            }

            return;
        }
    }

    IEnumerator DestroyDelay()
    {
        isDestroying = true;

        if (rb != null)
            rb.velocity = Vector2.zero;

        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}
