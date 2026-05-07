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
    public GameObject settingButton;//退出设置菜单默认选中
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
        if (isSettingOpen)
        {
            CloseSetting();
            AudioManager.Instance.PlayFX(AudioManager.Instance.UI_Select);
        }

    }

    #endregion




    /// <summary>
    /// 设置菜单
    /// </summary>
    #region
    [Header("设置菜单")]
    private bool isSettingOpen = false;

    public GameObject MainMenu;
    public GameObject SettingMenu;
    public GameObject settingFirstSelected;//打开设置默认选中
    public LanguageSelector languageSelector;//初始化设置的时候直接传输过去

    public void OpenSetting()
    {
        SettingMenu.SetActive(true);
        MainMenu.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
        GameFlowData.suppressNextSelectSound = true;//吞掉当前选中音
        EventSystem.current.SetSelectedGameObject(settingFirstSelected);

        isSettingOpen = true;
    }

    public void CloseSetting()
    {
        settingFirstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(settingButton);


        SettingMenu.SetActive(false);
        MainMenu.SetActive(true);
        isSettingOpen = false;
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





    public void NewGame()
    {

        SceneManager.LoadScene(1);

    }//跳转编号场景

    public void QuitGame()
    {

        Application.Quit();
    }
}
