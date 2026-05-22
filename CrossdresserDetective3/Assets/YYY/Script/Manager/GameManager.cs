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

    PlayerController player;
    public void IsPlayer(PlayerController controller)
    {
        player = controller;

    }//玩家自己传过来

    Door doorExit;
    public void IsExit(Door door)
    {
        doorExit = door;
    }//这个用于逃出模式
  



    private void Start()
    {
        //AudioManager.Instance.PlayBGM(AudioManager.Instance.BGM_Level_1, true);

        //根据当前临时存档读取位置
        switch (GameFlowData.nextAreaId)
        {
            case "":
            default:
            //俱乐部
            case "Chapter1_1":   
                SetArea(0);
                break;

            //停车场
            case "Chapter1_2":
                SetArea(1);
                break;

            //外景
            case "Chapter1_3":
                SetArea(2);
                break;


            //事务所
            case "Chapter1_4":
                SetArea(3);
                break;
        }


    }
    /// <summary>
    /// 关卡
    /// </summary>
    #region
    [Header("关卡")]
    public List<GameObject> areaList; // 在Inspector中添加Area_1~3

    public void SetArea(int index)
    {




        GameObject NewArea = Instantiate(areaList[index], Vector3.zero, Quaternion.identity);


        // 找到新区域里的 CameraBounds（PolygonCollider2D）
        //PolygonCollider2D newBounds = NewArea.transform.Find("CameraBounds").GetComponent<PolygonCollider2D>();
        //SetNewBounds(newBounds);


        // 把玩家的位置设为这个出生点

       //switch (GameFlowData.nextAreaId)
       //{
       //    default:
       //    case "Area01_1":
       //    case "Area02_1":
       //    case "Area03_1":
       //    case "Area04_1":
       //    case "Area05_1":
       //    case "Area06_1":
       //    case "Area07_1":
       //        Transform PlayerPoint = NewArea.transform.Find("PointForPlayer_1");
       //        player.transform.position = PlayerPoint.position;
       //        break;
       //
       //    case "Area01_2":
       //    case "Area02_2":
       //    case "Area04_2":
       //    case "Area05_2":
       //    case "Area06_2":
       //        Transform PlayerPoint_2 = NewArea.transform.Find("PointForPlayer_2");
       //        player.transform.position = PlayerPoint_2.position;
       //        break;
       //
       //
       //    case "Area05_3":
       //    case "Area01_3":
       //        Transform PlayerPoint_3 = NewArea.transform.Find("PointForPlayer_3");
       //        player.transform.position = PlayerPoint_3.position;
       //        break;
       //    case "Area01_4":
       //        Transform PlayerPoint_4 = NewArea.transform.Find("PointForPlayer_4");
       //        player.transform.position = PlayerPoint_4.position;
       //        break;
       //}


    }

    #endregion

    [Header("其他设置")]
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
