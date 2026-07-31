using System.Collections;
using UnityEngine;

public class RescueTarget : MonoBehaviour, IInteractable
{
    public enum RescueState
    {
        Bound,          // 已经被跪绑、吊绑，可以直接救
        BeingAbused,    // 正在被敌人侵犯或鞭打
        Fighting,       // 已生成敌人，等待敌人死亡
        Rescued         // 已获救
    }

    [Header("当前状态")]
    public RescueState state = RescueState.Bound;
    public RBQController rbqController;


    [Header("守卫敌人")]
    public GameObject enemyPrefab;//暂时只有男性Enemy_1
    public Transform enemySpawnPoint;

    private EnemyController spawnedEnemy;
    private string interactableTag;
    private bool registered;

    private void Awake()
    {
        interactableTag = gameObject.tag;
    }

    private void Start()
    {
        RegisterTarget();//告诉GameManager当前为救出模式
        RefreshAnimation();//随机拘束、调教中并告诉rbqController


        // 正在受辱时不能直接按E
        if (state == RescueState.BeingAbused)
        {
            gameObject.tag = "Untagged";
        }

    }

    private void RegisterTarget()
    {
        if (registered)
            return;

        registered = true;

        GameManager.instance.RegisterRescueTarget();
    }//告诉GameManager当前为救出模式

    private void RefreshAnimation()
    {

        if (Random.value > 0.5)
        {
            state = RescueState.BeingAbused;
        }
        else
        {
            state = RescueState.Bound;
        }


        switch (state)
        {
            case RescueState.Bound:
                rbqController.BoundAnimation();
                break;

            case RescueState.BeingAbused:
                rbqController.AbuseAnimation();
                break;

            case RescueState.Fighting:
                rbqController.BoundAnimation();
                break;
        }
    }



    public void TriggerAction()
    {
        switch (state)
        {
            case RescueState.Bound:
                Rescue();
                break;

            case RescueState.BeingAbused:
                // 正在受辱状态由玩家接近自动触发敌人
                break;

            case RescueState.Fighting:
            case RescueState.Rescued:

                AudioManager.Instance.PlayFX(AudioManager.Instance.SE_falldown);
                // 当前不能继续交互
                break;
        }
    }//按下E执行交互的位置


    private void StartRescueBattle()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning($"{name} 没有设置救援敌人Prefab。");

            // 没有敌人时，直接变成可救状态
            BecomeRescueable();
            return;
        }

        state = RescueState.Fighting;

        // 战斗中暂时取消交互提示
        gameObject.tag = "Untagged";

        Vector3 spawnPosition =
            enemySpawnPoint != null
                ? enemySpawnPoint.position
                : transform.position;

        GameObject enemyObject = Instantiate(
            enemyPrefab,
            spawnPosition,
            Quaternion.identity
        );

        spawnedEnemy = enemyObject.GetComponent<EnemyController>();

        // 让新生成的敌人立即朝向玩家
        float dir = GameManager.instance.player.transform.position.x > enemyObject.transform.position.x ? 1f : -1f;

        Vector3 scale = enemyObject.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        enemyObject.transform.localScale = scale;



        Invoke(nameof(SetEnemySkin), 0.2f);



        //播放没有敌人的一般拘束动画
        rbqController.BoundAnimation();


        StartCoroutine(WaitForEnemyDead());
    }//产生敌人


    void SetEnemySkin()
    {
        //将人质皮肤代入敌人
        spawnedEnemy.Man_clothesIndex = rbqController.Man_clothesIndex;
        spawnedEnemy.Man_hairIndex = rbqController.Man_hairIndex;
        spawnedEnemy.RefreshPlayerSkin();
    }

    private IEnumerator WaitForEnemyDead()
    {
        while (spawnedEnemy != null &&
               !spawnedEnemy.isDead)
        {
            yield return null;
        }

        BecomeRescueable();
    }

    private void BecomeRescueable()
    {
        state = RescueState.Bound;

        // 恢复E键交互
        gameObject.tag = interactableTag;
    }//变成能交互状态

    private void Rescue()
    {
        if (state != RescueState.Bound)
            return;

        state = RescueState.Rescued;

        // 防止连续触发
        gameObject.tag = "Untagged";

        GameManager.instance.CompleteRescue();

        // 目前先直接消失
        gameObject.SetActive(false);
    }//救出时触发




    [Header("自动发现玩家")]
    public float detectDistance = 5f;

    private bool battleTriggered;

    private void Update()
    {
        if (state != RescueState.BeingAbused)
            return;

        if (battleTriggered)
            return;

        if (GameManager.instance == null)
            return;

        PlayerController player = GameManager.instance.player;

        if (player == null || player.isDead)
            return;

        if (Vector2.Distance(
                transform.position,
                player.transform.position)
            <= detectDistance)
        {
            battleTriggered = true;
            StartRescueBattle();
        }
    }


















}