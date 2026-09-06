using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashManager : MonoBehaviour
{
    public AudioSource audioS;

    public AudioClip FTgirl;



    void PlayFTgirl()
    {
        audioS.PlayOneShot(FTgirl);
    }

    void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }





    private void Start()
    {
        InitLanguageOnce();//根据系统设置语言
    }


    void InitLanguageOnce()
    {

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

            case SystemLanguage.Korean:
                return 4;

            default:
                return 3;
        }
    }
}
