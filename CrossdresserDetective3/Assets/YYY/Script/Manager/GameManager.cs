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




        //检测是不是调查任务
        Invoke(nameof(CheckClue), 0.1f);
        //检测是不是救出任务
        Invoke(nameof(CheckRescue),0.2f);
        //检测是不是歼灭任务
        Invoke(nameof(CheckEliminate), 0.3f);
    }



    /// <summary>
    /// 关卡
    /// </summary>
    #region
    [Header("关卡")]
    public List<GameObject> AreaList; // 在Inspector中添加Area_1~3

    public void SetArea(int index)
    {
        Instantiate(AreaList[index], Vector3.zero, Quaternion.identity);
    }

    #endregion





    [Header("其他设置")]
    public bool gameOver = false;//玩家死亡游戏结束
    public bool PlayerWin = false;//好像获胜界面会触发很多下的样子，为了只触发一下
    public void Update()
    {

        gameOver = player.isDead || PlayerWin;//玩家死亡的时候所有敌人不能动


    }

    private bool isChangingScene;

    public void RestartScene()
    {
        if (isChangingScene) return;

        StartCoroutine(RestartSceneCoroutine());
    }

    private IEnumerator RestartSceneCoroutine()
    {
        isChangingScene = true;

        // 此时继续保持暂停，战斗场景不会恢复
        UIManager.instance.BlackScreen_FadeIn.SetActive(true);

        // 不受 Time.timeScale 影响
        yield return new WaitForSecondsRealtime(0.95f);

        // 只在加载场景前恢复
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public void NextScene()
    {
        if (isChangingScene) return;

        StartCoroutine(NextSceneCoroutine());
    }

    private IEnumerator NextSceneCoroutine()
    {
        isChangingScene = true;

        // 此时依旧保持 Time.timeScale = 0
        UIManager.instance.BlackScreen_FadeIn.SetActive(true);

        yield return new WaitForSecondsRealtime(0.95f);

        GameFlowData.CurrentStage++;

        // 每章10关
        if (GameFlowData.CurrentStage > 10)
        {
            GameFlowData.CurrentStage = 1;
            GameFlowData.CurrentChapter++;
        }

        // 加载之前才恢复正常时间
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }







    public enum WinMode
    {
        Eliminate,   // 消灭
        Escape,      // 逃脱
        Investigate, // 搜查
        Rescue       // 救援
    }

    public WinMode winMode;



    /// <summary>
    /// 线索数量/搜查模式获胜
    /// </summary>
    #region

    [Header("搜查模式")]
    public int totalClues;
    public int currentClues;

    public void RegisterClue()
    {
        totalClues++;
    }

    public void CompleteClue()
    {
        currentClues++;

        UIManager.instance.RefreshClueUI(currentClues, totalClues);

        if (currentClues >= totalClues)
        {
            PlayerEscapeWin();//调查模式获胜
        }
    }

    public void CheckClue() 
    {
        if (totalClues > 0)
        {
            winMode = WinMode.Investigate;

            //场景里有线索，这是调查任务
            UIManager.instance.RefreshClueUI(currentClues, totalClues);
            UIManager.instance.clueText.gameObject.SetActive(true);   
        }
    }



    #endregion



    /// <summary>
    /// 人质数量/救出模式获胜
    /// </summary>
    #region 救援模式

    [Header("救援模式")]
    public int totalRescues;
    public int currentRescues;

    public void RegisterRescueTarget()
    {
        totalRescues++;
    }

    public void CompleteRescue()
    {
        if (PlayerWin)
            return;

        currentRescues++;

        currentRescues = Mathf.Clamp(
            currentRescues,
            0,
            totalRescues
        );

        UIManager.instance.RefreshRescueUI(
            currentRescues,
            totalRescues
        );

        if (currentRescues >= totalRescues)
        {
            PlayerEscapeWin();//救出模式获胜
        }
    }

    public void CheckRescue()
    {
        if (totalRescues > 0)
        {
            winMode = WinMode.Rescue;

            //场景里有人质，这是救出模式
            UIManager.instance.RefreshRescueUI(currentRescues,totalRescues);
            UIManager.instance.rescueText.gameObject.SetActive(true);
        }
    }

    #endregion




    /// <summary>
    /// 出口大门/逃脱模式获胜
    /// </summary>
    #region
    ExitDoor exitDoor;
    public void IsExit(ExitDoor door)
    {
        exitDoor = door;

        winMode = WinMode.Escape;
        UIManager.instance.escapeText.gameObject.SetActive(true);
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

        bool allEnemiesDead =
        enemyCreators.Count <= 0 &&
        sceneEnemies.Count <= 0;

        if (!allEnemiesDead)
            return;

        switch (winMode)
        {
            case WinMode.Eliminate:
                PlayerEscapeWin();//歼灭模式通常胜利
                break;

            case WinMode.Investigate:
                PlayerEscapeWin();  // 搜查模式允许全部击杀直接胜利
                break;

            case WinMode.Rescue:
                if (currentRescues >= totalRescues && totalRescues > 0)
                {
                    PlayerEscapeWin(); // 救援模式杀光敌人不算完成，还必须把人质全部救走
                }
                break;

            case WinMode.Escape:
                // 逃脱模式必须到出口，不因杀光敌人获胜
                break;
        }

    }//消灭所有敌人通关，但是有人质的情况下必须救出所有人质


    //在玩家被击败后清理状态
    public void ClearEnemiesForGameOver()
    {
        // 先关闭敌人生成器，防止黑幕后继续刷怪
        foreach (AreaEncounterController creator in enemyCreators)
        {
            if (creator == null)
                continue;

            creator.gameObject.SetActive(false);
        }

        enemyCreators.Clear();

        // 隐藏所有现存敌人，但保留尸体替身
        foreach (EnemyController enemy in sceneEnemies)
        {
            if (enemy == null)
                continue;

            enemy.gameObject.SetActive(false);
        }

        sceneEnemies.Clear();
    }


    void CheckEliminate() 
    {
        if (winMode == WinMode.Eliminate) 
        {
            UIManager.instance.eliminateText.gameObject.SetActive(true);
        }


    }//检测如果没有别的任务，就是歼灭

    #endregion

}
