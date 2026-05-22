using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class MenuManager : MonoBehaviour
{

    /// <summary>
    /// 多端输入
    /// </summary>
    #region
    [Header("多端输入")]
    public GameObject newGameButton;//开头默认选中
    private int CurrentOpen;//-2章节二 -1章节一 0主菜单  1设置菜单  2章节菜单

  

    private PlayerInputControl inputControl;


    private void Awake()
    {
        inputControl = new PlayerInputControl();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(newGameButton);//开头设置默认按钮

        inputControl.UI.Cancel.started += OnCancel;

        //InitLanguageOnce();//根据系统设置语言


        //Debug.Log("目前是否根据系统语言进行设置" + PlayerPrefs.GetInt("language_initialized"));//0无设置  1已经设置好
        Debug.Log("目前储存的语言" + PlayerPrefs.GetInt("language"));//0日语 1简体中文 2繁体中文 3英语 4韩语


    }
    private void Start()
    {
        AudioManager.Instance.PlayBGM(AudioManager.Instance.BGM_Theme, true);
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

    public void OpenSetting()
    {
        SettingMenu.SetActive(true);
        MainMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingFirstSelected);

        CurrentOpen = 1;
    }

    public void CloseSetting()
    {
        SettingMenu.SetActive(false);
        MainMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        settingFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingButton);



        CurrentOpen = 0;
    }


    public void DeleteAllData()
    {
        AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Click);

        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Debug.Log("初始化");
    }


    void InitLanguageOnce()
    {
        //
        if (PlayerPrefs.HasKey("language_initialized"))
            return;

        int lang = DetectSystemLanguage();

        PlayerPrefs.SetInt("language", lang);
        PlayerPrefs.SetInt("language_initialized", 1);
        PlayerPrefs.Save();
    }

    int DetectSystemLanguage()
    {
        SystemLanguage sys = Application.systemLanguage;

        switch (sys)
        {
            case SystemLanguage.Japanese:
                return 0;
            case SystemLanguage.ChineseSimplified:
                return 1;
            case SystemLanguage.ChineseTraditional:
                return 2;
            case SystemLanguage.English:
                return 3;
            case SystemLanguage.Korean:
                return 4;

            default:
                return 0; //默认日语
        }
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
        MainMenu.SetActive(false);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        EventSystem.current.SetSelectedGameObject(null);  
        EventSystem.current.SetSelectedGameObject(ChapterFirstSelected);

        CurrentOpen = 2;
    }

    public void CloseChapter()
    {

        ChapterMenu.SetActive(false);
        MainMenu.SetActive(true);

        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音

        settingFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(ChapterButton);

        CurrentOpen = 0;
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

    public void NewGame()
    {

        SceneManager.LoadScene(1);

    }//跳转编号场景

    public void QuitGame()
    {

        Application.Quit();
    }
}
