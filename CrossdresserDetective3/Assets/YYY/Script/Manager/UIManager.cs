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
        Time.timeScale = 1f;//防止重刷场景前的暂停

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

        UI_CameraChangeAll();
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


        //每次关上界面的时候，让放大图出现一个进来的动画
        anim.Play("Show", 0, 0f);


        UI_CameraChangeMiddle();


        if (playerController.attackType == -10)
        {
            playerController.attackType = playerController.meleeSlot;
        }//炸弹动画被武器槽替换
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

        UI_CameraChangeMiddle();
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

        UI_CameraChangeAll();
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

        UI_CameraChangeMiddle();
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

        UI_CameraChangeAll();
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

        UI_CameraChangeMiddleLeft();
        playerController.UI_anim.SetTrigger("Change_2");
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

        UI_CameraChangeMiddle();
        playerController.UI_anim.SetTrigger("Change");
    }


    public void ChangeMelee(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Melee, index);
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

        UI_CameraChangeMiddleLeft();
        playerController.UI_anim.SetTrigger("Change_2");
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

        UI_CameraChangeMiddle();
        playerController.UI_anim.SetTrigger("Change");
    }

    public void ChangeRanged(int index)
    {

        switch (index) 
        {
            case 0:
                //步枪手枪哪个变成0都不影响
                playerController.ChangeEquip(GameFlowData.EquipPart.Pistol, 0);
                playerController.ChangeEquip(GameFlowData.EquipPart.Rifle, 0);
                break;


            case 1:
                playerController.ChangeEquip(GameFlowData.EquipPart.Pistol, 1);
                break;
            case 2:
                playerController.ChangeEquip(GameFlowData.EquipPart.Pistol, 2);
                break;
            case 3:
                playerController.ChangeEquip(GameFlowData.EquipPart.Pistol, 3);
                break;




            case 11:
                playerController.ChangeEquip(GameFlowData.EquipPart.Rifle, 1);
                break;
            case 12:
                playerController.ChangeEquip(GameFlowData.EquipPart.Rifle, 2);
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

        UI_CameraChangeMiddleLeft();

        playerController.UI_anim.SetTrigger("Change_2");
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

        UI_CameraChangeMiddle();
      
        playerController.UI_anim.SetTrigger("Change");
    }

    public void ChangeThrowable(int index)
    {
        if (index == 0)
        {
            //站姿变化
            playerController.ChangeEquip(GameFlowData.EquipPart.Throw, 0);
        }
        else
        {
            playerController.ChangeEquip(GameFlowData.EquipPart.Throw, index);
            playerController.attackType = -10;//仅仅是为了触发炸弹装备动画
        }

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

        UI_CameraChangeMiddleDown();
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


        UI_CameraChangeMiddle();
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

        UI_CameraChangeMiddleDown();
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

        UI_CameraChangeMiddle();
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

        UI_CameraChangeDown();
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

        UI_CameraChangeMiddle();
    }

    public void ChangeStockings(int index)
    {
        playerController.ChangeEquip(GameFlowData.EquipPart.Stockings, index);

      
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

        UI_CameraChangeDown();
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

        UI_CameraChangeMiddle();
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
    /// 放大显示人体和放大镜头控制
    /// </summary>
    #region
    [Header("放大显示人体")]
    public Animator anim;
    public Animator anim_Camera;
    public void ShowPortrait()
    {
        anim.SetBool("isShow", true);
    }

    public void HidePortrait()
    {
        anim.SetBool("isShow", false);
    }

    public void UI_CameraChangeMiddle() 
    {
        anim_Camera.SetInteger("CameraWork", 0);
    }
    public void UI_CameraChangeDown()
    {
        anim_Camera.SetInteger("CameraWork", 1);
    }
    public void UI_CameraChangeAll()
    {
        anim_Camera.SetInteger("CameraWork", 2);
    }
    public void UI_CameraChangeMiddleDown()
    {
        anim_Camera.SetInteger("CameraWork", 3);
    }

    public void UI_CameraChangeMiddleLeft()
    {
        anim_Camera.SetInteger("CameraWork", 4);
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


        data.InitStageData();//通关触发

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
    /// 生命值，体力值，弹药，武器等UI
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


    public Image throwChargeBar;//投掷蓄力槽
    public Animator throwUIAnim;
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


        throwChargeBar.fillAmount = playerController.throwCharge;

        throwUIAnim.SetBool("Show",playerController.isHoldingThrow);
    }







    [Header("弹药UI")]
    public Transform bulletRoot;
    GameObject bulletPrefab;
    public GameObject bulletPrefab_Pistol;
    public GameObject bulletPrefab_Rifle;

    public GameObject magazineIcon;
    public Text magazineText;

    public void RefreshAmmoUI(
     int currentAmmo,
     int maxAmmo, int magazineCount
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

        magazineText.text = "X" + magazineCount.ToString();
       
     
    }



    [Header("近战UI")]
    public GameObject[] meleeIcons;

    [Header("远程UI")]
    public GameObject[] pistolIcons;
    public GameObject[] rifleIcons;
  

    [Header("投掷UI")]
    public GameObject[] throwableIcons;

    public void RefreshWeaponSlotUI(PlayerController player)
    {
        foreach (GameObject obj in meleeIcons)
            obj.SetActive(false);

        foreach (GameObject obj in pistolIcons)
            obj.SetActive(false);

        foreach (GameObject obj in rifleIcons)
            obj.SetActive(false);

        foreach (GameObject obj in throwableIcons)
            obj.SetActive(false);


        //近战(空手不显示)
        switch (player.meleeType) 
        {
            case 1:
                meleeIcons[0].SetActive(true);
                break;

            case 2:
                meleeIcons[1].SetActive(true);
                break;
            case 3:
                meleeIcons[2].SetActive(true);
                break;
        }




        //远程
        if(player.rangedSlot == -1)
        {
            switch (player.pistolType)
            {
                case 1:
                    pistolIcons[0].SetActive(true);
                    break;
                case 2:
                    pistolIcons[1].SetActive(true);
                    break;
                case 3:
                    pistolIcons[2].SetActive(true);
                    break;
            }
        }
        if (player.rangedSlot == -2)
        {
            switch (player.rifleType)
            {
                case 1:
                    rifleIcons[0].SetActive(true);
                    break;
                case 2:
                    rifleIcons[1].SetActive(true);
                    break;
            }
        }

        magazineIcon.SetActive(playerController.attackType < 0);//显示隐藏弹夹数量
        bulletRoot.gameObject.SetActive(playerController.attackType < 0);//显示隐藏子弹数量

        //投掷
        switch (player.throwType)
        {
            case 1:
                throwableIcons[0].SetActive(true);
                break;
            case 2:
                throwableIcons[1].SetActive(true);
                break;
            case 3:
                throwableIcons[2].SetActive(true);
                break;
        }

    }

    #endregion
}
