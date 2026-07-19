using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameEvent_Audio : MonoBehaviour
{
    /// <summary>
    /// 声音
    /// </summary>
    #region
    [Header("声音")]

    AudioManager AudioManager;
    public AudioSource audioS;

    void Start()
    {
        AudioManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<AudioManager>();
    }



    public void _Attack_sword_chop()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_sword_chop1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_sword_chop2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Attack_sword_chop3);
                break;
            case 3:
                audioS.PlayOneShot(AudioManager.Attack_katana);
                break;
        }
    }

    public void _Attack_katana_in() { audioS.PlayOneShot(AudioManager.Attack_katana_in); }//这个由Player中代码调用
    public void _Attack_katana_draw() { audioS.PlayOneShot(AudioManager.Attack_katana_draw); }//这个由Player中代码调用

    public void _Attack_sword_clash() 
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_sword_clash2);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_sword_clash3);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Attack_sword_clash4);
                break;
        }
    }//这个由ThrowableWeapon中代码调用

    //public void _Attack_blood1() { audioS.PlayOneShot(AudioManager.Attack_blood1); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    //public void _Attack_blood2() { audioS.PlayOneShot(AudioManager.Attack_blood2); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用
    //public void _Attack_blood3() { audioS.PlayOneShot(AudioManager.Attack_blood3); }//Spine帧事件没有，由敌人或玩家或者其他挂着FrameEvent脚本直接使用

    public void _Attack_largeSword() 
    {
        audioS.PlayOneShot(AudioManager.Attack_largeSword);
    }//这个由敌人脚本调用




    public void _Bullet_OutOfBullet() 
    {
        audioS.PlayOneShot(AudioManager.Bullet_OutOfBullet);
    }//这个由Player中代码调用   
    public void _Bullet_Pistol_1()
    {
        audioS.PlayOneShot(AudioManager.Bullet_Pistol_1);
    }//这个由Player中代码调用
    public void _Bullet_Pistol_2()
    {
        audioS.PlayOneShot(AudioManager.Bullet_Pistol_2);
    }//这个由Player中代码调用
    public void _Bullet_Pistol_3()
    {
        audioS.PlayOneShot(AudioManager.Bullet_Pistol_3);
    }//这个由Player中代码调用
    public void _Bullet_AK()
    {
        audioS.PlayOneShot(AudioManager.Bullet_AK);
    }//这个由Player中代码调用
    public void _Bullet_M4a1()
    {
        audioS.PlayOneShot(AudioManager.Bullet_M4a1);
    }//这个由Player中代码调用
    public void _Bullet_SD()
    {
        audioS.PlayOneShot(AudioManager.Bullet_SD);
    }//这个由Player中代码调用

    public void _Bullet_Pistol_Load()
    {
        audioS.PlayOneShot(AudioManager.Bullet_Pistol_Load);
    }
    public void _Bullet_SD_Load()
    {
        audioS.PlayOneShot(AudioManager.Bullet_SD_Load);
    }



    public void _SE_Clothes()
    {
        audioS.PlayOneShot(AudioManager.SE_Clothes);
    }

    public void _SE_Tuo()
    {
        audioS.PlayOneShot(AudioManager.SE_Tuo);
    }

    public void _SE_Tear() 
    {
        audioS.PlayOneShot(AudioManager.SE_Tear);
    }

    public void _SE_HighHeels_Walk()
    {
        if (Random.Range(0, 2) == 0)
        {
            audioS.PlayOneShot(AudioManager.SE_WalkClip_1);
        }
        else
        {
            audioS.PlayOneShot(AudioManager.SE_WalkClip_2);
        }

    }
    public void _SE_HighHeels_Run()
    {
        if (Random.Range(0, 2) == 0)
        {
            audioS.PlayOneShot(AudioManager.SE_RunClip_1);
        }
        else
        {
            audioS.PlayOneShot(AudioManager.SE_RunClip_2);
        }

    }

    public void _SE_Shose_Walk() 
    {
        if (Random.Range(0, 2) == 0)
        {
            audioS.PlayOneShot(AudioManager.SE_Man_WalkClip_1);
        }
        else
        {
            audioS.PlayOneShot(AudioManager.SE_Man_WalkClip_2);
        }
    }
    public void _SE_Shose_Run()
    {
        if (Random.Range(0, 2) == 0)
        {
            audioS.PlayOneShot(AudioManager.SE_Man_RunClip_1);
        }
        else
        {
            audioS.PlayOneShot(AudioManager.SE_Man_RunClip_2);
        }
    }

    public void _SE_falldown() { audioS.PlayOneShot(AudioManager.SE_falldown); }



    public bool isCage = false;

    public void _SE_Door_Open()
    {
        if (isCage) 
        {
            audioS.PlayOneShot(AudioManager.SE_Cage);
        }
        else
        {
            audioS.PlayOneShot(AudioManager.SE_Door_Open);
        }
      
    }
    public void _SE_Door_Close()
    {
        if (isCage)
        {
            audioS.PlayOneShot(AudioManager.SE_Cage);
        }
        else
        {
            audioS.PlayOneShot(AudioManager.SE_Door_Close);
        }
       
    }

    public void _YYY_attack() 
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.YYY_attack1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.YYY_attack2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.YYY_attack3);
                break;
            case 3:
                audioS.PlayOneShot(AudioManager.YYY_attack4);
                break;
        }
    }
    public void _YYY_die()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.YYY_die1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.YYY_die2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.YYY_die3);
                break;
            case 3:
                audioS.PlayOneShot(AudioManager.YYY_die4);
                break;
        }
    }


    public void _YYY_duzui()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.YYY_duzui1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.YYY_duzui2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.YYY_duzui3);
                break;
            case 3:
                audioS.PlayOneShot(AudioManager.YYY_duzui4);
                break;
        }
    }


    public void _Man_attack()
    {
        audioS.PlayOneShot(AudioManager.Man_attack1);
    }
    public void _Man_die()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Man_die1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Man_die2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Man_die3);
                break;
            case 3:
                audioS.PlayOneShot(AudioManager.Man_die4);
                break;
        }
    }
    public void _Girl_attack()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Girl_attack1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Girl_attack2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Girl_attack3);
                break;
        }
    }
    public void _Girl_die()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Girl_die1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Girl_die2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Girl_die3);
                break;
        }
    }

    public void _Attack_blood() 
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_blood1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_blood2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Attack_blood3);
                break;
        }
    }//这个由Player和Enemy中代码各自调用

    public void _Attack_bomb_bounce()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_bomb_bounce_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_bomb_bounce_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Attack_bomb_bounce_3);
                break;
        }
    }//这个由Bomb中代码调用


    public void _UI_Click()
    {
        audioS.PlayOneShot(AudioManager.UI_Click);
    }//这个由Player中代码调用



    #endregion
}
