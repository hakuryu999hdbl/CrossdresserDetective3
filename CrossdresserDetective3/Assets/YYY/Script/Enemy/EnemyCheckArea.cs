using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckArea : MonoBehaviour
{
    public EnemyController enemy;

    private void OnTriggerEnter2D(Collider2D other)
    {
        enemy.OnCheckAreaEnter(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        enemy.OnCheckAreaExit(other);
    }
}
