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

    [Header("Spine动画控制")]
    public Animator anim;

    [Tooltip("直接可救时播放的拘束动画")]
    public string boundAnimation;

    [Tooltip("正在受辱时播放的循环动画")]
    public string abuseAnimation;

    [Tooltip("敌人出现后，切换成的等待救援动画")]
    public string waitingRescueAnimation;

    [Header("守卫敌人")]
    public GameObject enemyPrefab;
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
        RegisterTarget();
        RefreshAnimation();

        RandomSkin();
        frameEvent.SetRBQ_Bondage_1();


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
    }

    private void RefreshAnimation()
    {
        if (anim == null)
            return;

        switch (state)
        {
            case RescueState.Bound:
                PlayAnimation(boundAnimation);
                break;

            case RescueState.BeingAbused:
                PlayAnimation(abuseAnimation);
                break;

            case RescueState.Fighting:
                PlayAnimation(waitingRescueAnimation);
                break;
        }
    }

    private void PlayAnimation(string animationName)
    {
        if (string.IsNullOrEmpty(animationName))
            return;

        anim.Play(animationName, 0, 0f);
        anim.Update(0f);
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
                // 当前不能继续交互
                break;
        }
    }//执行交互的位置


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
        float dir = GameManager.instance.player.transform.position.x > enemyObject.transform.position.x ? 1f: -1f;

        Vector3 scale = enemyObject.transform.localScale;
        scale.x = Mathf.Abs(scale.x) * dir;
        enemyObject.transform.localScale = scale;



        Invoke(nameof(SetEnemySkin), 0.2f);



        //播放没有敌人的一般拘束动画
        PlayAnimation(waitingRescueAnimation);


        StartCoroutine(WaitForEnemyDead());
    }//产生敌人


    void SetEnemySkin() 
    {
        //将人质皮肤代入敌人
        spawnedEnemy.Man_clothesIndex = Man_clothesIndex;
        spawnedEnemy.Man_hairIndex = Man_hairIndex;
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




    /// <summary>
    /// Spine外观
    /// </summary>
    #region
    [Header("Spine外观")]
    public FrameEvent frameEvent;
    public int beltIndex;
    public int hairIndex;
    public int clothesIndex;
    public int glovesIndex;
    public int pantiesIndex;
    public int shoesIndex;
    public int skirtIndex;
    public int stockingsIndex;
    public int hatIndex;
    public int maskIndex;


    public int Girl_hairIndex;
    public int Girl_clothesIndex;
    public int Girl_glovesIndex;
    public int Girl_underwearIndex;
    public int Girl_shoesIndex;
    public int Girl_stockingsIndex;
    public int Girl_hatIndex;
    public int Girl_maskIndex;

    public int Man_hairIndex;
    public int Man_clothesIndex;

    [Header("武器与攻击方式")]
    public int meleeType;//0空手 1匕首 2武士刀 3尼泊尔军刀
    public int pistolType;//0空手 1柯尔特M1911 2沙鹰手枪 3格洛克手枪
    public int rifleType;//0空手 1步枪M4A1 2步枪AK47
    public int throwType;//0空手 1手榴弹 2烟雾弹 3闪光弹 4燃烧弹  5震撼弹  6飞刀
    public int attackType;//-2步枪射击  -1手枪射击 0踢击 1挥砍

    public int bondageType;//0绳子捆绑 1锁链捆绑


    public void RandomSkin()
    {
        Girl_hairIndex = Random.Range(0, 3);

        Girl_clothesIndex = Random.Range(0, 3);
        Girl_glovesIndex = Random.Range(0, 2);

        Girl_shoesIndex = Random.Range(0, 3);


        switch (Random.Range(0, 3))
        {
            case 0:
                Girl_underwearIndex = 0;
                Girl_stockingsIndex = 0;
                break;
            case 1:
                Girl_underwearIndex = 1;
                Girl_stockingsIndex = 0;
                break;
            case 2:
                Girl_underwearIndex = 2;
                Girl_stockingsIndex = 2;
                break;
        }

        Girl_hatIndex = Random.Range(0, 2);
        Girl_maskIndex = 1;


        Man_hairIndex = Random.Range(0, 3);
        Man_clothesIndex = Random.Range(1, 3);


        meleeType = Random.Range(1, 4);


        pistolType = Random.Range(1, 4);
        rifleType = Random.Range(1, 3);


    }




    public void RefreshPlayerSkin()
    {
        if (frameEvent == null) return;

        frameEvent.ShowCurrentAll(
            beltIndex,
            hairIndex,
            clothesIndex,
            glovesIndex,
            pantiesIndex,
            shoesIndex,
            skirtIndex,
            stockingsIndex,
            hatIndex,
            maskIndex,

            Girl_hairIndex,
            Girl_clothesIndex,
            Girl_glovesIndex,
            Girl_underwearIndex,
            Girl_shoesIndex,
            Girl_stockingsIndex,
            Girl_hatIndex,
            Girl_maskIndex,

            Man_hairIndex,
            Man_clothesIndex,

            meleeType,
            pistolType,
            rifleType,
            throwType,

            bondageType
       );

    }//更新外观


    #endregion













}