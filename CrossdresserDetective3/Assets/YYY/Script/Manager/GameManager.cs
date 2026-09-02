using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameFlowData;

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



    [Header("关卡")]
    public GameObject DetectiveAgency_1, DetectiveAgency_2;
    public GameObject Company_1, Company_2;

    private void Start()
    {
        //AudioManager.Instance.PlayBGM(AudioManager.Instance.BGM_Level_1, true);






        //由于点击【下一关】不回到主菜单出发记录，所以GameManager还是需要身上保留关卡地图任务字典
        switch (CurrentChapter)
        {
            case 1:
                switch (CurrentStage)
                {
                    case 1:
                        Instantiate(DetectiveAgency_1, Vector3.zero, Quaternion.identity);
                        break;

                    case 2:
                        Instantiate(DetectiveAgency_2, Vector3.zero, Quaternion.identity);
                        break;

                    case 3:
                        Instantiate(Company_1, Vector3.zero, Quaternion.identity);
                        skyboxSample.Night();//晚上关卡单独指定
                        break;

                    case 4:
                        Instantiate(Company_2, Vector3.zero, Quaternion.identity);
                        break;

                    case 5:
                        Instantiate(Company_1, Vector3.zero, Quaternion.identity);
                        break;

                    case 6:
                        Instantiate(Company_2, Vector3.zero, Quaternion.identity);
                        skyboxSample.Night();//晚上关卡单独指定
                        break;
                    case 7:
                    case 8:
                    case 9:
                    case 10:
                        Instantiate(Company_1, Vector3.zero, Quaternion.identity);
                        break;
                }
                break;
        }



    }




    /// <summary>
    /// 重启游戏/下一关入口
    /// </summary>
    #region

    [Header("重启游戏/下一关入口")]
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
        UIManager.instance.blackScreen.SetFadeIn();

        // 不受 Time.timeScale 影响
        yield return new WaitForSecondsRealtime(1f);

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
        UIManager.instance.blackScreen.SetFadeIn();

        yield return new WaitForSecondsRealtime(1f);

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

    #endregion



    /// <summary>
    /// 各类任务UI
    /// </summary>
    #region

    public SkyboxSample skyboxSample;

    public void IsSkyboxSample(SkyboxSample _skyboxSample)
    {
        skyboxSample = _skyboxSample;
    }//天空盒自己来报告

    public RoomManager roomManager;

    public void IsRoomManager(RoomManager _roomManager)
    {
        roomManager = _roomManager;
    }//RoomManager自己来报告

    public void ShowMissionUI()
    {



        //Debug.Log("显示模式：" + GameFlowData.CurrentMissionType);




        //由于点击【下一关】不回到主菜单出发记录，所以GameManager还是需要身上保留关卡地图任务字典
        switch (GameFlowData.CurrentChapter)
        {
            case 1:
                switch (GameFlowData.CurrentStage)
                {
                    case 1:                
                        ShowEscape();
                        break;

                    case 2:                 
                        ShowEliminate();
                        break;

                    case 3:
                        ShowClue();
                        break;

                    case 4:
                        ShowRescue();
                        break;

                    case 5:
                        ShowRescue();
                        break;

                    case 6:                    
                        ShowClue();
                        break;

                    case 7:
                    case 8:
                    case 9:
                    case 10:                  
                        ShowEliminate();
                        break;
                }
                break;
        }



    }//一般在装备设置界面关闭和主线剧情过场动画结尾触发这是什么任务的提示和通知RoomManager

    #endregion


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

    public void ShowClue()
    {
        GameFlowData.CurrentMissionType = MissionType.Investigate;
        Debug.Log("搜查模式");


        if (roomManager != null)
        {
            roomManager.SetupInvestigate();
        }



        //场景里有线索，这是调查任务
        UIManager.instance.RefreshClueUI(currentClues, totalClues);
        UIManager.instance.clueText.gameObject.SetActive(true);

    }//弹出这是搜查模式提示



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

    public void ShowRescue()
    {
        GameFlowData.CurrentMissionType = MissionType.Rescue;
        Debug.Log("救出模式");

        if (roomManager != null)
        {
            roomManager.SetupRescue();
        }


        Invoke(nameof(RenewRescuesUI), 0.2f);//似乎是生成RBQ之后要缓一下更新UI？

    }//弹出这是救出模式提示


    void RenewRescuesUI()
    {
        //场景里有人质，这是救出模式
        UIManager.instance.RefreshRescueUI(currentRescues, totalRescues);
        UIManager.instance.rescueText.gameObject.SetActive(true);
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
    }
    public void PlayerEscapeWin()
    {
        if (PlayerWin) return;

        UIManager.instance.WinUI();
        PlayerWin = true;
    }//这个用于逃出模式

    void ShowEscape()
    {
        GameFlowData.CurrentMissionType = MissionType.Escape;
        Debug.Log("逃脱模式");

        if (roomManager != null)
        {
            roomManager.SetupEscape();
        }

        UIManager.instance.escapeText.gameObject.SetActive(true);

    }//弹出这是逃脱任务提示

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

        switch (GameFlowData.CurrentMissionType)
        {
            case GameFlowData.MissionType.Eliminate:
                PlayerEscapeWin();
                break;

            case GameFlowData.MissionType.Investigate:
                // 搜查模式允许杀光敌人直接胜利
                PlayerEscapeWin();
                break;

            case GameFlowData.MissionType.Rescue:
                // 救援必须救完人质
                if (currentRescues >= totalRescues &&
                    totalRescues > 0)
                {
                    PlayerEscapeWin();
                }
                break;

            case GameFlowData.MissionType.Escape:
                // 逃脱必须到出口
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


    void ShowEliminate()
    {
        GameFlowData.CurrentMissionType = MissionType.Eliminate;
        Debug.Log("歼灭模式");

        if (roomManager != null)
        {
            roomManager.SetupEliminate();
        }



        UIManager.instance.eliminateText.gameObject.SetActive(true);

    }//跳出歼灭模式提示

    #endregion

}
