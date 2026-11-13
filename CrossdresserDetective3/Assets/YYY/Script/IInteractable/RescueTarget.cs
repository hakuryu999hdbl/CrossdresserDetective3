using System.Collections;
using UnityEngine;

public class RescueTarget : MonoBehaviour, IInteractable
{
    public enum RescueState
    {
        Bound,          //拘束循环状态
        BeingAbused,    //被敌人凌辱状态
        Fighting,       //拘束循环状态，但是敌人没被击败所以不能拯救
        Rescued         //拘束循环状态，敌人被击败可以解救
    }

    [Header("拘束循环状态")]
    public RescueState state = RescueState.Bound;
    public RBQController rbqController;


    [Header("敌人")]
    public GameObject enemyPrefab;//目前暂时全体Enemy_1
    public Transform enemySpawnPoint;

    private EnemyController spawnedEnemy;
    private string interactableTag;
    private bool registered;

    public bool IsDecorative => rbqController != null && rbqController.isDecorative;

    public GameObject NoTouch;//目前没有办法救的提示

    private void Awake()
    {
        interactableTag = gameObject.tag;
        if (rbqController == null)
            rbqController = GetComponentInParent<RBQController>();
    }

    private void Start()
    {
        if (IsDecorative)
        {
            // 保留场景中的演出，但不参与任务和交互。
            gameObject.tag = "Untagged";
            if (NoTouch != null) NoTouch.SetActive(false);
            return;
        }

        RegisterTarget();//向GameManager登记这是救出任务
        RefreshAnimation();//随机拘束还是凌辱，告知rbqController


        //如果是拘束凌辱状态初始不能被救
        if (state == RescueState.BeingAbused)
        {
            gameObject.tag = "Untagged";
        }

    }

    private void RegisterTarget()
    {
        if (IsDecorative || registered)
            return;

        registered = true;

        GameManager.instance.RegisterRescueTarget();
    }//向GameManager登记这是救出任务

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
                rbqController.BoundAnimation();//局内失踪少女初始单个捆绑状态
                break;

            case RescueState.BeingAbused:
                rbqController.AbuseAnimation();//局内失踪少女入口
                break;

            case RescueState.Fighting:
                rbqController.BoundAnimation();//局内失踪少女在敌人离开后进入单个捆绑状态
                break;
        }
    }



    public void TriggerAction()
    {
        if (IsDecorative) return;

        if (GameManager.instance.player.isBondage)
            return;


        switch (state)
        {
            case RescueState.Bound:
                Rescue();
                break;

            case RescueState.BeingAbused:
                // 在凌辱期间
                break;

            case RescueState.Fighting:
            case RescueState.Rescued:

                AudioManager.Instance.PlayFX(AudioManager.Instance.SE_falldown);
                // 不能被救声
                break;
        }
    }//每当E键按下的时候


    private void StartRescueBattle()
    {
        if (IsDecorative) return;

        if (enemyPrefab == null)
        {
            BecomeRescueable();
            return;
        }

        state = RescueState.Fighting;

        //变为不能解救状态
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

        // 敌人朝向玩家
        float dir = GameManager.instance.player.transform.position.x > enemyObject.transform.position.x ? 1f : -1f;

        Vector3 scale = enemyObject.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        enemyObject.transform.localScale = scale;



        Invoke(nameof(SetEnemySkin), 0.2f);



        NoTouch.SetActive(true);
        rbqController.BoundAnimation();    //敌人离开。失踪少女变成拘束


        StartCoroutine(WaitForEnemyDead());
    }//产生敌人


    void SetEnemySkin()
    {
        //将皮肤带入产生的敌人身上
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
        if (IsDecorative) return;

        state = RescueState.Bound;
        NoTouch.SetActive(false);

        //变成可交互
        gameObject.tag = interactableTag;
    }

    private void Rescue()
    {
        if (IsDecorative) return;

        if (state != RescueState.Bound)
            return;

        state = RescueState.Rescued;

        //不可交互
        gameObject.tag = "Untagged";

        GameManager.instance.CompleteRescue();

        rbqController.frameEvent_Audio._Girl_thankYou();

        rbqController.anim.Play("Man_AbuseGirl_8");

       Invoke(nameof(Dis), 3f);
      
    }//救出

    void Dis() 
    {
        // 未来更改别的状态
        gameObject.SetActive(false);
    }


    [Header("敌人触发范围")]
    float detectDistanceX = 4f;
    float maxYDifference = 2f;

    private bool battleTriggered;

    private void Update()
    {
        if (IsDecorative) return;

        if (state != RescueState.BeingAbused)
            return;

        if (battleTriggered)
            return;

        GameManager game = GameManager.instance;
        if (game == null || game.EnemiesSuspendedForCaptivity)
            return;

        PlayerController player = game.player;

        // 被抓或传送到拘束房期间，不消耗救出战斗的触发机会。
        if (player == null || player.isDead || player.isCaptured)
            return;

        float xDistance = Mathf.Abs(
      player.transform.position.x - transform.position.x
  );

        float yDistance = Mathf.Abs(
            player.transform.position.y - transform.position.y
        );

        if (xDistance <= detectDistanceX &&
            yDistance <= maxYDifference)
        {
            battleTriggered = true;
            StartRescueBattle();
        }
    }


















}
