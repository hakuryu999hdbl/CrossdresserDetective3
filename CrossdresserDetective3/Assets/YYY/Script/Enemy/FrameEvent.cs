using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameEvent : MonoBehaviour
{
    public EnemyController enemyController;

    public void SetOff()
    {
        if (enemyController.targetPoint.GetComponent<Bomb>() != null)
        {
            enemyController.targetPoint.GetComponent<Bomb>().TurnOff();
        }
        
      
    }
}
