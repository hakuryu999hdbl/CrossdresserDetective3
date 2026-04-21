using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public void Awake()
    {
        //跨场景保存，单独留有一个
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }


        player = FindObjectOfType<PlayerController>();
        doorExit = FindObjectOfType<Door>();
    }


    PlayerController player;

    public bool gameOver = false;//玩家死亡游戏结束

    public void Update()
    {
        gameOver = player.isDead;
        UIManager.instance.GameOverUI(gameOver);
    }

    public void RestartScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void NextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    Door doorExit;




    public List<EnemyController> enemies = new List<EnemyController>();//游戏开始的时候所有敌人登记进入这个列表，当这个列表空了后打开大门

    public void IsEnemy(EnemyController enemy) 
    {
        enemies.Add(enemy);
    }

    public void EnemyDead(EnemyController enemy) 
    {
        enemies.Remove(enemy);

        if (enemies.Count<=0) 
        {
            doorExit.OpenDoor();
        }

    }

}
