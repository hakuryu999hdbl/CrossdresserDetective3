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
    private bool playerInitialized;
    public void IsPlayer(PlayerController controller)
    {
        player = controller;
        if (IsUndercoverStage)
        {
            var suspicion = player.GetComponent<PlayerSuspicion>();
            if (suspicion == null)
                suspicion = player.gameObject.AddComponent<PlayerSuspicion>();
            suspicion.BeginUndercover();
        }
        playerInitialized = true;
        TryInitializeMission();

    }//玩家自己传过来


    [Header("关卡")]
    public GameObject Light_Night;

    public GameObject DetectiveAgency_1, DetectiveAgency_2, DetectiveAgency_3;
    public GameObject Company_1, Company_2, Company_3;
    public GameObject ParkingArea_1;
    public GameObject Outside_1;
    public GameObject Outside_2;
    public GameObject Office_1;
    public GameObject ConstructionSite_1, ConstructionSite_2;

    private void Start()
    {








        //由于点击【下一关】不回到主菜单出发记录，所以GameManager还是需要身上保留关卡地图任务字典
        switch (CurrentChapter)
        {
            case 1:
                switch (CurrentStage)
                {
                    case 1:
                        LoadMap(DetectiveAgency_1);
                        break;

                    case 2:
                        LoadMap(DetectiveAgency_2);
                        break;

                    case 3:
                        LoadMap(Company_1);
                        break;

                    case 4:
                        LoadMap(Company_2);
                        break;

                    case 5:
                        LoadMap(Company_1);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;

                    case 6:
                        LoadMap(Company_2);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;
                    case 7:
                        LoadMap(Company_3);
                        break;
                    case 8:
                        LoadMap(Company_3);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;
                    case 9:
                        LoadMap(DetectiveAgency_3);
                        break;
                    case 10:
                        LoadMap(DetectiveAgency_3);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;
                }
                break;

            case 2:
                switch (CurrentStage)
                {
                    case 1:
                        LoadMap(Office_1);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;

                    case 2:
                        LoadMap(Outside_1);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;

                    case 3:
                        LoadMap(DetectiveAgency_3);
                        break;
                    case 4:
                    case 5:
                        LoadMap(Outside_1);
                        break;



                    case 6:
                        LoadMap(Company_2);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;
                    case 7:
                        LoadMap(Outside_1);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;

                    case 8:
                        LoadMap(Outside_2);
                        break;
                    case 9:
                    case 10:
                        LoadMap(Outside_2);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;
                }
                break;

            case 3:
                switch (CurrentStage)
                {
                    case 1:
                        LoadMap(ConstructionSite_1);
                        break;
                    case 2:
                        LoadMap(ConstructionSite_2);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;
                    case 3:
                        LoadMap(ConstructionSite_1);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
                        break;
                    case 4:
                        LoadMap(ConstructionSite_2);
                        break;

                    case 5:
                        LoadMap(ConstructionSite_1);
                        break;
                    case 6:
                        LoadMap(ConstructionSite_2);
                        skyboxSample.Night();//晚上关卡单独指定
                        Light_Night.SetActive(true);//夜间灯光
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

        LoadMissionEntry();
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

        // Demo限制：通关第一章第10关后，点击下一关进入感谢试玩。
        if (GameFlowData.CurrentChapter == 1 && GameFlowData.CurrentStage == 10)
        {
            GameFlowData.nextAreaId = "Demo";
            GameFlowData.returnPath = null;
            Time.timeScale = 1f;
            SceneManager.LoadScene("Spine");
            yield break;
        }

        GameFlowData.CurrentStage++;

        // 每章10关
        if (GameFlowData.CurrentStage > 10)
        {
            GameFlowData.CurrentStage = 1;
            GameFlowData.CurrentChapter++;
        }

        // 加载之前才恢复正常时间
        Time.timeScale = 1f;

        // 2-2 开头先播放剧情 CG，结束后由 DialogSystem 进入 Level。
        if (GameFlowData.CurrentChapter == 2 && GameFlowData.CurrentStage == 2)
        {
            GameFlowData.nextAreaId = "CG_Fera";
            GameFlowData.returnPath = null;
            SceneManager.LoadScene("Spine");
            yield break;
        }

        LoadMissionEntry();
    }

    #endregion



    /// <summary>
    /// 各类任务UI
    /// </summary>
    #region

    [Header("各类任务UI")]

    public SkyboxSample skyboxSample;
    public GameObject Mission;
    public void IsSkyboxSample(SkyboxSample _skyboxSample)
    {
        skyboxSample = _skyboxSample;
    }//天空盒自己来报告

    public RoomManager roomManager;
    private bool escapeInitialized;

    public static bool IsEscapeStage =>
        (CurrentChapter == 1 && CurrentStage == 1) ||
        (CurrentChapter == 2 && (CurrentStage == 1 || CurrentStage == 2 || CurrentStage == 10));

    public static bool IsDirectRescueStage => CurrentChapter == 2 && CurrentStage == 9;

    // 潜伏操作限制按关卡指定，暴露后解除，与任务类型独立。
    public static bool IsUndercoverStage =>
        CurrentChapter == 2 && (CurrentStage == 1 || CurrentStage == 2 || CurrentStage == 9);

    public static bool IsEliminateStage =>
        (CurrentChapter == 1 && (CurrentStage == 2 || CurrentStage == 9)) ||
        (CurrentChapter == 2 && (CurrentStage == 6 || CurrentStage == 7)) ||
        (CurrentChapter == 3 && (CurrentStage == 5 || CurrentStage == 6));

    private bool directRescueInitialized;

    private void LoadMap(GameObject mapPrefab)
    {
        GameObject map = Instantiate(mapPrefab, Vector3.zero, Quaternion.identity);
        roomManager = map.GetComponentInChildren<RoomManager>();
        TryInitializeMission();
    }

    private void TryInitializeMission()
    {
        if (!playerInitialized || roomManager == null)
            return;

        // 复用地图的逃脱关由出生点触发开场 Timeline。
        if (RequiresMissionSetup || IsDirectRescueStage ||
            (IsEscapeStage && roomManager.playerSpawnPoints_escape != null))
        {
            ShowMissionUI();
        }
    }

    public void ShowMissionUI()
    {
        if (IsEscapeStage)
        {
            Mission.SetActive(true);
            ShowEscape();
            return;
        }

        Mission.SetActive(true);

        if (IsEliminateStage)
        {
            ShowEliminate();
            return;
        }

        Debug.Log("显示模式：" + GameFlowData.CurrentMissionType);




        //由于点击【下一关】不回到主菜单出发记录，所以GameManager还是需要身上保留关卡地图任务字典
        switch (GameFlowData.CurrentChapter)
        {
            case 1:
                switch (GameFlowData.CurrentStage)
                {
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
                        ShowClue();
                        break;
                    case 8:
                        ShowRescue();
                        break;
                    case 10:
                        ShowRescue();
                        break;
                }
                break;

            case 2:
                switch (GameFlowData.CurrentStage)
                {
                    case 3:
                        ShowClue();
                        break;
                    case 4:
                        ShowRescue();
                        break;
                    case 5:
                        ShowClue();
                        break;
                    case 8:
                        ShowClue();
                        break;
                    case 9:
                        if (!directRescueInitialized && player != null && roomManager != null)
                        {
                            directRescueInitialized = true;
                            ShowRescue();
                        }
                        break;
                }
                break;

            case 3:
                switch (GameFlowData.CurrentStage)
                {
                    case 1:
                        ShowClue();
                        break;
                    case 2:
                        ShowRescue();
                        break;
                    case 3:
                        ShowRescue();
                        break;
                    case 4:
                        ShowClue();
                        break;
                }
                break;
        }



    }//任务入场或剧情事件通知 RoomManager 初始化对应任务

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

    public void PlayerCutsceneWin()
    {
        if (PlayerWin) return;

        PlayerWin = true;
        UIManager.instance.CutsceneWinUI();
    }

    void ShowEscape()
    {
        if (escapeInitialized) return;
        escapeInitialized = true;
        GameFlowData.CurrentMissionType = MissionType.Escape;
        Debug.Log("逃脱模式");
        if (roomManager != null)
        {
            roomManager.SetupEscape();
        }

        UIManager.instance.SetEscapeObjectiveVisible(true);

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

    public bool EnemiesSuspendedForCaptivity { get; private set; }
    private readonly List<EnemyController> captivitySuspendedEnemies = new List<EnemyController>();

    [ContextMenu("测试/暂停全体敌人")]
    public void SuspendEnemiesForCaptivity()
    {
        EnemiesSuspendedForCaptivity = true;
        // 包括救援对象生成的敌人；不删 sceneEnemies、不改变任务引用。
        foreach (EnemyController enemy in FindObjectsByType<EnemyController>(FindObjectsSortMode.None))
        {
            if (enemy == null || enemy.isDead) continue;
            if (!captivitySuspendedEnemies.Contains(enemy)) captivitySuspendedEnemies.Add(enemy);
            enemy.SuspendForCaptivity();
        }
    }

    [ContextMenu("测试/恢复全体敌人")]
    public void ResumeEnemiesAfterCaptivity()
    {
        if (!EnemiesSuspendedForCaptivity) return;
        EnemiesSuspendedForCaptivity = false;
        // 同时收集暂停期间新生成的敌人。
        foreach (EnemyController enemy in FindObjectsByType<EnemyController>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            if (enemy != null && enemy.IsSuspended && !captivitySuspendedEnemies.Contains(enemy))
                captivitySuspendedEnemies.Add(enemy);
        foreach (EnemyController enemy in captivitySuspendedEnemies)
            if (enemy != null && !enemy.isDead) enemy.ResumeFromCaptivity();
        captivitySuspendedEnemies.Clear();
    }

    public void ClearEnemiesForGameOver()
    {
        //不用关闭，变成生成ENEMY_1
        // 先关闭敌人生成器，防止黑幕后继续刷怪
        //foreach (AreaEncounterController creator in enemyCreators)
        //{
        //    if (creator == null)
        //        continue;
        //
        //    creator.gameObject.SetActive(false);
        //}
        //
        //enemyCreators.Clear();

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








    // 进入解缚逃脱
    public void ShowBondageUnlockText()
    {


        // 逃脱模式保留原任务标题
        if (GameFlowData.CurrentMissionType == GameFlowData.MissionType.Escape)
        {
            UIManager.instance.BondageUnlockText.SetActive(false);
            UIManager.instance.SetEscapeObjectiveVisible(true);
            return;
        }






        // 隐藏原任务信息
        UIManager.instance.clueText.gameObject.SetActive(false);
        UIManager.instance.rescueText.gameObject.SetActive(false);
        UIManager.instance.SetEscapeObjectiveVisible(false);
        UIManager.instance.eliminateText.SetActive(false);

        // 显示解缚任务
        UIManager.instance.BondageUnlockText.SetActive(true);
    }


    // 拿到开锁器，恢复原任务
    public void HideBondageUnlockText()
    {
        UIManager.instance.BondageUnlockText.SetActive(false);

        // 根据当前任务模式恢复

        switch (GameFlowData.CurrentMissionType)
        {
            case GameFlowData.MissionType.Investigate:
                UIManager.instance.clueText.gameObject.SetActive(true);
                break;

            case GameFlowData.MissionType.Rescue:
                UIManager.instance.rescueText.gameObject.SetActive(true);
                break;

            case GameFlowData.MissionType.Eliminate:
                UIManager.instance.eliminateText.SetActive(true);
                break;

            case GameFlowData.MissionType.Escape:
                UIManager.instance.SetEscapeObjectiveVisible(true);
                break;
        }


    }



}
