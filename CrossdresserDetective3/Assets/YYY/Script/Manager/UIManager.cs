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

    public void Start()
    {
        OpenSetUp();
    }



    /// <summary>
    /// 暂停菜单
    /// </summary>
    #region

    [Header("暂停菜单")]
    public GameObject PauseMenu,PauseButton;//隐藏暂停按钮
    public GameObject PauseSetUpFirstSelected; //进入设置页面最先选中 X 或 Master Slider

    public bool isPaused=false;
    public PlayerController playerController;//玩家脚本
    public void TogglePause() 
    {

        if (isSetUp) { return; }//设置界面内把暂停界面挡住

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
        PauseMenu.SetActive(true); PauseButton.SetActive(false);

        playerController.DisableGameplayInput();  // 打开 UI 输入、设置默认选中，关闭玩家中的游戏设置

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(PauseSetUpFirstSelected);
    }

    public void ClosePause()
    {
        isPaused = false;
        PauseMenu.SetActive(false); PauseButton.SetActive(true);
        Time.timeScale = 1f;

        playerController.EnableGameplayInput();// 关闭 UI 输入

        PauseSetUpFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
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
        SceneManager.LoadScene("Menu");

    }//跳转编号场景


    #endregion



    /// <summary>
    /// 整备菜单
    /// </summary>
    #region

    [Header("整备菜单")]
    public GameObject SetUpMenu;
    public GameObject ContactMenu;
    public GameObject EquipMenu,WeaponMenu;
    public GameObject MeleeMenu, RangedMenu, ThrowableMenu;
    public GameObject ClothesMenu, GlovesMenu, SkirtMenu, PantiesMenu, StockingsMenu, ShoesMenu;

    //进入页面最先选中
    public GameObject SetUpFirstSelected; 
    public GameObject EquipFirstSelected; 
    public GameObject WeaponFirstSelected;

    public GameObject MeleeFirstSelected;
    public GameObject RangedFirstSelected;
    public GameObject ThrowableFirstSelected;

    public GameObject ClothesFirstSelected;
    public GameObject GlovesFirstSelected;
    public GameObject SkirtFirstSelected;
    public GameObject PantiesFirstSelected;
    public GameObject StockingsFirstSelected;
    public GameObject ShoesFirstSelected;


    public bool isSetUp = true;

    private int CurrentOpen;//-7鞋子菜单  -6丝袜菜单  -5内裤菜单 -4裙子菜单  -3手套菜单 -2衣服菜单  -1服装菜单  0整备主菜单  1武器菜单  2近战武器菜单  3远程武器菜单  4投掷武器菜单

    public void ToggleSetUp()
    {
        if (isSetUp)
        {
            CloseSetUp();
        }
        else
        {
            OpenSetUp();
        }
    }
    public void OpenSetUp()
    {
        isSetUp = true;
        CurrentOpen = 0;

        SetUpMenu.SetActive(true); PauseButton.SetActive(false);
        //Time.timeScale = 0f;

        playerController.DisableGameplayInput();  // 打开 UI 输入、设置默认选中，关闭玩家中的游戏设置

        GameFlowData.suppressNextSelectSound = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(SetUpFirstSelected);
    }

    public void CloseSetUp()
    {
        isSetUp = false;
        CurrentOpen = 0;

        SetUpMenu.SetActive(false); PauseButton.SetActive(true);
        //Time.timeScale = 1f;

        playerController.EnableGameplayInput();// 关闭 UI 输入

        GameFlowData.suppressNextSelectSound = true;

        SetUpFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
    }


    #region 服装菜单
    public void OpenEquipMenu()
    {
        EquipMenu.SetActive(true);
        ContactMenu.SetActive(false);

        CurrentOpen = -1;


        GameFlowData.suppressNextSelectSound = true;

        SetUpFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(EquipFirstSelected);
    }

    public void CloseEquipMenu()
    {
        EquipMenu.SetActive(false);
        ContactMenu.SetActive(true);

        CurrentOpen = 0;


        GameFlowData.suppressNextSelectSound = true;

        EquipFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(SetUpFirstSelected);
    }
    #endregion

    #region 武器菜单

    public void OpenWeaponMenu()
    {
        WeaponMenu.SetActive(true);
        ContactMenu.SetActive(false);

        CurrentOpen = 1;


        GameFlowData.suppressNextSelectSound = true;

        SetUpFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(WeaponFirstSelected);
    }

    public void CloseWeaponMenu()
    {
        WeaponMenu.SetActive(false);
        ContactMenu.SetActive(true);

        CurrentOpen = 0;


        GameFlowData.suppressNextSelectSound = true;

        WeaponFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(SetUpFirstSelected);
    }

    #endregion

    #region 近战武器菜单

    public void OpenMeleeMenu()
    {
        MeleeMenu.SetActive(true);
        WeaponMenu.SetActive(false);

        CurrentOpen = 2;


        GameFlowData.suppressNextSelectSound = true;

        WeaponFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(MeleeFirstSelected);
    }

    public void CloseMeleeMenu()
    {
        MeleeMenu.SetActive(false);
        WeaponMenu.SetActive(true);

        CurrentOpen = 1;


        GameFlowData.suppressNextSelectSound = true;

        MeleeFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(WeaponFirstSelected);
    }


    public void ChangeMelee(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Melee, index);
        playerController.attackType = 1;
    }

    #endregion

    #region 远程武器菜单

    public void OpenRangedMenu()
    {
        RangedMenu.SetActive(true);
        WeaponMenu.SetActive(false);

        CurrentOpen = 3;


        GameFlowData.suppressNextSelectSound = true;

        WeaponFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(RangedFirstSelected);
    }

    public void CloseRangedMenu()
    {
        RangedMenu.SetActive(false);
        WeaponMenu.SetActive(true);

        CurrentOpen = 1;


        GameFlowData.suppressNextSelectSound = true;

        RangedFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(WeaponFirstSelected);
    }

    public void ChangeRanged(int index)
    {

        switch (index) 
        {
            case 1:
                playerController.ChangeEquip(GameFlowData.EquipPart.Pistol, 1);
                playerController.attackType = -1;
                break;
            case 2:
                playerController.ChangeEquip(GameFlowData.EquipPart.Pistol, 2);
                playerController.attackType = -1;
                break;
            case 3:
                playerController.ChangeEquip(GameFlowData.EquipPart.Pistol, 3);
                playerController.attackType = -1;
                break;
            case 4:
                playerController.ChangeEquip(GameFlowData.EquipPart.Rifle, 1);
                playerController.attackType = -2;
                break;
            case 5:
                playerController.ChangeEquip(GameFlowData.EquipPart.Rifle, 2);
                playerController.attackType = -2;
                break;
        }

    }

    #endregion

    #region 投掷武器菜单

    public void OpenThrowableMenu()
    {
        ThrowableMenu.SetActive(true);
        WeaponMenu.SetActive(false);

        CurrentOpen = 4;


        GameFlowData.suppressNextSelectSound = true;

        WeaponFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ThrowableFirstSelected);
    }

    public void CloseThrowableMenu()
    {
        ThrowableMenu.SetActive(false);
        WeaponMenu.SetActive(true);

        CurrentOpen = 1;


        GameFlowData.suppressNextSelectSound = true;

        ThrowableFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);

        EventSystem.current.SetSelectedGameObject(WeaponFirstSelected);
    }

    public void ChangeThrowable(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Throw, index);
    }

    #endregion

    #region 衣服菜单

    public void OpenClothesMenu()
    {
        ClothesMenu.SetActive(true);
        EquipMenu.SetActive(false);

        CurrentOpen = -2;

        GameFlowData.suppressNextSelectSound = true;

        EquipFirstSelected = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ClothesFirstSelected);
    }

    public void CloseClothesMenu()
    {
        ClothesMenu.SetActive(false);
        EquipMenu.SetActive(true);

        CurrentOpen = -1;

        GameFlowData.suppressNextSelectSound = true;

        ClothesFirstSelected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(EquipFirstSelected);
    }


    public void ChangeClothes(int index) 
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Clothes, index);

        if (index == 3)
        {
            playerController.ChangeEquip(GameFlowData.EquipPart.Skirt, 0);
        }//兔女郎装不能穿裙子
    }


    #endregion

    #region 手套菜单

    public void OpenGlovesMenu()
    {
        GlovesMenu.SetActive(true);
        EquipMenu.SetActive(false);

        CurrentOpen = -3;

        GameFlowData.suppressNextSelectSound = true;

        EquipFirstSelected = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GlovesFirstSelected);
    }

    public void CloseGloveMenu()
    {
        GlovesMenu.SetActive(false);
        EquipMenu.SetActive(true);

        CurrentOpen = -1;

        GameFlowData.suppressNextSelectSound = true;

        GlovesFirstSelected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(EquipFirstSelected);
    }
    public void ChangeGloves(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Gloves, index);
    }
    #endregion

    #region 裙子菜单

    public void OpenSkirtMenu()
    {
        SkirtMenu.SetActive(true);
        EquipMenu.SetActive(false);

        CurrentOpen = -4;

        GameFlowData.suppressNextSelectSound = true;

        EquipFirstSelected = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(SkirtFirstSelected);
    }

    public void CloseSkirtMenu()
    {
        SkirtMenu.SetActive(false);
        EquipMenu.SetActive(true);

        CurrentOpen = -1;

        GameFlowData.suppressNextSelectSound = true;

        SkirtFirstSelected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(EquipFirstSelected);
    }

    public void ChangeSkirt(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Skirt, index);
    }

    #endregion

    #region 内裤菜单

    public void OpenPantiesMenu()
    {
        PantiesMenu.SetActive(true);
        EquipMenu.SetActive(false);

        CurrentOpen = -5;

        GameFlowData.suppressNextSelectSound = true;

        EquipFirstSelected = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(PantiesFirstSelected);
    }

    public void ClosePantiesMenu()
    {
        PantiesMenu.SetActive(false);
        EquipMenu.SetActive(true);

        CurrentOpen = -1;

        GameFlowData.suppressNextSelectSound = true;

        PantiesFirstSelected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(EquipFirstSelected);
    }

    public void ChangePanties(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Panties, index);
    }

    #endregion

    #region 丝袜菜单

    public void OpenStockingsMenu()
    {
        StockingsMenu.SetActive(true);
        EquipMenu.SetActive(false);

        CurrentOpen = -6;

        GameFlowData.suppressNextSelectSound = true;

        EquipFirstSelected = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(StockingsFirstSelected);
    }

    public void CloseStockingsMenu()
    {
        StockingsMenu.SetActive(false);
        EquipMenu.SetActive(true);

        CurrentOpen = -1;

        GameFlowData.suppressNextSelectSound = true;

        StockingsFirstSelected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(EquipFirstSelected);
    }

    public void ChangeStockings(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Stockings, index);




        if (index == 3)
        {
            playerController.ChangeEquip(GameFlowData.EquipPart.Panties, 3);

        }//裤袜固定部件
    }

    #endregion

    #region 鞋子菜单

    public void OpenShoesMenu()
    {
        ShoesMenu.SetActive(true);
        EquipMenu.SetActive(false);

        CurrentOpen = -7;

        GameFlowData.suppressNextSelectSound = true;

        EquipFirstSelected = EventSystem.current.currentSelectedGameObject;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ShoesFirstSelected);
    }

    public void CloseShoesMenu()
    {
        ShoesMenu.SetActive(false);
        EquipMenu.SetActive(true);

        CurrentOpen = -1;

        GameFlowData.suppressNextSelectSound = true;

        ShoesFirstSelected = EventSystem.current.currentSelectedGameObject;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(EquipFirstSelected);
    }

    public void ChangeShoes(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Shoes, index);
    }

    #endregion



    public void OnCancel() 
    {

        switch (CurrentOpen) 
        {
            case 4:
                CloseThrowableMenu();
                break;
            case 3:
                CloseRangedMenu();
                break;
            case 2:
                CloseMeleeMenu();
                break;
            case 1:
                CloseWeaponMenu();
                break;
            case -1:
                CloseEquipMenu();
                break;
            case -2:
                CloseClothesMenu();
                break;
            case -3:
                CloseGloveMenu();
                break;
            case -4:
                CloseSkirtMenu();
                break;
            case -5:
                ClosePantiesMenu();
                break;
            case -6:
                CloseStockingsMenu();
                break;
            case -7:
                CloseShoesMenu();
                break;
        }

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);

    }


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
        // 直接从游戏场景启动时，可能没有选择存档
        if (string.IsNullOrEmpty(GameFlowData.CurrentPlayer))
        {
            Debug.LogWarning("没有当前存档，跳过关卡保存。");
            return;
        }



        SaveData data = SaveManager.LoadGame(GameFlowData.CurrentPlayer);


        // 存档不存在 / 读取失败
        if (data == null)
        {
            Debug.LogWarning("读取存档失败，跳过关卡保存。");
            return;
        }


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
    GameObject bulletPrefab;
    public GameObject bulletPrefab_Pistol;
    public GameObject bulletPrefab_Rifle;

    public void RefreshAmmoUI(
     int currentAmmo,
     int maxAmmo
 )
    {
        if (playerController.attackType == -1){ bulletPrefab = bulletPrefab_Pistol; }
        if (playerController.attackType == -2) { bulletPrefab = bulletPrefab_Rifle; }

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
