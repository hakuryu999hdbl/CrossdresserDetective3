using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(Collider2D))]
public class TimelineTrigger : MonoBehaviour
{
    [Header("Timeline")]
    [SerializeField] private PlayableDirector director;

    [Header("触发设置")]
    [SerializeField] private bool triggerOnlyOnce = true;
    [SerializeField] private bool disableColliderAfterTrigger = true;

    private bool alreadyTriggered;
    private PlayerController player;
    private Collider2D triggerCollider;

    private void Awake()
    {
        triggerCollider = GetComponent<Collider2D>();
        triggerCollider.isTrigger = true;

        if (director == null)
            director = GetComponent<PlayableDirector>();
    }

    private void OnEnable()
    {
        if (director != null)
            director.stopped += OnTimelineStopped;
    }

    private void OnDisable()
    {
        if (director != null)
            director.stopped -= OnTimelineStopped;

        // 防止物体突然被关闭，玩家永远无法恢复
        RestorePlayer();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnlyOnce && alreadyTriggered)
            return;

        PlayerController enteredPlayer =
            other.GetComponentInParent<PlayerController>();

        if (enteredPlayer == null)
            return;

        if (director == null)
        {
            Debug.LogError(
                $"{name} 没有指定 PlayableDirector。",
                this
            );
            return;
        }

        player = GameManager.instance != null
            ? GameManager.instance.player
            : enteredPlayer;

        if (player == null)
            player = enteredPlayer;

        alreadyTriggered = true;

        player.EnterCutscene();


        // ========== 新增：注册到 UIManager ==========
        if (UIManager.instance != null)
            UIManager.instance.RegisterTimeline(this);
        // ==========================================

        director.gameObject.SetActive(true);
        director.time = 0;
        director.Play();

        if (disableColliderAfterTrigger && triggerOnlyOnce)
            triggerCollider.enabled = false;

        SetInit();//这段过场动画需要的填入
    }

    private void OnTimelineStopped(PlayableDirector stoppedDirector)
    {
        // ========== 新增：注销 ==========
        if (UIManager.instance != null)
            UIManager.instance.UnregisterTimeline(this);
        // ==============================

        RestorePlayer();
    }

    private void RestorePlayer()
    {
        if (player == null)
            return;

        player.ExitCutscene();
        player = null;
    }

    /// <summary>
    /// 给跳过按钮或其他脚本调用。
    /// </summary>
    /// 



    public void SkipTimeline()
    {
        if (director == null)
            return;




        director.time = director.duration;
        director.Evaluate();// ← 这里会强制把中间所有还没播到的 Signal / Marker 全部执行一遍
        director.Stop();




        // 保险注销（Stop 会触发 stopped，但双重保险无害）
        if (UIManager.instance != null)
            UIManager.instance.UnregisterTimeline(this);

        Invoke(nameof(SetFadeOutOver), 0.1f);
    }

    void SetFadeOutOver() 
    {
        UIManager.instance.BlackScreen_FadeIn.SetActive(false);
    }




    /// <summary>
    /// 允许重复触发时，手动重置。
    /// </summary>
    public void ResetTrigger()
    {
        alreadyTriggered = false;

        if (triggerCollider != null)
            triggerCollider.enabled = true;
    }




    /// <summary>
    /// TimeLine中端帧事件触发
    /// </summary>
    [Header("TimeLine中端帧事件触发")]
    public string Chapter;

    public void SetInit() 
    {
        switch (Chapter) 
        {

            case "Chapter_01_1":
                UIManager.instance.Init_Chapter_01_1();
                break;
            case "Chapter_01_2":
                UIManager.instance.Init_Chapter_01_2();
                break;
            case "Chapter_01_3":
                UIManager.instance.Init_Chapter_01_3();
                break;
        }
    }



    public void SetBlackScreen_FadeIn() 
    {
        UIManager.instance.BlackScreen_FadeIn.SetActive(true);
    }
    public void SetBlackScreen_FadeOut()
    {
        UIManager.instance.BlackScreen_FadeOut.SetActive(true);
    }
    public void ShowText() 
    {
        UIManager.instance.ShowNext();
    }




    #region 玩家控制

  
    public void SetPlayerAnim_Washing() 
    {
        GameManager.instance.player.playerAnimation.PlayWashing();
    }

    public void SetPlayerAnim_Undressing()
    {
        GameManager.instance.player.playerAnimation.PlayUndressing();
    }

    public void SetPlayerAnim_Walking()
    {
        GameManager.instance.player.playerAnimation.PlayWalking();
    }

    public void SetPlayerAnim_Runing()
    {
        GameManager.instance.player.playerAnimation.PlayRuning();
    }


    public void SetPlayerAnim_Idle()
    {
        GameManager.instance.player.playerAnimation.PlayIdle();
    }
    public void SetPlayerAnim_Crouch()
    {
        GameManager.instance.player.playerAnimation.PlayCrouch();
    }


    public void SetPlayer_Turn() 
    {
        GameManager.instance.player.playerAnimation.SetPlayer_Turn();
    }

    public GameObject PlayerSlot;
    public void SetPlayerToSlot() 
    {
        // 1. 将玩家的父级设置为 PlayerSlot 的 transform
        GameManager.instance.player.transform.SetParent(PlayerSlot.transform);

        // 2. （可选）将玩家的本地坐标和旋转归零，使其完美对齐到插槽中心
        //GameManager.instance.player.transform.localPosition = Vector3.zero;

        // 2. 记住当前的本地坐标
        Vector3 currentLocalPos = GameManager.instance.player.transform.localPosition;

        // 3. 重新赋值：X 强制归零，Y 和 Z 保持它此时的相对位置
        GameManager.instance.player.transform.localPosition = new Vector3(0f, currentLocalPos.y, currentLocalPos.z);

        GameManager.instance.player.transform.localRotation = Quaternion.identity;
    }

    public void SetPlayerOutSlot()
    {

        // 将父级设置为 null，玩家就会移出插槽，回到场景的根目录
        GameManager.instance.player.transform.SetParent(null);
    }

    public void SetPlayer_Hide()
    {
        GameManager.instance.player.playerAnimation.gameObject.SetActive(false);
    }
    public void SetPlayer_Show()
    {
        GameManager.instance.player.playerAnimation.gameObject.SetActive(true);

        //这个一般来说在过场动画最后出现
        GameManager.instance.player.transform.position = PlayerSlot.transform.position;
    }





    public void SetPlayer_Clothes_01() 
    {
        GameManager.instance.player.frameEvent.SetPlayer_Clothes_01();
    }//主角赤裸绳子捆绑状态

    public void SetPlayer_Clothes_02()
    {
        GameManager.instance.player.frameEvent.SetPlayer_Clothes_02();
    }//主角赤裸状态

    public void SetPlayer_Clothes_03()
    {
        GameManager.instance.player.frameEvent.SetPlayer_Clothes_03();
    }//主角赤裸状态

    public void SetPlayer_EnterBondage()
    {
        GameManager.instance.player.EnterBondageState();

        UIManager.instance.InputPrompt_1.SetActive(false);
        UIManager.instance.InputPrompt_2.SetActive(true);

    }//进入拘束状态


    #endregion


}