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


        director.gameObject.SetActive(true);
        director.time = 0;
        director.Play();

        if (disableColliderAfterTrigger && triggerOnlyOnce)
            triggerCollider.enabled = false;
    }

    private void OnTimelineStopped(PlayableDirector stoppedDirector)
    {
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
    public void SkipTimeline()
    {
        if (director == null)
            return;

        director.time = director.duration;
        director.Evaluate();
        director.Stop();
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

    #endregion


    #region 敌人_1_控制

    public Animator Enemy_1;
    public FrameEvent Enemy_1_FrameEvent;

    public void SetEnemy_1_Clothes_01() 
    {
        Enemy_1_FrameEvent.Story_Clothes_Man_01();
    }

    public void SetEnemy_1_Anim_Idle()
    {
        Enemy_1.Play("Story_Idle", 0, 0f);
        Enemy_1.Update(0f);
    }

    public void SetEnemy_1_Anim_Walking()
    {
        Enemy_1.Play("Story_Walk", 0, 0f);
        Enemy_1.Update(0f);
    }

    public void SetEnemy_1_Anim_TurnCrouch()
    {
        Enemy_1.Play("Story_TurnCrouch", 0, 0f);
        Enemy_1.Update(0f);
    }

    #endregion

    #region 敌人_2_控制
    public Animator Enemy_2;
    public FrameEvent Enemy_2_FrameEvent;

    public void SetEnemy_2_Clothes_01()
    {
        Enemy_2_FrameEvent.Story_Clothes_Man_01();
    }

    public void SetEnemy_2_Anim_Idle()
    {
        Enemy_2.Play("Story_Idle", 0, 0f);
        Enemy_2.Update(0f);
    }
    public void SetEnemy_2_Anim_Walking()
    {
        Enemy_2.Play("Story_Walk", 0, 0f);
        Enemy_2.Update(0f);
    }

    #endregion
}