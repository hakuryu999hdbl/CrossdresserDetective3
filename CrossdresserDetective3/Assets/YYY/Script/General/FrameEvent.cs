using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using UnityEngine.InputSystem;

public class FrameEvent : MonoBehaviour
{


    /// <summary>
    /// 皮肤
    /// </summary>
    #region
    [Header("皮肤")]
    SkeletonMecanim skeletonAnimation;
    Skin blendSkin = new Skin("BlendedSkin");// 创建一个新的混合皮肤


    // Start is called before the first frame update
    void Awake()
    {
        //换皮肤
        skeletonAnimation = GetComponent<SkeletonMecanim>();

       


    }
  

    void Update()
    {
        // 按 T 随机换装测试
        if (Keyboard.current.tKey.wasPressedThisFrame)
        {
            //RandomTestSkin();


              ShowCurrentAll(
            0, // 吊袜带
            1, // 衣服
            1, // 手套：0 = 不显示
            0, // 内裤：0 = 不显示
            1, // 鞋子
            0, // 裙子
            1, // 丝袜：0 = 不显示
            1,  //刀剑
            1,  // 手枪
            1
            );

        }
    }
    public void RandomTestSkin()
    {
        ShowCurrentAll(

            Random.Range(0, 2), // 吊袜带
            Random.Range(0, 2), // 衣服
            Random.Range(0, 2), // 手套
            Random.Range(0, 2), // 内裤
            Random.Range(0, 2), // 鞋子
            Random.Range(0, 2), // 裙子
            Random.Range(0, 3), // 丝袜
            Random.Range(0, 2), // 刀剑
            Random.Range(0, 2), // 手枪
            Random.Range(0, 2)  // 步枪

        );//随机皮肤
    }

    public void ShowCurrentAll
        (
           int _YYY_beltIndex, int _YYY_clothesIndex, int _YYY_glovesIndex, int _YYY_pantiesIndex, int _YYY_shoesIndex, int _YYY_skirtIndex, int _YYY_stockingsIndex,
           int _Weapon_MeleeIndex, int _Weapon_PistolIndex, int _Weapon_RifleIndex
        )
    {

        //// 每次重新创建混合皮肤，不然旧皮肤会残留
        //Skin newSkin = new Skin("BlendedSkin");
        //
        //
        //if (_YYY_beltIndex != 0)
        //    blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin($"YYY/Belt/YYY_Belt_color{_YYY_beltIndex}"));
        //
        //if (_YYY_clothesIndex != 0)
        //    blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin($"YYY/Clothes/YYY_Clothes_color{_YYY_clothesIndex}"));
        //
        //// 手套：0 也要加，因为是裸体/皮肤状态
        //blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin($"YYY/Gloves/YYY_Gloves_color{_YYY_glovesIndex}"));
        //
        //// 内裤：0 也要加
        //blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin($"YYY/Panties/YYY_Panties_color{_YYY_pantiesIndex}"));
        //
        //if (_YYY_shoesIndex != 0)
        //    blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin($"YYY/Shoes/YYY_Shoes_color{_YYY_shoesIndex}"));
        //
        //if (_YYY_skirtIndex != 0)
        //    blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin($"YYY/Skirt/YYY_Skirt_color{_YYY_skirtIndex}"));
        //
        //// 丝袜：0 也要加
        //blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin($"YYY/Stockings/YYY_Stockings_color{_YYY_stockingsIndex}"));
        //
        //if (_Weapon_MeleeIndex != 0)
        //    blendSkin.AddSkin(skeletonAnimation.Skeleton.Data.FindSkin($"Weapon/Melee/Weapon_Melee_color{_Weapon_MeleeIndex}"));
        //
        //
        //skeletonAnimation.Skeleton.SetSkin(blendSkin);
        //skeletonAnimation.Skeleton.SetSlotsToSetupPose();
        //
        //blendSkin = newSkin;


        Skin newSkin = new Skin("BlendedSkin");

        if (_YYY_beltIndex != 0)
            AddSkinSafe(newSkin, $"YYY/Belt/YYY_Belt_color{_YYY_beltIndex}");

        if (_YYY_clothesIndex != 0)
            AddSkinSafe(newSkin, $"YYY/Clothes/YYY_Clothes_color{_YYY_clothesIndex}");

        AddSkinSafe(newSkin, $"YYY/Gloves/YYY_Gloves_color{_YYY_glovesIndex}");
        AddSkinSafe(newSkin, $"YYY/Panties/YYY_Panties_color{_YYY_pantiesIndex}");

        if (_YYY_shoesIndex != 0)
            AddSkinSafe(newSkin, $"YYY/Shoes/YYY_Shoes_color{_YYY_shoesIndex}");

        if (_YYY_skirtIndex != 0)
            AddSkinSafe(newSkin, $"YYY/Skirt/YYY_Skirt_color{_YYY_skirtIndex}");

        AddSkinSafe(newSkin, $"YYY/Stockings/YYY_Stockings_color{_YYY_stockingsIndex}");

        if (_Weapon_MeleeIndex != 0)
            AddSkinSafe(newSkin, $"Weapon/Melee/Weapon_Melee_color{_Weapon_MeleeIndex}");

        if (_Weapon_PistolIndex != 0)
            AddSkinSafe(newSkin, $"Weapon/Pistol/Weapon_Pistol_color{_Weapon_PistolIndex}");

        if (_Weapon_RifleIndex != 0)
            AddSkinSafe(newSkin, $"Weapon/Rifle/Weapon_Rifle_color{_Weapon_RifleIndex}");

        skeletonAnimation.Skeleton.SetSkin(newSkin);
        skeletonAnimation.Skeleton.SetSlotsToSetupPose();

        blendSkin = newSkin;
    }

