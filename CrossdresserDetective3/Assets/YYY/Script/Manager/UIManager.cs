using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class UIManager : MonoBehaviour
{




    [Header("判断摇杆")]
    public GameObject mobileTouch;

    public static UIManager instance;

    public void Awake()
    {

#if UNITY_STANDALONE
    mobileTouch.SetActive(false);
#endif


        //跨场景保存，单独留有一个
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

   



    /// <summary>
    /// 暂停菜单
    /// </summary>
    #region

    [Header("暂停菜单")]
    public GameObject PauseMenu;
    public GameObject firstSelected; //进入设置页面最先选中 X 或 Master Slider

    public bool isPaused=false;
    public PlayerController playerController;//玩家脚本
    public void TogglePause() 
    {
        if (isPaused)
        {
            ClosePause();
        }
        else 
        {
            OpenPause();
        }
    }
    public void OpenPause()
    {
        isPaused = true;
        Time.timeScale = 0f;
        PauseMenu.SetActive(true);

        playerController.DisableGameplayInput();  // 打开 UI 输入、设置默认选中，关闭玩家中的游戏设置

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void ClosePause()
    {
        isPaused = false;
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;

        playerController.EnableGameplayInput();// 关闭 UI 输入

        firstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void BackToMenu()
    {
        //根据当前临时存档确认回退到主菜单后的位置
        switch (GameFlowData.CurrentChapter)
        {
            default:
            case 1:
                GameFlowData.returnPath = "chapter_1";
                break;
          
        }


        Time.timeScale = 1f;
        SceneManager.LoadScene(0);

    }//跳转编号场景

  
    #endregion



    /// <summary>
    /// 放大显示人体
    /// </summary>
    #region
    [Header("放大显示人体")]
    public Animator anim;

    public void ShowPortrait()
    {
        anim.SetBool("isShow", true);
    }

    public void HidePortrait()
    {
        anim.SetBool("isShow", false);
    }

    #endregion







    [Header("Boss生命值")]
    public Slider bossHealthBar;
 
    public void SetBossHealth(float health)
    {
        bossHealthBar.maxValue = health;
    }
    
    public void UpdateBossHealth(float health)
    {
        bossHealthBar.value = health;
    }









    /// <summary>
    /// 游戏结束菜单
    /// </summary>
    #region
    [Header("游戏结束菜单")]
    public GameObject gameOverPanel;
    public GameObject GameOverfirstSelected; //进入设置页面最先选中 X 或 Master Slider

    public GameObject MissionFailure;
    public void GameOverUI()
    {
        //一旦开始结算，另一种结果不能出现
        if (isResultShowing) return;
        isResultShowing = true;

        playerController.DisableGameplayInput();  // 打开 UI 输入、设置默认选中，关闭玩家中的游戏设置

        MissionFailure.SetActive(true);

        StartCoroutine(GameOverDelay());
    }

    IEnumerator GameOverDelay()
    {
        yield return new WaitForSeconds(1f);

        gameOverPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GameOverfirstSelected);
    }


    public GameObject WinPanel;
    public GameObject WinfirstSelected; //进入设置页面最先选中 X 或 Master Slider
    
    public GameObject MissionComplete;

    public void WinUI()
    {
        //一旦开始结算，另一种结果不能出现
        if (isResultShowing) return;
        isResultShowing = true;


        SaveStageResult();//储存通关星数

        playerController.DisableGameplayInput();  // 打开 UI 输入、设置默认选中，关闭玩家中的游戏设置

        MissionComplete.SetActive(true);

        StartCoroutine(WinDelay());
    }

    IEnumerator WinDelay()
    {
        yield return new WaitForSeconds(1f);

        WinPanel.SetActive(true);
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(WinfirstSelected);
    }

    public bool isResultShowing = false;//一旦开始结算，另一种结果不能出现

    private void SaveStageResult()
    {
        SaveData data =
            SaveManager.LoadGame(GameFlowData.CurrentPlayer);

        data.InitStageData();

        int chapter = GameFlowData.CurrentChapter;
        int stage = GameFlowData.CurrentStage;

        int index =
            (chapter - 1) * data.stagePerChapter
            + (stage - 1);

        int star = 3; // 以后换成实际计算

        // 保留最高评价
        data.stageStars[index] =
            Mathf.Max(data.stageStars[index], star);

        // 解锁下一关
        int nextIndex = index + 1;

        if (nextIndex < data.stageStars.Length)
        {
            if (data.stageStars[nextIndex] < 0)
            {
                data.stageStars[nextIndex] = 0;
            }
        }

        SaveManager.SaveGame(data);
    }


    #endregion

















    /// <summary>
    /// 生命值，体力值，弹药等UI
    /// </summary>
    #region

    [Header("事件监听")]
    public CharacterEventSO healthEvent;

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;

    }
    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;

    }

    void OnHealthEvent(Character character) 
    {
        var persentage = character.currentHealth / character.maxHealth;//将百分比传输
        OnHealthChange(persentage);
        OnPowerChange(character);
    }






    public Image healthImage;
    public Image healthDelayImage;
    public Image powerImage;

    public void OnHealthChange(float persentage) 
    {
        healthImage.fillAmount = persentage;
    }

    public bool isRecovering;//体力值恢复
    public void OnPowerChange(Character character)
    {
        isRecovering = true;
        currentCharacter = character;
    }
    Character currentCharacter;

    private void Update()
    {
        if (healthDelayImage.fillAmount > healthImage.fillAmount) 
        {
            healthDelayImage.fillAmount -= Time.deltaTime*1.2f;//可以调整速度
        }


        if (isRecovering)
        {
            float percentage = currentCharacter.currentPower / currentCharacter.maxPower;
            powerImage.fillAmount = percentage;

            if (percentage >= 1)
            {
                isRecovering = false;
                return;
            }
        }

    }



    [Header("弹药UI")]
    public Transform bulletRoot;
    public GameObject bulletPrefab;

    public void RefreshAmmoUI(
     int currentAmmo,
     int maxAmmo
 )
    {
        foreach (Transform child in bulletRoot)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < currentAmmo; i++)
        {
            Instantiate(
                bulletPrefab,
                bulletRoot
            );
        }
    }

    #endregion
}
