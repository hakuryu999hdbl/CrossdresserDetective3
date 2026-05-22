using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, IDamageable
{
    [Header("数值")]
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;
    public bool StopPowerRecover=false;//在墙上不能回复体力

    [Header("受伤无敌")]
    public float invulnerableDuration;//无敌时长
    float invulnerableCounter;//计数器
    public bool invulnerable;//是否无敌

    [Header("受伤击退死亡")]
    public UnityEvent<Character> OnHealthChange;//只要生命值有一点改变，就把Character广播出去到ScriptObject
    public UnityEvent<Transform> OnTakeDamge;
    public UnityEvent OnDie;
    public bool isDead = false;



    private void Start()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        //传输Character过去
        OnHealthChange?.Invoke(this);

    }

    private void Update()
    {
        if (invulnerable) 
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }



        if (currentPower < maxPower)
        {
            if (!StopPowerRecover)
            {
                currentPower += powerRecoverSpeed * Time.deltaTime;
                currentPower = Mathf.Clamp(currentPower, 0, maxPower);

                OnHealthChange?.Invoke(this);
            }
        }//恢复体力值
    }


    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("DieZone"))
        {
            //死亡，更新血量
            currentHealth = 0;
            OnHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (isDead) { return; }//不能鞭尸
        if (invulnerable) { return; }//处于无敌


        if (currentHealth - attacker.damage > 0) 
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();//打开无敌时间

            //受伤执行击退事件
            OnTakeDamge?.Invoke(attacker.transform);
            GetComponent<PlayerController>()?.OnTakeDamage(attacker); // 玩家直接击退
            GetComponent<EnemyController>()?.OnTakeDamage(attacker); // 敌人直接击退


        }
        else
        {
            currentHealth = 0;

            //触发死亡
            OnDie?.Invoke();

            isDead = true;
        }



        //传输Character过去
        OnHealthChange?.Invoke(this);

    }

 




    void TriggerInvulnerable() 
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }//触发无敌


    public void OnSlide(int cost) 
    {
        currentPower -= cost;
        currentPower = Mathf.Clamp(currentPower, 0, maxPower);

        //传输Character过去
        OnHealthChange?.Invoke(this);

    }//体力消耗传输




}
