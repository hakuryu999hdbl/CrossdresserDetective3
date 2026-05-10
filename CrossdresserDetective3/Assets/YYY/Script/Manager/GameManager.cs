using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public void Awake()
    {
        instance = this;


        //player = FindFirstObjectByType<PlayerController>();
        //doorExit = FindFirstObjectByType<Door>();
    }



    private void Start()
    {
        //AudioManager.Instance.PlayBGM(AudioManager.Instance.BGM_Level_1, true);
    }


    public void IsPlayer(PlayerController controller) 
    {
        player = controller;

    }//玩家自己传过来

    public void IsExit(Door door) 
    {
        doorExit = door;
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
            //doorExit.OpenDoor();
        }

    }

}
