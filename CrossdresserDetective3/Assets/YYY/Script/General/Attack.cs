using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;//伤害数值
    public float attackRange;//伤害范围
    public float attackRate;//伤害频率

    private void OnTriggerStay2D(Collider2D other)
    {

        other.GetComponent<Character>()?.TakeDamage(this);
    }
}

