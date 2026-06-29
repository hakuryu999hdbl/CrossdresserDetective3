using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Bomb :  ThrowableEffectBase, IDamageable
{

    private Animator anim;

    public float startTime;
    public float waitTime;
    public float bombForce;
    

    Collider2D coll;
    Rigidbody2D rb;

    [Header("Check")]
    public float radius;
    public LayerMask targetLayer;

    [Header("初始是否为易燃易爆物品")]
    public bool isTurnOff = false;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        startTime = Time.time;

        if (isTurnOff)
        {
            TurnOff();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("bomb_off"))
        {
            if (Time.time > startTime + waitTime)
            {
                anim.Play("bomb_explotion");
            }
        }//炸弹吹灭了就不继续


      
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    public GameObject Blast;

    public void Explotion()//动画帧事件调用，爆炸检测层把对应物体推出
    {
        Vector3 spawnPos = transform.position + new Vector3(0, 2f, 0);
        Instantiate(Blast, spawnPos, Quaternion.identity);
        //Instantiate(Blast, transform.position, Quaternion.identity);





       // coll.enabled = false;//防止自己被炸到

        Collider2D[] aroundObjects = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);

       // rb.gravityScale = 0;//防止掉出屏幕

        foreach (var item in aroundObjects)
        {


           

            EnemyController enemy = item.GetComponent<EnemyController>();

            if (enemy != null)
            {
                enemy.OnBlastHit(transform.position, bombForce);
            }
            else
            {
                Rigidbody2D itemRb = item.GetComponent<Rigidbody2D>();
                if (itemRb != null)
                {
                    Vector3 pos = transform.position - item.transform.position;
                    itemRb.AddForce((-pos + Vector3.up) * bombForce, ForceMode2D.Impulse);
                }
            }

            //Vector3 pos = transform.position - item.transform.position;
            //
            //item.GetComponent<Rigidbody2D>().AddForce((-pos + Vector3.up) * bombForce, ForceMode2D.Impulse);//炸飞


            //触发可掉落物体
            FallOnExplosion fall = item.GetComponent<FallOnExplosion>();
            if (fall != null)
            {
                fall.OnBlastHit(transform.position);
            }

            if (item.CompareTag("Bomb"))
            {
                Animator anim = item.GetComponent<Animator>();
                Bomb bomb = item.GetComponent<Bomb>();

                if (anim != null &&
                    bomb != null &&
                    anim.GetCurrentAnimatorStateInfo(0).IsName("bomb_off"))
                {
                    bomb.TurnOn();
                }
            }//重新点燃爆炸物
        }

        OnBlast?.Invoke(transform);//相机震动
    }

    public void DestoryThis() 
    {
        Destroy(gameObject);

    }
    public UnityEvent<Transform> OnBlast;//爆炸抖动相机

    public void TurnOff() 
    {
        anim.Play("bomb_off");
        gameObject.layer = LayerMask.NameToLayer("NPC");
    }

    public void TurnOn()
    {
        anim.Play("bomb_on");
        gameObject.layer = LayerMask.NameToLayer("Bomb");
        startTime = Time.time;
    }

  

    public void TakeDamage(Attack attack)
    {
        if (attack == null) return;

        // 只有允许点燃炸弹的攻击才能触发
        if (!attack.TurnOnBomb) return;

        // 已经点燃就不用重复点
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("bomb_off")) return;



        TurnOn();
    }//子弹引燃
}
