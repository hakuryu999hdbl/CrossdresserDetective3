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
        switch (GameFlowData.CurrentChapter)
        {           
            default:       
            case 1:

                switch (GameFlowData.CurrentStage)
                {
                   
                    case 1:
                        SetArea(4); 
                        break;
                    case 2:
                        SetArea(5);  
                        break;
                    case 3:
                        SetArea(6);
                        break;
                    default:
                    case 4:
                        SetArea(7);
                        break;
                }
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
        Instantiate(areaList[index], Vector3.zero, Quaternion.identity);
    }

    #endregion





    [Header("其他设置")]
    public bool gameOver = false;//玩家死亡游戏结束
    public bool PlayerWin = false;//好像获胜界面会触发很多下的样子，为了只触发一下
    public void Update()
    {

        gameOver = player.isDead;//玩家死亡的时候所有敌人不能动
       
        
    }

    public void RestartScene() 
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
     
    }

    public void NextScene()
    {
        GameFlowData.CurrentStage++;

        // 每章10关
        if (GameFlowData.CurrentStage > 10)
        {
            GameFlowData.CurrentStage = 1;
            GameFlowData.CurrentChapter++;
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }






    /// <summary>
    /// 检测敌人
    /// </summary>
    #region
    [Header("检测敌人生成器")]
    public List<AreaEncounterController> EnemyCreators = new List<AreaEncounterController>();//游戏开始的时候所有敌人生成器登记进入这个列表，当这个列表空了后打开大门

    public void IsEnemyCreator(AreaEncounterController areaEncounterController) 
    {
        EnemyCreators.Add(areaEncounterController);
    }

    public void EnemyCleanOver(AreaEncounterController areaEncounterController) 
    {
        EnemyCreators.Remove(areaEncounterController);

        if (EnemyCreators.Count<=0&& !PlayerWin) 
        {
            //doorExit.OpenDoor();

            UIManager.instance.WinUI();

            PlayerWin = true;
        }

    }
    #endregion

}
