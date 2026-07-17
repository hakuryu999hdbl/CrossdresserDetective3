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
}