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

    }

    public PlayerController player;
    public void IsPlayer(PlayerController controller)
    {
        player = controller;

    }//玩家自己传过来





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
                    default:
                    case 1:
                        Invoke(nameof(SetPlayerClothes_01), 0.2f);
                        SetArea(0); 




                        break;
                  
                    case 2:
                        SetArea(1);  
                        break;
                    case 3:
                        SetArea(2);
                        break;
                  
                    case 4:
                        SetArea(3);
                        break;
                    case 5:
                        SetArea(4);
                        break;
                    case 6:
                        SetArea(5);
                        break;
                    case 7:
                        SetArea(6);
                        break;
                    case 8:
                        SetArea(7);
                        break;
                    
                    case 9:
                        SetArea(8);
                        break;
                    case 10:
                        SetArea(9);
                        break;



                }
                break;

        }

       
    }

    public void SetPlayerClothes_01() 
    {
        player.frameEvent.Story_Clothes_YYY_01();//第一章事务所衣物
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
    /// 出口大门/逃脱模式获胜
    /// </summary>
    #region
    ExitDoor exitDoor;
    public void IsExit(ExitDoor door)
    {
        exitDoor = door;
    }
    public void PlayerEscapeWin()
    {
        if (PlayerWin) return;

        UIManager.instance.WinUI();
        PlayerWin = true;
    }//这个用于逃出模式

    #endregion




    /// <summary>
    /// 检测敌人/歼灭模式获胜
    /// </summary>
    #region
    [Header("检测敌人生成器")]
    public List<AreaEncounterController> enemyCreators = new List<AreaEncounterController>();
    [Header("场景固定敌人")]
    public List<EnemyController> sceneEnemies = new List<EnemyController>();

    public void IsEnemyCreator(AreaEncounterController areaEncounterController)
    {
        if (!enemyCreators.Contains(areaEncounterController))
            enemyCreators.Add(areaEncounterController);
    }

    public void EnemyCleanOver(AreaEncounterController areaEncounterController)
    {
        enemyCreators.Remove(areaEncounterController);
        CheckWinCondition();
    }

    public void RegisterSceneEnemy(EnemyController enemy)
    {
        if (!sceneEnemies.Contains(enemy))
            sceneEnemies.Add(enemy);
    }

    public void SceneEnemyDead(EnemyController enemy)
    {
        sceneEnemies.Remove(enemy);
        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (PlayerWin) return;

        sceneEnemies.RemoveAll(e => e == null || e.isDead);

        if (enemyCreators.Count <= 0 && sceneEnemies.Count <= 0)
        {
            UIManager.instance.WinUI();
            PlayerWin = true;
        }
    }
    #endregion

}
