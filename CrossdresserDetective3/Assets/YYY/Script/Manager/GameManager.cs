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


        //player = FindFirstObjectByType<PlayerController>();
        //doorExit = FindFirstObjectByType<Door>();
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

        PlayerPrefs.DeleteKey("playerHealth");
    }

    public void NewGame() 
    {
        PlayerPrefs.DeleteAll();
        SceneManager.LoadScene(1);
    }//跳转编号场景

    public void NextScene()
    {
        //进入下一关存储血量
        SaveData();


        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextIndex);
        }
        else
        {
            Debug.Log("已经是最后一个场景了");
        }
    }


    public void QuitGame() 
    {

        Application.Quit();
    }

    public float LoadHealth() 
    {
        if (!PlayerPrefs.HasKey("playerHealth"))
        {
            PlayerPrefs.SetFloat("playerHealth", 3f);
        }

        float currentHealth = PlayerPrefs.GetFloat("playerHealth");

        return currentHealth;
    }

    public void SaveData() 
    {
        PlayerPrefs.SetFloat("playerHealth",player.health);
        PlayerPrefs.Save();
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
