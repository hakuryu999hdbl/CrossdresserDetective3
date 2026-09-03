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

    void Awake()
    {
        AudioManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<AudioManager>();

    }

    #region Attack/Bullet 战斗系音效

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
                audioS.PlayOneShot(AudioManager.Attack_sword_clash1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_sword_clash2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.Attack_sword_clash3);
                break;
        }
    }//这个由ThrowableWeapon中代码调用

    public void _Attack_hit()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_hit1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_hit2);
                break;
        }
    }//这个由PlayerEnemy中代码调用

    public void _Attack_pick()
    {
        audioS.PlayOneShot(AudioManager.Attack_hit3);
    }//这个因为非常像抓取的声音



    //拍打声
    public void _Attack_pai()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.Attack_pai1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.Attack_pai2);
                break;
        }

    }

    public void _Attack_largeSword()
    {
        audioS.PlayOneShot(AudioManager.Attack_largeSword);
    }//这个由敌人脚本调用


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


    #endregion



    #region SE 泛用场景内音效

    //鞭打声
    public void _SE_Whip()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Whip_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Whip_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_Whip_3);
                break;
        }

    }

    //吸舔
    public void _SE_Xitian()
    {
        audioS.PlayOneShot(AudioManager.SE_Xitian);

    }

    //咽下
    public void _SE_Yanxia() { audioS.PlayOneShot(AudioManager.SE_Yanxia); }

    //抽插声
    public void _SE_Semen_1()
    {
        switch (Random.Range(0, 5))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Semen_1_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Semen_1_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_Semen_1_3);
                break;
            case 3:
                audioS.PlayOneShot(AudioManager.SE_Semen_1_4);
                break;
            case 4:
                audioS.PlayOneShot(AudioManager.SE_Semen_1_5);
                break;
        }
    }

    //手淫声
    public void _SE_Semen_2()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Semen_2_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Semen_2_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_Semen_2_3);
                break;
        }

    }

    //射精声
    public void _SE_Semen_3()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Semen_3_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Semen_3_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_Semen_3_3);
                break;
        }

    }

    //插入
    public void _SE_Semen_fuck_in() { audioS.PlayOneShot(AudioManager.SE_Semen_fuck_in); }

    //拔出
    public void _SE_Semen_fuck_out() { audioS.PlayOneShot(AudioManager.SE_Semen_fuck_out); }

    //潮吹喷水声
    public void _SE_Squirting()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Squirting_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Squirting_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_Squirting_3);
                break;
        }
    }

    //抚摸声
    public void _SE_LotionGauze()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_LotionGauze_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_LotionGauze_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_LotionGauze_3);
                break;
        }

    }



    public void _SE_Tentacle_slow()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_slow_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_slow_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_slow_3);
                break;
        }

    }

    public void _SE_Tentacle_middle()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_middle_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_middle_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_middle_3);
                break;
        }

    }

    public void _SE_Tentacle_quick()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_quick_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_quick_2);
                break;
            case 2:
                audioS.PlayOneShot(AudioManager.SE_Tentacle_quick_3);
                break;
        }

    }






    //走路和拘束的绳索
    public void _SE_Rope()
    {
        audioS.PlayOneShot(AudioManager.SE_Rope_1);
    }//代码调用

    //走路和拘束的锁链
    public void _SE_Chain()
    {
        switch (Random.Range(0, 2))
        {
            case 0:
                audioS.PlayOneShot(AudioManager.SE_Chain_1);
                break;
            case 1:
                audioS.PlayOneShot(AudioManager.SE_Chain_2);
                break;
        }
    }//代码调用




    public void _SE_Clothes()
    {
        audioS.PlayOneShot(AudioManager.SE_Clothes);
    }

    public void _SE_Tuo()
    {
        audioS.PlayOneShot(AudioManager.SE_Tuo);
    }

    //抓住后撕衣服
    public void _SE_Tear()
    {
        audioS.PlayOneShot(AudioManager.SE_Tear);
    }//代码调用

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

    public void _SE_Falldown() { audioS.PlayOneShot(AudioManager.SE_falldown); }



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




    #endregion



    #region 人物音效

    public void _YYY_attack()
    {

        AudioClip[] clips =
 {
        AudioManager.YYY_attack1,
        AudioManager.YYY_attack2,
        AudioManager.YYY_attack3,
        AudioManager.YYY_attack4
    };

        PlaySingleVoice(
            clips[Random.Range(0, clips.Length)]
        );
    }//单次触发，打断循环

    public void _YYY_die()
    {


        AudioClip[] clips =
     {
        AudioManager.YYY_die1,
        AudioManager.YYY_die2,
        AudioManager.YYY_die3,
        AudioManager.YYY_die4
    };

        PlaySingleVoice(
            clips[Random.Range(0, clips.Length)]
        );


    }//单次触发，打断循环

    public void _YYY_niao()
    {
        PlayVoiceLoop(
    VoiceLoopType.YYY_Niao,
    AudioManager.YYY_niao
);

    }//循环触发

    public void _YYY_duzui()
    {

        PlayVoiceLoop(
     VoiceLoopType.YYY_Duzui,
     AudioManager.YYY_duzui1,
     AudioManager.YYY_duzui2,
     AudioManager.YYY_duzui3,
     AudioManager.YYY_duzui4
 );


    }//循环触发

    public void _YYY_breath()
    {

        PlayVoiceLoop(
       VoiceLoopType.YYY_Breath,
       AudioManager.YYY_breath1,
       AudioManager.YYY_breath2,
       AudioManager.YYY_breath3
   );//AudioManager.YYY_breath4  单纯感觉喘息4不是非常适合



    }//循环触发

    public void _YYY_gasping()
    {

        PlayVoiceLoop(
          VoiceLoopType.YYY_Gasping,
          AudioManager.YYY_gasping1,
          AudioManager.YYY_gasping2
      );


    }//循环触发

    public void _YYY_gasping_quick()
    {

        PlayVoiceLoop(
        VoiceLoopType.YYY_GaspingQuick,
        AudioManager.YYY_gasping_quick1,
        AudioManager.YYY_gasping_quick2,
        AudioManager.YYY_gasping_quick3
    );


    }//循环触发

    public void _YYY_gasping_weak()
    {

        PlayVoiceLoop(
       VoiceLoopType.YYY_GaspingWeak,
       AudioManager.YYY_gasping_weak1,
       AudioManager.YYY_gasping_weak2
   );



    }//循环触发

    public void _YYY_scream_strong()
    {

        AudioClip[] clips =
        {
        AudioManager.YYY_scream_strong1,
        AudioManager.YYY_scream_strong2,
        AudioManager.YYY_scream_strong3,
        AudioManager.YYY_scream_strong4,
        AudioManager.YYY_scream_strong5,
        AudioManager.YYY_scream_strong6,
        AudioManager.YYY_scream_strong7,
        AudioManager.YYY_scream_strong8
    };

        PlaySingleVoice(
            clips[Random.Range(0, clips.Length)]
        );


    }//单次触发，打断循环



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
    public void _Girl_thankYou()
    {

        AudioClip[] clips =
{
       AudioManager.Girl_thankYou1,
        AudioManager.Girl_thankYou2
    };

        PlaySingleVoice(
            clips[Random.Range(0, clips.Length)]
        );

    }//单次触发，打断循环




    #endregion

    #region UI层音效

    public void _UI_Click()
    {
        audioS.PlayOneShot(AudioManager.UI_Click);
    }//这个由Player中代码调用
    #endregion


    #endregion


    #region 人物循环音系统

    public enum VoiceLoopType
    {
        None,

        YYY_Duzui,
        YYY_Breath,
        YYY_Gasping,
        YYY_GaspingQuick,
        YYY_GaspingWeak,
        YYY_Niao,

    }

    [Header("人物音声系统")]
    public AudioSource voiceAudioS;

    [SerializeField]
    private VoiceLoopType currentVoiceLoopType = VoiceLoopType.None;

    private AudioClip[] currentVoiceLoopClips;

    private Coroutine voiceLoopCoroutine;
    private Coroutine singleVoiceCoroutine;

    private bool loopPausedBySingle;
    private int lastVoiceLoopIndex = -1;


    private void PlayVoiceLoop(
    VoiceLoopType loopType,
    params AudioClip[] clips
)
    {

        if (muteVoice)
            return;

        if (voiceAudioS == null)
            return;

        if (loopType == VoiceLoopType.None)
            return;

        if (clips == null || clips.Length == 0)
            return;

        // 相同的循环音已经在持续，不重新开始
        if (currentVoiceLoopType == loopType &&
            voiceLoopCoroutine != null)
        {
            return;
        }

        // 新循环音替换旧循环音
        StopVoiceLoop();

        currentVoiceLoopType = loopType;
        currentVoiceLoopClips = clips;
        lastVoiceLoopIndex = -1;

        voiceLoopCoroutine = StartCoroutine(VoiceLoopRoutine());
    }


    //循环音方法
    private IEnumerator VoiceLoopRoutine()
    {
        while (currentVoiceLoopType != VoiceLoopType.None)
        {
            AudioClip clip = GetRandomVoiceLoopClip();

            if (clip == null)
            {
                yield return null;
                continue;
            }

            voiceAudioS.clip = clip;
            voiceAudioS.Play();

            while (voiceAudioS.isPlaying)
            {
                yield return null;
            }

            if (currentVoiceLoopType == VoiceLoopType.None)
                break;

            yield return null;
        }

        voiceLoopCoroutine = null;
    }

    private AudioClip GetRandomVoiceLoopClip()
    {
        if (currentVoiceLoopClips == null ||
            currentVoiceLoopClips.Length == 0)
        {
            return null;
        }

        if (currentVoiceLoopClips.Length == 1)
            return currentVoiceLoopClips[0];

        int index;

        do
        {
            index = Random.Range(0, currentVoiceLoopClips.Length);
        }
        while (index == lastVoiceLoopIndex);

        lastVoiceLoopIndex = index;

        return currentVoiceLoopClips[index];
    }


    //停止循环音

    public void _Voice_StopLoop()
    {
        StopVoiceLoop();
    }

    private void StopVoiceLoop()
    {
        currentVoiceLoopType = VoiceLoopType.None;
        currentVoiceLoopClips = null;
        lastVoiceLoopIndex = -1;
        loopPausedBySingle = false;

        if (voiceLoopCoroutine != null)
        {
            StopCoroutine(voiceLoopCoroutine);
            voiceLoopCoroutine = null;
        }

        if (voiceAudioS != null)
        {
            voiceAudioS.Stop();
            voiceAudioS.clip = null;
        }
    }


    //单次人物音
    private void PlaySingleVoice(AudioClip clip)
    {
        if (muteVoice)
            return;

        if (clip == null || voiceAudioS == null)
            return;

        if (singleVoiceCoroutine != null)
        {
            StopCoroutine(singleVoiceCoroutine);
            singleVoiceCoroutine = null;
        }

        singleVoiceCoroutine =
            StartCoroutine(SingleVoiceRoutine(clip));
    }

    private IEnumerator SingleVoiceRoutine(AudioClip clip)
    {
        bool hadLoop =
            currentVoiceLoopType != VoiceLoopType.None &&
            voiceAudioS.isPlaying;

        if (hadLoop)
        {
            voiceAudioS.Pause();
            loopPausedBySingle = true;
        }

        /*
         * 这里同一个AudioSource无法在暂停循环的同时播放单次音。
         * 所以必须临时保存循环进度。
         */

        AudioClip pausedLoopClip = null;
        float pausedLoopTime = 0f;

        if (loopPausedBySingle)
        {
            pausedLoopClip = voiceAudioS.clip;
            pausedLoopTime = voiceAudioS.time;

            voiceAudioS.Stop();
        }

        voiceAudioS.clip = clip;
        voiceAudioS.time = 0f;
        voiceAudioS.Play();

        while (voiceAudioS.isPlaying)
        {
            yield return null;
        }

        singleVoiceCoroutine = null;

        if (loopPausedBySingle &&
            currentVoiceLoopType != VoiceLoopType.None &&
            pausedLoopClip != null)
        {
            voiceAudioS.clip = pausedLoopClip;
            voiceAudioS.time = Mathf.Clamp(
                pausedLoopTime,
                0f,
                pausedLoopClip.length
            );

            voiceAudioS.Play();
        }

        loopPausedBySingle = false;
    }


    //声音完全屏蔽
    [Header("人物音声屏蔽")]
    public bool muteVoice = false;

    public void _Voice_Mute()
    {
        SetMuteVoice(true);
    }

    public void _Voice_UnMute()
    {
        SetMuteVoice(false);
    }

    public void SetMuteVoice(bool mute)
    {
        muteVoice = mute;

        if (mute)
        {
            // 停止循环
            StopVoiceLoop();

            // 停止正在进行的单次人物音协程
            if (singleVoiceCoroutine != null)
            {
                StopCoroutine(singleVoiceCoroutine);
                singleVoiceCoroutine = null;
            }

            // 彻底停止人物 AudioSource
            if (voiceAudioS != null)
            {
                voiceAudioS.Stop();
                voiceAudioS.clip = null;
            }

            loopPausedBySingle = false;
        }
    }


    #endregion
}
