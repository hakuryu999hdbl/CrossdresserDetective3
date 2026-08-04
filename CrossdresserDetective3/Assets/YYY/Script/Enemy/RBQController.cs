using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RescueTarget;

public class RBQController : MonoBehaviour
{
    private void Awake()
    {
        currentAnimationType = Random.Range(0, 2);//玩家随机动画
    }//这个好像动画很早就被触发应，所以随机需要更早


    private void Start()
    {

        RandomSkin();

        frameEvent.SetRBQ_Bondage_1();


        RandomizeZ();


       
    }






    /// <summary>
    /// 动画循环
    /// </summary>
    #region



    [Header("当前动画系")]
    [SerializeField]
    private int currentAnimationType =1;//0 Rape系   1 Abuse系







    private Coroutine abuseCoroutine;


    [Header("Spine动画器控制")]
    public Animator anim;


    public void BoundAnimation()
    {
        StopAbuseAnimation();


        switch (currentAnimationType) 
        {
            case 0:
                anim.Play("Man_RapeYYY_7", 0, 0f);
                frameEvent.SetDeadBondage_Bondage_1();//全身捆绑
                break;

            case 1:
                anim.Play("Man_AbuseYYY_8", 0, 0f);
                frameEvent.SetDeadBondage_Bondage_1();//全身捆绑
                break;
        }
       
        anim.Update(0f);


    }//拘束循环


    public void AbuseAnimation()
    {
        StopAbuseAnimation();

        if (abuseCoroutine != null)
        {
            StopCoroutine(abuseCoroutine);
        }



        switch (currentAnimationType)
        {
            case 0:
                anim.Play("Man_RapeYYY_1", 0, 0f);
                frameEvent.SetDeadBondage_Bondage_1();//全身捆绑
                break;

            case 1:
                anim.Play("Man_AbuseYYY_1", 0, 0f);
                frameEvent.SetDeadBondage_Bondage_2();//全身捆绑，不绑腿
                break;
        }

        anim.Update(0f);

        abuseCoroutine = StartCoroutine(AbuseLoop());

    }//带敌人强奸循环





    private IEnumerator AbuseLoop()
    {


        while (true)
        {
            // 1 / 2
            yield return new WaitForSeconds(15f);

            anim.SetTrigger("Next");


            yield return new WaitUntil(
                () => !anim.IsInTransition(0)
            );

            // 3 / 4
            yield return new WaitForSeconds(10f);

            anim.SetTrigger("Next");

            yield return new WaitUntil(
                () => !anim.IsInTransition(0)
            );

            // 5 / 6 
            yield return new WaitForSeconds(10f);

            anim.SetTrigger("Next");

            yield return new WaitUntil(
                () => !anim.IsInTransition(0)
            );

        }
    }
    public void StopAbuseAnimation()
    {
        if (abuseCoroutine != null)
        {
            StopCoroutine(abuseCoroutine);
            abuseCoroutine = null;
        }

        anim.ResetTrigger("Next");
    }


    public void ReadCurrentGame(PlayerController player)
    {


        clothesIndex = player.clothesIndex;
        glovesIndex = player.glovesIndex;
        pantiesIndex = player.pantiesIndex;
        shoesIndex = player.shoesIndex;
        skirtIndex = player.skirtIndex;
        stockingsIndex = player.stockingsIndex;

        RefreshPlayerSkin();

    }//尸体读取玩家




    #endregion


















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

    public void RandomizeZ()
    {
        anim.transform.position = new Vector3(
         anim.transform.position.x,
         anim.transform.position.y,
        Random.Range(-0.2f, -0.3f)
    );
    }

}
