using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public void Awake()
    {

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


    public GameObject healthBar;

    public GameObject PauseMenu;

    public void UpdateHealth(float currentHealth) 
    {
        switch (currentHealth) 
        {
            case 3:
                healthBar.transform.GetChild(0).gameObject.SetActive(true);
                healthBar.transform.GetChild(1).gameObject.SetActive(true);
                healthBar.transform.GetChild(2).gameObject.SetActive(true);
                break;

            case 2:
                healthBar.transform.GetChild(0).gameObject.SetActive(true);
                healthBar.transform.GetChild(1).gameObject.SetActive(true);
                healthBar.transform.GetChild(2).gameObject.SetActive(false);
                break;

            case 1:
                healthBar.transform.GetChild(0).gameObject.SetActive(true);
                healthBar.transform.GetChild(1).gameObject.SetActive(false);
                healthBar.transform.GetChild(2).gameObject.SetActive(false);
                break;

            case 0:
                healthBar.transform.GetChild(0).gameObject.SetActive(false);
                healthBar.transform.GetChild(1).gameObject.SetActive(false);
                healthBar.transform.GetChild(2).gameObject.SetActive(false);
                break;
        }
    }

    public void PauseGame() 
    {
        PauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        PauseMenu.SetActive(false);
        Time.timeScale = 1;
    }



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

}
