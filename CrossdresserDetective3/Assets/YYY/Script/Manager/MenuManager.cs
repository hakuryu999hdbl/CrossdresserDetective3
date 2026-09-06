using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;


public class MenuManager : MonoBehaviour
{
    [System.Serializable]
    public class SaveThumbnailEntry
    {
        public string id;
        public Sprite sprite;
    }



    public static MenuManager instance;


    /// <summary>
    /// 多端输入
    /// </summary>
    #region
    [Header("多端输入")]
    public GameObject newGameButton;//开头默认选中
    private int CurrentOpen;//-2章节二 -1章节一 0主菜单  1设置菜单  2章节菜单  3存档菜单  4回想菜单  5确认删除存档菜单  6赞助者名单  7社群菜单

  

    private PlayerInputControl inputControl;


    private void Awake()
    {
        instance = this;

        inputControl = new PlayerInputControl();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(newGameButton);//开头设置默认按钮

        inputControl.UI.Cancel.started += OnCancel;
        inputControl.UI.Delete.started += OnDeleteSave;



        //Debug.Log("目前是否根据系统语言进行设置" + PlayerPrefs.GetInt("language_initialized"));//0无设置  1已经设置好
        Debug.Log("目前储存的语言" + PlayerPrefs.GetInt("language"));//0日语 1简体中文 2繁体中文 3英语 4韩语

        
    }
    private void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.BGM_Theme, true);

        switch (GameFlowData.returnPath)
        {
            case "chapter_1":
                OpenChapter_Number(1);
                break;

            case "cg":
                OpenGalleryMenu();
                break;

        }


        if (GameFlowData.returnPath != null) { ChangeCharacter(); }//小彩蛋，看完CG或者玩完关卡回到主菜单角色变化

        GameFlowData.returnPath = null; // 用完清掉


        if (Application.platform == RuntimePlatform.Android)
        {
            Debug.Log("当前是 Android");
        }
        else
        {
            Debug.Log("当前是 PC");

            StartSetDisplayMode();//根据存档设置对应屏幕以及分辨率

            GetResolutionIndex_Text();//设置屏幕分辨率文字

          
        }


    }
    private void OnEnable()
    {

        inputControl.Enable();

    }

    private void OnDisable()
    {
        inputControl.Disable();
    }

    // 🎮 Cancel键逻辑
    private void OnCancel(InputAction.CallbackContext ctx)
    {

        switch (CurrentOpen)
        {
            case 1:
                CloseSetting();
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;
            case 2:
                CloseChapter();
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;
            case 3:
                CloseSaveMenu();
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;
            case 4:
                CloseGalleryMenu();
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;
            case 5:
                CancelDeleteSave();
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;
            case 6:
                CloseCreditsMenu();
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;
            case 7:
                CloseCommunityMenu();
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;


            case -1:
                CloseChapter_Number(1);
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;
            case -2:
                CloseChapter_Number(2);
                AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
                break;

        }
    }

    private void OnDeleteSave(InputAction.CallbackContext ctx)
    {
        if (CurrentOpen!=3) return;

        if (CurrentSaveSlotUI == null) return;

        if (!SaveManager.Exists(CurrentSaveSlotUI.slotName))return;

        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Click);


        //CurrentSaveSlotUI.OnDeleteClicked();

        OpenDeleteConfirm(CurrentSaveSlotUI);
    }//删除当前存档

    #endregion


    /// <summary>
    /// 存档统合
    /// </summary>
    #region
    [Header("存档界面UI")]
    public SaveSlotUI CurrentSaveSlotUI;
    public SaveSlotUI Save_1, Save_2, Save_3;

    [Header("存档封面图片合集")]
    public List<SaveThumbnailEntry> saveThumbnails =
    new List<SaveThumbnailEntry>();

    public Sprite GetSaveThumbnail(string thumbnailId)
    {
        if (string.IsNullOrEmpty(thumbnailId))
            return null;

        for (int i = 0; i < saveThumbnails.Count; i++)
        {
            SaveThumbnailEntry entry = saveThumbnails[i];

            if (entry != null && entry.id == thumbnailId)
            {
                return entry.sprite;
            }
        }

        Debug.LogWarning(
            "MenuManager中没有登记存档封面：" +
            thumbnailId
        );

        return null;
    }

    public void OnConfirmNameInput()
    {
        if (CurrentSaveSlotUI != null)
        {

            // 新建存档
            SaveData newData = new SaveData(CurrentSaveSlotUI.slotName);

            newData.slotName = CurrentSaveSlotUI.slotName;//记住档的名字

            newData.InitStageData();//新建存档的时候就更新一下关卡记录
            newData.InitDefaultEquip();//新建存档的时候就更新一下默认服装防止裸体

            SaveManager.SaveGame(newData);

            CurrentSaveSlotUI.Refresh();//更新当前存档内容
        }

    }//玩家确定这个存档名称


    public void OpenSaveURL()
    {
        Application.OpenURL(Application.persistentDataPath);
    }//打开存档位置文件夹

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
        {
            OpenSaveURL();
        }
    }

    public void Delete_All()
    {
        Save_1.OnDeleteClicked();
        Save_2.OnDeleteClicked();
        Save_3.OnDeleteClicked();

        PlayerPrefs.DeleteAll();

    }//这是删除全部存档，这个不用了
    #endregion



    [Header("姐弟二人")]
    public Image Character;
    public Image Shadow;
    public Sprite Before, After;
    void ChangeCharacter() 
    {
        Character.sprite = After;
        Shadow.sprite = After;
    }

    /// <summary>
    /// 存档菜单
    /// </summary>
    #region
    [Header("存档菜单")]
    public GameObject SaveMenu;
    public GameObject SaveFirstSelected;//打开存菜单档默认选中（可变换）
    public GameObject SaveButton;//退出存档菜单默认选中
    public GameObject Prompt;//展示隐藏按键提示

    public void OpenSaveMenu()
    {
        SaveMenu.SetActive(true);
        MainMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(SaveFirstSelected);

        CurrentOpen = 3;

        Prompt.SetActive(true);
    }

    public void CloseSaveMenu()
    {
        SaveMenu.SetActive(false);
        MainMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        SaveFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(SaveButton);

        CurrentOpen = 0;

        Prompt.SetActive(false);
    }


    #endregion

    /// <summary>
    /// 确认删除存档菜单
    /// </summary>
    #region

    [Header("删除存档确认")]
    public GameObject DeleteConfirmMenu;
    public GameObject DeleteConfirmFirstSelected;
    public GameObject DeleteConfirmReturnSelected;

    private SaveSlotUI pendingDeleteSlot;

    public void OpenDeleteConfirm(SaveSlotUI slot)
    {
        if (slot == null)
            return;

        if (!SaveManager.Exists(slot.slotName))
            return;

        pendingDeleteSlot = slot;

        // 记录确认窗口关闭后，要重新选中的存档槽位
        DeleteConfirmReturnSelected =
            EventSystem.current.currentSelectedGameObject;

        DeleteConfirmMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(
            DeleteConfirmFirstSelected
        );

        CurrentOpen = 5;
    }//打开确认删除菜单
    public void CloseDeleteConfirm()
    {
        DeleteConfirmMenu.SetActive(false);

        pendingDeleteSlot = null;

        GameFlowData.suppressNextSelectSound = true;

        EventSystem.current.SetSelectedGameObject(null);

        if (DeleteConfirmReturnSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(
                DeleteConfirmReturnSelected
            );
        }
        else if (SaveFirstSelected != null)
        {
            EventSystem.current.SetSelectedGameObject(
                SaveFirstSelected
            );
        }

        CurrentOpen = 3;
    }//关闭确认删除菜单


    public void ConfirmDeleteSave()
    {
        if (pendingDeleteSlot != null)
        {
            pendingDeleteSlot.DeleteSaveImmediately();
        }

        AudioManager.Instance.PlayFX(
            AudioManager.Instance.UI_Click
        );

        CloseDeleteConfirm();
    }//确认删除

    public void CancelDeleteSave()
    {
        AudioManager.Instance.PlayFX(
            AudioManager.Instance.UI_Select
        );

        CloseDeleteConfirm();
    }//不删除

    #endregion


    /// <summary>
    /// 设置菜单
    /// </summary>
    #region
    [Header("设置菜单")]
    public GameObject MainMenu;

    public GameObject SettingMenu;
    public GameObject settingFirstSelected;//打开设置默认选中（可变换）
    public GameObject settingButton;//退出设置菜单默认选中


    public LanguageSelector languageSelector;//初始化设置的时候直接传输过去

    public GameObject SD_YYY_05;


    public void OpenSetting()
    {
        // SettingMenu.SetActive(true);
        // MainMenu.SetActive(false);
        //
        // GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        //
        // EventSystem.current.SetSelectedGameObject(null);
        // EventSystem.current.SetSelectedGameObject(settingFirstSelected);
        //
        // CurrentOpen = 1;

        SettingMenu.SetActive(true);
        MainMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;

        EventSystem.current.SetSelectedGameObject(null);

        if (settingFirstSelected != null &&
            settingFirstSelected.activeInHierarchy)
        {
            EventSystem.current.SetSelectedGameObject(
                settingFirstSelected
            );
        }
        else
        {
            Debug.LogWarning(
                "settingFirstSelected为空或未激活"
            );
        }

        CurrentOpen = 1;

        SD_YYY_05.SetActive(true);
    }

    public void CloseSetting()
    {
        // SettingMenu.SetActive(false);
        // MainMenu.SetActive(true);
        //
        // GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        //
        // settingFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        // EventSystem.current.SetSelectedGameObject(null);
        // EventSystem.current.SetSelectedGameObject(settingButton);
        //
        //
        //
        // CurrentOpen = 0;


        GameObject currentSelected =
        EventSystem.current.currentSelectedGameObject;

        // 只有确实选中了设置菜单中的有效按钮，才记录
        if (currentSelected != null &&
            currentSelected.activeInHierarchy &&
            currentSelected.transform.IsChildOf(
                SettingMenu.transform
            ))
        {
            settingFirstSelected = currentSelected;
        }

        SettingMenu.SetActive(false);
        MainMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingButton);

        CurrentOpen = 0;


        SD_YYY_05.SetActive(false);

    }


    public void DeleteAllData()
    {
        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Click);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Debug.Log("初始化");
    }


    #endregion


    /// <summary>
    /// 頁面設置UI显示
    /// </summary>
    #region
    [Header("画面显示方法")]
    public GameObject DisplayMode_1;//全屏
    public GameObject DisplayMode_2;//窗口



    void StartSetDisplayMode()
    {
        bool fullscreen = PlayerPrefs.GetInt("DisplayMode", 1) == 1;
        int resIndex = PlayerPrefs.GetInt("ResolutionIndex", 2); // 默认1080p

        currentMode = fullscreen ? DisplayMode.Fullscreen : DisplayMode.Windowed;

        //修改显示
        if (currentMode == DisplayMode.Fullscreen)
        {
            DisplayMode_1.SetActive(true);
            DisplayMode_2.SetActive(false);
        }
        else
        {
            DisplayMode_1.SetActive(false);
            DisplayMode_2.SetActive(true);
        }



        var res = supportedResolutions[resIndex];
        Screen.SetResolution(res.x, res.y, fullscreen);

    }//开始设置屏幕分辨率




    enum DisplayMode
    {
        Fullscreen,
        Windowed
    }

    DisplayMode currentMode;
    Resolution currentResolution;



    public void SetFullScreenOrWindowed()
    {
        if (currentMode == DisplayMode.Fullscreen)
        {
            SetDisplayMode(false);
        }
        else
        {
            SetDisplayMode(true);
        }
    }//设置屏幕模式活扣

    public void SetDisplayMode(bool fullscreen)
    {
        Screen.fullScreen = fullscreen;
        currentMode = fullscreen ? DisplayMode.Fullscreen : DisplayMode.Windowed;

        PlayerPrefs.SetInt("DisplayMode", fullscreen ? 1 : 0);



        //修改显示
        if (currentMode == DisplayMode.Fullscreen)
        {
            DisplayMode_1.SetActive(true);
            DisplayMode_2.SetActive(false);
        }
        else
        {
            DisplayMode_1.SetActive(false);
            DisplayMode_2.SetActive(true);
        }


    }//设置全屏或者窗口化

    Vector2Int[] supportedResolutions =
{
    new Vector2Int(3840, 2160),
    new Vector2Int(2560, 1440),
    new Vector2Int(1920, 1080),
    new Vector2Int(1600, 900),
    new Vector2Int(1280, 720),
};

    public void SetResolutionByIndex(int index)
    {
        var res = supportedResolutions[index];

        Screen.SetResolution(
            res.x,
            res.y,
            currentMode == DisplayMode.Fullscreen
        );

        // if (index == 0)
        // {
        //     //默认的就是基于当前屏幕分辨率
        //     InitResolutions();
        // }
        // else
        // {
        //     var res = supportedResolutions[index];
        //
        //     Screen.SetResolution(
        //         res.x,
        //         res.y,
        //         currentMode == DisplayMode.Fullscreen
        //     );
        // }

        PlayerPrefs.SetInt("ResolutionIndex", index);

        //设置屏幕分辨率文字
        GetResolutionIndex_Text();

    }//设置当前屏幕模式的分辨率


    public void ChangeResolution()
    {
        //读取当前分辨率编号，默认1920×1080（索引2）
        int index = PlayerPrefs.GetInt("ResolutionIndex", 2);

        //切换到下一个
        index++;

        //超过最后一个后回到第一个
        if (index >= supportedResolutions.Length)
        {
            index = 0;
        }

        var res = supportedResolutions[index];

        Screen.SetResolution(
            res.x,
            res.y,
            currentMode == DisplayMode.Fullscreen
        );

        PlayerPrefs.SetInt("ResolutionIndex", index);
        PlayerPrefs.Save();

        GetResolutionIndex_Text();
    }

    public Text ResolutionsText;

    public void GetResolutionIndex_Text()
    {
        //设置屏幕分辨率文字
        int index = PlayerPrefs.GetInt("ResolutionIndex");
        ResolutionsText.text = GetResolutionLabel(index).ToString();

    }//读取分辨率数字



    string GetResolutionLabel(int index)
    {
        var r = supportedResolutions[index];

        return $"{r.x}×{r.y}";
    }


    #endregion

    /// <summary>
    /// 回想菜单
    /// </summary>
    #region

    [Header("回想菜单")]
    public GameObject GalleryMenu;
    public GameObject GalleryFirstSelected;//打开存菜单档默认选中（可变换）
    public GameObject GalleryButton;//退出存档菜单默认选中

    public void OpenGalleryMenu()
    {
        GalleryMenu.SetActive(true);
        MainMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GalleryFirstSelected);

        CurrentOpen = 4;
    }

    public void CloseGalleryMenu()
    {
        GalleryMenu.SetActive(false);
        MainMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        GalleryFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(GalleryButton);

        CurrentOpen = 0;
    }


    public void ToCG(string Name)
    {

        GameFlowData.nextAreaId = Name;
        GameFlowData.returnPath = "cg";

        blackScreen.SetFadeIn();// 黑幕淡入
        Invoke(nameof(StartCG), 1f);

    }//跳转CG场景

    private void StartCG()
    {
        SceneManager.LoadScene("Spine");
    }

    #endregion


    /// <summary>
    /// 赞助名单菜单
    /// </summary>
    #region

    [Header("赞助名单菜单")]
    public GameObject CreditsMenu;
    public GameObject CreditsFirstSelected;//打开存菜单档默认选中（可变换）
    public GameObject CreditsButton;//退出存档菜单默认选中

    public GameObject SD_YYY_03;

    public void OpenCreditsMenu()
    {
        CreditsMenu.SetActive(true);
        MainMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CreditsFirstSelected);

        CurrentOpen = 6;

        SD_YYY_03.SetActive(true);
    }

    public void CloseCreditsMenu()
    {
        CreditsMenu.SetActive(false);
        MainMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        CreditsFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CreditsButton);

        CurrentOpen = 0;

        SD_YYY_03.SetActive(false);
    }





    #endregion


    /// <summary>
    /// 社群菜单
    /// </summary>
    #region

    [Header("社群菜单")]
    public GameObject CommunityMenu;
    public GameObject CommunityFirstSelected;//打开社群菜单档默认选中（可变换）
    public GameObject CommunityButton;//退出社群菜单默认选中


    public GameObject SD_YYY_02;

    public void OpenCommunityMenu()
    {
        CommunityMenu.SetActive(true);
        MainMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CommunityFirstSelected);

        CurrentOpen = 7;

        SD_YYY_02.SetActive(true);
    }

    public void CloseCommunityMenu()
    {
        CommunityMenu.SetActive(false);
        MainMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        CommunityFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(CommunityButton);

        CurrentOpen = 0;

        SD_YYY_02.SetActive(false);
    }



    public void OpenURL_Ci_en()
    {
        Application.OpenURL("https://ci-en.dlsite.com/creator/16247");

    }

    public void OpenURL_Patreon()
    {
        Application.OpenURL("https://www.patreon.com/c/FTGirl");


    }

    public void OpenURL_Discord()
    {
        Application.OpenURL("https://discord.gg/bc49G5Xcq9");
    }



    public void OpenURL_YYY()
    {
        Application.OpenURL("https://x.com/Detective_ye");

    }


    #endregion


    /// <summary>
    /// 关卡菜单
    /// </summary>
    #region
    [Header("关卡菜单")]
    public GameObject ChapterMenu;
    public GameObject ChapterFirstSelected;//打开章节默认选中（可变换）
    public GameObject ChapterButton;//退出章节菜单默认选中

    public void OpenChapter()
    {
        ChapterMenu.SetActive(true);
        SaveMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        EventSystem.current.SetSelectedGameObject(null);  
        EventSystem.current.SetSelectedGameObject(ChapterFirstSelected);

        CurrentOpen = 2;
    }

    public void CloseChapter()
    {

        ChapterMenu.SetActive(false);
        SaveMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        ChapterFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ChapterButton);

        CurrentOpen = 3;
    }

    [Header("关卡一菜单")]
    public GameObject Chapter_1_Menu;
    public GameObject Chapter_1_FirstSelected;//打开章节默认选中（可变换）
    public GameObject Chapter_1_Button;//退出章节一菜单默认选中
    [Header("关卡二菜单")]
    public GameObject Chapter_2_Menu;
    public GameObject Chapter_2_FirstSelected;//打开章节默认选中（可变换）
    public GameObject Chapter_2_Button;//退出章节二菜单默认选中

    public void OpenChapter_Number(int Number) 
    {
        RefreshChapter1Buttons();//读取更新关卡（每次只有在打开关卡进度的时候更新）
        MainMenu.SetActive(false);//这个主要用于从关卡回退到主菜单的时候隐藏

        switch (Number)
        {
            case 1:
                ChapterMenu.SetActive(false);
                Chapter_1_Menu.SetActive(true);

                GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(Chapter_1_FirstSelected);

                CurrentOpen = -1;

                break;
            case 2:
                ChapterMenu.SetActive(false);
                Chapter_2_Menu.SetActive(true);

                GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(Chapter_2_FirstSelected);

                CurrentOpen = -2;

                break;
        }
    }

    public void CloseChapter_Number(int Number) 
    {
        switch (Number)
        {
            case 1:
                ChapterMenu.SetActive(true);
                Chapter_1_Menu.SetActive(false);

                GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

                Chapter_1_FirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(Chapter_1_Button);

                CurrentOpen = 2;

                break;
            case 2:
                ChapterMenu.SetActive(true);
                Chapter_2_Menu.SetActive(false);

                GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

                Chapter_2_FirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(Chapter_2_Button);

                CurrentOpen = 2;

                break;
        }
    }

    #endregion


    /// <summary>
    /// 关卡解锁进度
    /// </summary>
    #region

    [Header("第一章关卡按钮")]
    public StageButtonUI[] chapter1Buttons;

    private SaveData currentData;

    public void ReadChapter()
    {
        currentData = SaveManager.LoadGame(GameFlowData.CurrentPlayer);
        currentData.InitStageData();//刷新调用
    }

    // 打开第一章时调用这个
    public void RefreshChapter1Buttons()
    {
        ReadChapter();
        RefreshChapterButtons(1, chapter1Buttons);
    }

    // 通用刷新：第几章 + 这一章的按钮数组
    public void RefreshChapterButtons(int chapter, StageButtonUI[] buttons)
    {
        int startIndex = (chapter - 1) * currentData.stagePerChapter;

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = startIndex + i;

            if (index >= currentData.stageStars.Length)
                break;

            int star = currentData.stageStars[index];

            buttons[i].SetStageState(star);
        }
    }

    #endregion



    /// <summary>
    /// 进入游戏
    /// </summary>
    #region
    public BlackScreen blackScreen;

    public void NewGame(int Chapter, int Stage)
    {
        nextChapter = Chapter;
        nextStage = Stage;

        blackScreen.SetFadeIn(); // 黑幕淡入
        Invoke(nameof(StartLevel), 1f);

    }//跳转编号场景
    private int nextChapter;
    private int nextStage;
    private void StartLevel()
    {
        GameFlowData.CurrentChapter = nextChapter;
        GameFlowData.CurrentStage = nextStage;



        //特殊情况的主线需要单独先进入AVG

        if (nextChapter == 1 && nextStage == 1)
        {
            GameFlowData.nextAreaId = "Introduce";
            StartCG();
        }
        else 
        {
            SceneManager.LoadScene("Level");
        }

    }


    public void QuitGame()
    {

        Application.Quit();
    }
    #endregion
}
