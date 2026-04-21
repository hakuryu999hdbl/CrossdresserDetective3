using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckArea : MonoBehaviour
{
    public EnemyController enemy;


    private void OnTriggerStay2D(Collider2D other)
    {
        enemy.OnCheckAreaStay(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        enemy.OnCheckAreaExit(other);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!enemy.isDead && !GameManager.instance.gameOver)
        {
            StartCoroutine(OnAlarm());
        }

    }
    IEnumerator OnAlarm()
    {
        enemy.alarmSign.SetActive(true);
        yield return new WaitForSeconds(enemy.alarmSign.GetComponent<Animator>().GetCurrentAnimatorClipInfo(0)[0].clip.length);
        enemy.alarmSign.SetActive(false);
    }//检测该物体的动画播放完隐藏

}