    private void AddSkinSafe(Skin targetSkin, string skinName)
    {
        Skin skin = skeletonAnimation.Skeleton.Data.FindSkin(skinName);

        if (skin == null)
        {
            Debug.LogWarning("找不到 Skin: " + skinName);
            return;
        }

        targetSkin.AddSkin(skin);
    }

    #region  渐变进入 渐变消失

    public void FadeIn(float duration = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeInCoroutine(duration));
    }

    private IEnumerator FadeInCoroutine(float duration)
    {
        float timer = 0f;

        skeletonAnimation.Skeleton.A = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(
                0f,
                1f,
                timer / duration
            );

            skeletonAnimation.Skeleton.A = alpha;

            yield return null;
        }

        skeletonAnimation.Skeleton.A = 1f;
    }

    public void FadeOut(float duration = 0.5f)
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutCoroutine(duration));
    }

    private IEnumerator FadeOutCoroutine(float duration)
    {
        float timer = 0f;

        skeletonAnimation.Skeleton.A = 1f;

        while (timer < duration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(
                1f,
                0f,
                timer / duration
            );

            skeletonAnimation.Skeleton.A = alpha;

            yield return null;
        }

        skeletonAnimation.Skeleton.A = 0f;
    }
    public void HideSkeleton()
    {
        skeletonAnimation.Skeleton.A = 0f; // 完全透明
    }

    public void ShowSkeleton()
    {
        skeletonAnimation.Skeleton.A = 1f; // 完全不透明
    }
    //public void HalfShowSkeleton()
    //{
    //    skeletonAnimation.Skeleton.A = 0.3f; // 半透明
    //}

    #endregion

 
    public void SetBlack()
    {
        skeletonAnimation.Skeleton.SetColor(new Color(0f, 0f, 0f, 1f));

    }//变黑

    public void SetRed()
    {
       
        skeletonAnimation.Skeleton.SetColor(new Color(0.3f, 0f, 0f, 1f));

    }//变红

    public void ResetColor()
    {
        skeletonAnimation.Skeleton.SetColor(Color.white);

    }//恢复原来颜色

    #endregion















    public EnemyController enemyController;


    #region 吹灭炸弹
    public void SetOff()
    {

        if (enemyController == null)
            return;

        if (enemyController.targetPoint == null)
            return;

        Bomb bomb = enemyController.targetPoint.GetComponent<Bomb>();

        if (bomb == null)
            return;

        bomb.TurnOff();

        enemyController.attackList.Remove(enemyController.targetPoint);
        enemyController.targetPoint = null;


        //if (enemyController.targetPoint.GetComponent<Bomb>() != null)
        //{
        //    enemyController.targetPoint.GetComponent<Bomb>().TurnOff();
        //}


    }
    #endregion


    #region 丢掉炸弹
    [Header("丢掉炸弹")]
    public Transform pickupPoint;

    public void PickUpBomb()
    {
        if (enemyController == null) return;
        if (enemyController.targetPoint == null) return;
        if (enemyController.hasBomb) return;

        if (enemyController.targetPoint.CompareTag("Bomb"))
        {
            Transform bomb = enemyController.targetPoint;

            bomb.position = pickupPoint.position;
            bomb.SetParent(pickupPoint);

            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = Vector2.zero;
            }

            enemyController.heldBomb = bomb;
            enemyController.hasBomb = true;
        }




        // 🔴 第一层挡板：targetPoint 已经被销毁
       //if (enemyController.targetPoint == null) return;
       //
       //// 🔴 第二层挡板：已经拿了炸弹
       //if (enemyController.hasBomb) return;
       //
       //if (enemyController.targetPoint.CompareTag("Bomb")&&!enemyController.hasBomb)
       //{
       //    //Debug.Log("捡起炸弹");
       //
       //
       //    enemyController.targetPoint.gameObject.transform.position = pickupPoint.position;
       //    enemyController.targetPoint.SetParent(pickupPoint);
       //    enemyController.targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
       //    enemyController.hasBomb = true;
       //
       //
       //}//如果是炸弹，就捡起来移到对应位置（成为子集）,同时刚体为（移动平台）
    }

    public float power;
    public void ThrowAway() 
    {

        if (enemyController == null) return;
        if (!enemyController.hasBomb) return;
        if (enemyController.heldBomb == null)
        {
            enemyController.hasBomb = false;
            return;
        }

        Transform bomb = enemyController.heldBomb;
        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();

        if (rb == null)
        {
            enemyController.hasBomb = false;
            enemyController.heldBomb = null;
            return;
        }

        rb.bodyType = RigidbodyType2D.Dynamic;
        bomb.SetParent(transform.parent.parent);

        PlayerController player = FindFirstObjectByType<PlayerController>();

        float dir = 1f;
        if (player != null)
        {
            dir = player.transform.position.x - transform.position.x < 0 ? -1f : 1f;
        }

        rb.velocity = Vector2.zero;
        rb.AddForce(new Vector2(dir, 1f) * power, ForceMode2D.Impulse);

        enemyController.hasBomb = false;
        enemyController.heldBomb = null;


        //if (enemyController.hasBomb) 
        //{
        //    enemyController.targetPoint.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        //    enemyController.targetPoint.SetParent(transform.parent.parent);
        //
        //    if (FindFirstObjectByType<PlayerController>().gameObject.transform.position.x-transform.position.x<0) 
        //    {
        //        enemyController.targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(-1, 1) * power, ForceMode2D.Impulse);
        //    }
        //    else
        //    {
        //        enemyController.targetPoint.GetComponent<Rigidbody2D>().AddForce(new Vector2(1, 1) * power, ForceMode2D.Impulse);
        //    }
        //    enemyController.hasBomb = false;
        //
        //}//设置为父级的父级（最外面），并刚体重新物理化，丢到玩家方向

    }

    #endregion


    #region 吞下炸弹
    [Header("吞下炸弹")]
    public float scale;
    public void Swalow() 
    {

        /////////挡板
        if (enemyController == null)
            return;

        if (enemyController.targetPoint == null)
            return;

        Bomb bomb = enemyController.targetPoint.GetComponent<Bomb>();
        if (bomb == null)
            return;
        /////////挡板



        enemyController.targetPoint.GetComponent<Bomb>().TurnOff();
        enemyController.targetPoint.gameObject.SetActive(false);


        /////////吞下炸弹目标清空
        enemyController.targetPoint = null;
        enemyController.attackList.Clear();
        /////////





        enemyController.transform.localScale *= scale;//吞下炸弹变大
    }
    #endregion


    #region 野猪死后消灭本体
    [Header("死后消灭本体")]
    public GameObject TargetDestory;
    public void DestroyAfterAnimation()
    {
        Destroy(TargetDestory);
    }
    #endregion


    #region 攻击动画触发
    [Header("攻击动画触发")]
    public GameObject AttackArea_1;
    public GameObject AttackArea_2;
    public GameObject AttackArea_3;
    public void Attack_1()
    {
        StartCoroutine(AttackRoutine(AttackArea_1));
        //Debug.Log("产生攻击碰撞体");
    }//普攻
    public void Attack_2()
    {
        StartCoroutine(AttackRoutine(AttackArea_2));
    }//击退
    public void Attack_3()
    {
        StartCoroutine(AttackRoutine(AttackArea_3));
       

    }//击飞

    public void Dash() 
    {
        playerController.Dash();//这个是冲刺一段
    }


    IEnumerator AttackRoutine(GameObject area)
    {
        if (area == null) yield break;

        area.SetActive(true);

        yield return new WaitForSeconds(0.2f);

        area.SetActive(false);
    }




    #endregion

    #region 玩家特殊帧事件触发
    [Header("玩家特殊帧事件触发")]
    public PlayerController playerController;

    public void LandFX()
    {
        //跳落第一帧触发
        playerController.LandFX();
    }

    public void Throw()
    {

        playerController.ThrowWeapon();
    }

    public void Shoot()
    {

        playerController.Shoot();
    }

    public void Reload() 
    {
        playerController.OnReloadAnimationEnd();
    }

    #endregion






}
