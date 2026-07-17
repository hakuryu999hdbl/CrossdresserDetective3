using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{


    /// <summary>
    /// 声音控制
    /// </summary>
    #region
    public AudioMixer Mixer;

    public Slider masterSlider;
    public Slider bgmSlider;
    public Slider fxSlider;

    public void SetMasterVolume(float value)
    {
        Mixer.SetFloat("MasterVolume", value);
    }

    public void SetBGMVolume(float value)
    {
        Mixer.SetFloat("BGMVolume", value);
    }

    public void SetFXVolume(float value)
    {
        Mixer.SetFloat("FXVolume", value);
    }




    #endregion



    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
    }



    public AudioSource bgmSource;
    public AudioSource fxSource;

    // 🎵 播放BGM（可循环）
    public void PlayBGM(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        bgmSource.clip = clip;
        bgmSource.loop = loop;
        bgmSource.Play();
    }

    // 🔊 播放音效（不打断）
    public void PlayFX(AudioClip clip)
    {
        if (clip == null) return;

        fxSource.PlayOneShot(clip);
    }//这个主要由UI层按键触发，局内通过FrameEvent自带的AuidoSoure播放范围音效）



    /// <summary>
    /// 声音
    /// </summary>
    #region
    [Header("效果音")]
    public AudioClip BGM_Theme;
    public AudioClip BGM_Level_1;

    public AudioClip Attack_sword_chop1, Attack_sword_chop2, Attack_sword_chop3, Attack_katana, Attack_katana_in, Attack_katana_draw,
                     Attack_sword_clash2, Attack_sword_clash3, Attack_sword_clash4,
                     Attack_blood1, Attack_blood2, Attack_blood3,
                     Attack_bomb_bounce_1, Attack_bomb_bounce_2, Attack_bomb_bounce_3;

    public AudioClip　Attack_largeSword;

    public AudioClip Bullet_OutOfBullet;
    public AudioClip Bullet_Wall;
    public AudioClip Bullet_Pistol_1, Bullet_Pistol_2, Bullet_Pistol_3;
    public AudioClip Bullet_Pistol_Load;
    public AudioClip Bullet_AK, Bullet_M4a1, Bullet_SD;
    public AudioClip Bullet_SD_Load;

    public AudioClip YYY_attack1, YYY_attack2, YYY_attack3, YYY_attack4;
    public AudioClip YYY_die1, YYY_die2, YYY_die3, YYY_die4;

    public AudioClip Man_die1, Man_die2, Man_die3, Man_die4;
    public AudioClip Man_attack1;



    public AudioClip Girl_attack1, Girl_attack2, Girl_attack3;
    public AudioClip Girl_die1, Girl_die2, Girl_die3;


    public AudioClip SE_Clothes, SE_Tuo, SE_Tear;
    public AudioClip SE_WalkClip_1, SE_WalkClip_2, SE_RunClip_1, SE_RunClip_2;
    public AudioClip SE_Man_WalkClip_1, SE_Man_WalkClip_2, SE_Man_RunClip_1, SE_Man_RunClip_2;
    public AudioClip SE_falldown;
    public AudioClip SE_Door_Open, SE_Door_Close, SE_Cage;
    public AudioClip SE_Button, SE_Elevator_1, SE_Elevator_2;

    public AudioClip UI_Click, UI_Select;
    #endregion







}
