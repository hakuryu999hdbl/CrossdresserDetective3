using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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


        inputControl = new PlayerInputControl();
        inputControl.UI.Cancel.started += OnCancel;
    }



    /// <summary>
    /// 暂停菜单
    /// </summary>
    #region

    [Header("暂停菜单")]
    public GameObject PauseMenu;
    public GameObject firstSelected; //进入设置页面最先选中 X 或 Master Slider
    private PlayerInputControl inputControl;//UI端多端输入

    private bool isPaused=false;
    public PlayerController playerController;
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

        playerController.DisableGameplayInput();
        // 打开 UI 输入、设置默认选中
        inputControl.Enable();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelected);
    }

    public void ClosePause()
    {
        isPaused = false;
        PauseMenu.SetActive(false);
        Time.timeScale = 1f;

        playerController.EnableGameplayInput();
        // 关闭 UI 输入
        inputControl.Disable();
        firstSelected = EventSystem.current.currentSelectedGameObject;//记录上一次你选中的位置
        EventSystem.current.SetSelectedGameObject(null);
    }
    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);

    }//跳转编号场景

    private void OnCancel(InputAction.CallbackContext ctx)
    {
        if (isPaused)
        {
            ClosePause();
        }

    }

    #endregion







    public Slider bossHealthBar;
    public GameObject gameOverPanel;
    public void SetBossHealth(float health)
    {
        bossHealthBar.maxValue = health;
    }
    
    public void UpdateBossHealth(float health)
    {
        bossHealthBar.value = health;
    }

    public void GameOverUI(bool playerDead)
    {
        gameOverPanel.SetActive(playerDead);
    }




















    /// <summary>
    /// 生命值，体力值等UI
    /// </summary>
    #region

    [Header("事件监听")]
    public CharacterEventSO healthEvent;

    private void OnEnable()
    {
        healthEvent.OnEventRaised += OnHealthEvent;
        inputControl.Enable();
    }
    private void OnDisable()
    {
        healthEvent.OnEventRaised -= OnHealthEvent;
        inputControl.Disable();
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
    #endregion
}
