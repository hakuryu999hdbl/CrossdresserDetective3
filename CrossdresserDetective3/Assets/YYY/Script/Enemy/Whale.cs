using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whale : EnemyController, IDamageable
{
    public void GetHit(float damage)
    {
        health -= damage;

        if (health < 1)
        {
            health = 0;
            isDead = true;
        }

        anim.SetTrigger("hit");
    }//不同的敌人做出不同反应



    
}
