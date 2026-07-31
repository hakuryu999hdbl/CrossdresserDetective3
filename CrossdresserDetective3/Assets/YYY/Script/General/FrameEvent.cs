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
          0, // 伪娘头发
          0, // 伪娘吊袜带
          2, // 伪娘衣服：0 = 不显示
          1, // 伪娘手套：0 = 不显示
          0, // 伪娘内裤：0 = 不显示
          1, // 伪娘鞋子
          0, // 伪娘裙子
          1, // 伪娘丝袜：0 = 不显示
          1, // 伪娘帽子：0 = 不显示
          1, // 伪娘面具：0 = 不显示

          0, // 御姐头发
          2, // 御姐衣服
          1, // 御姐手套：0 = 不显示
          0, // 御姐内衣：0 = 不显示
          1, // 御姐鞋子
          1, // 御姐丝袜：0 = 不显示
          0, // 御姐帽子
          0, // 御姐面具

          0, // 男人头发
          1, // 男人衣服


          1,  //刀剑
          1,  // 手枪
          1, // 步枪
          0, //投掷品

          0//拘束类型

          );

        }
    }
    public void RandomTestSkin()
    {
        ShowCurrentAll(

            Random.Range(0, 2), // 伪娘头发
            Random.Range(0, 2), // 伪娘吊袜带
            Random.Range(0, 2), // 伪娘衣服
            Random.Range(0, 2), // 伪娘手套
            Random.Range(0, 2), // 伪娘内裤
            Random.Range(0, 2), // 伪娘鞋子
            Random.Range(0, 2), // 伪娘裙子
            Random.Range(0, 3), // 伪娘丝袜
            Random.Range(0, 2), // 伪娘帽子
            Random.Range(0, 2), // 伪娘面具


            Random.Range(0, 3), // 御姐头发
            Random.Range(0, 2), // 御姐衣服
            Random.Range(0, 2), // 御姐手套
            Random.Range(0, 2), // 御姐内衣
            Random.Range(0, 2), // 御姐鞋子
            Random.Range(0, 3), // 御姐丝袜
            Random.Range(0, 2), // 御姐帽子
            Random.Range(0, 2), // 御姐面具

            Random.Range(0, 3), // 男人头发
            Random.Range(0, 3), // 男人衣服


            Random.Range(0, 2), // 刀剑
            Random.Range(0, 2), // 手枪
            Random.Range(0, 2), // 步枪
            Random.Range(0, 2), // 投掷品

            Random.Range(0, 2)  // 拘束
        );//随机皮肤
    }

    public void ShowCurrentAll
        (
           int _YYY_beltIndex, int _YYY_hairIndex, int _YYY_clothesIndex, int _YYY_glovesIndex, int _YYY_pantiesIndex, int _YYY_shoesIndex, int _YYY_skirtIndex, int _YYY_stockingsIndex, int _YYY_hatIndex, int _YYY_maskIndex,
           int _Girl_hairIndex, int _Girl_clothesIndex, int _Girl_glovesIndex, int _Girl_underwearIndex, int _Girl_shoesIndex, int _Girl_stockingsIndex, int _Girl_hatIndex, int _Girl_maskIndex,
            int _Man_hairIndex, int _Man_clothesIndex,
           int _Weapon_MeleeIndex, int _Weapon_PistolIndex, int _Weapon_RifleIndex, int _Weapon_ThrowableIndex,
            int _Weapon_BondageIndex
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

        AddSkinSafe(newSkin, $"YYY/Hair/YYY_Hair_color{_YYY_hairIndex}");

        AddSkinSafe(newSkin, $"YYY/Clothes/YYY_Clothes_color{_YYY_clothesIndex}");

        AddSkinSafe(newSkin, $"YYY/Gloves/YYY_Gloves_color{_YYY_glovesIndex}");
        AddSkinSafe(newSkin, $"YYY/Panties/YYY_Panties_color{_YYY_pantiesIndex}");

        if (_YYY_shoesIndex != 0)
            AddSkinSafe(newSkin, $"YYY/Shoes/YYY_Shoes_color{_YYY_shoesIndex}");

        if (_YYY_skirtIndex != 0)
            AddSkinSafe(newSkin, $"YYY/Skirt/YYY_Skirt_color{_YYY_skirtIndex}");

        AddSkinSafe(newSkin, $"YYY/Stockings/YYY_Stockings_color{_YYY_stockingsIndex}");

        if (_YYY_hatIndex != 0)
            AddSkinSafe(newSkin, $"YYY/Hat/YYY_Hat_color{_YYY_hatIndex}");

        if (_YYY_maskIndex != 0)
            AddSkinSafe(newSkin, $"YYY/Mask/YYY_Mask_color{_YYY_maskIndex}");


        AddSkinSafe(newSkin, $"Girl/Hair/Girl_Hair_color{_Girl_hairIndex}");

        if (_Girl_clothesIndex != 0)
            AddSkinSafe(newSkin, $"Girl/Clothes/Girl_Clothes_color{_Girl_clothesIndex}");

        AddSkinSafe(newSkin, $"Girl/Gloves/Girl_Gloves_color{_Girl_glovesIndex}");

        AddSkinSafe(newSkin, $"Girl/Underwear/Girl_Underwear_color{_Girl_underwearIndex}");

        if (_Girl_shoesIndex != 0)
            AddSkinSafe(newSkin, $"Girl/Shoes/Girl_Shoes_color{_Girl_shoesIndex}");

        AddSkinSafe(newSkin, $"Girl/Stockings/Girl_Stockings_color{_Girl_stockingsIndex}");

        if (_Girl_hatIndex != 0)
            AddSkinSafe(newSkin, $"Girl/Hat/Girl_Hat_color{_Girl_hatIndex}");

        if (_Girl_maskIndex != 0)
            AddSkinSafe(newSkin, $"Girl/Mask/Girl_Mask_color{_Girl_maskIndex}");


        if (_Weapon_MeleeIndex != 0)
            AddSkinSafe(newSkin, $"Weapon/Melee/Weapon_Melee_color{_Weapon_MeleeIndex}");

        if (_Weapon_PistolIndex != 0)
            AddSkinSafe(newSkin, $"Weapon/Pistol/Weapon_Pistol_color{_Weapon_PistolIndex}");

        if (_Weapon_RifleIndex != 0)
            AddSkinSafe(newSkin, $"Weapon/Rifle/Weapon_Rifle_color{_Weapon_RifleIndex}");

        if (_Weapon_ThrowableIndex != 0)
            AddSkinSafe(newSkin, $"Weapon/Throwable/Weapon_Throwable_color{_Weapon_ThrowableIndex}");

        AddSkinSafe(newSkin, $"Weapon/Bondage/Weapon_Bondage_color{_Weapon_BondageIndex}");

        AddSkinSafe(newSkin, $"Man/Hair/Man_Hair_color{_Man_hairIndex}");

        AddSkinSafe(newSkin, $"Man/Clothes/Man_Clothes_color{_Man_clothesIndex}");





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
    public void HalfShowSkeleton()
    {
        skeletonAnimation.Skeleton.A = 0.3f; // 半透明
    }

    #endregion


    #region  闪红

    Coroutine flashCoroutine;
    public void FlashRed(float time = 0.28f)
    {

        //Debug.Log("受伤特效");

        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);

        flashCoroutine = StartCoroutine(FlashColor(
            new Color(1f, 0.25f, 0.25f, 1f),
            time));
    }

    IEnumerator FlashColor(Color color, float time)
    {
        skeletonAnimation.Skeleton.SetColor(color);

        yield return new WaitForSeconds(time);

        skeletonAnimation.Skeleton.SetColor(Color.white);

        flashCoroutine = null;
    }
    #endregion

 

    #endregion













    [Header("帧事件触发")]

    public EnemyController enemyController;

    public PlayerController playerController;

    public RescueTarget rescueTarget;

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



    #region  敌人瞄准投掷
    public void AimThrowHeldObject()
    {


        if (enemyController != null)
        {
            enemyController.AimThrowSpawnExplosion();

        }


    }

    //敌人瞄准投掷技能结束
    public void OnThrowOver()
    {
        if (enemyController != null)
        {
            enemyController.AimThrowStartLaugh();

        }
    }

    //敌人嘲笑结束
    public void OnLaughOver()
    {
        if (enemyController != null)
        {
            enemyController.AimThrowOver();

        }
    }
    #endregion

    #region  敌人将玩家投出去
    public void OnCatchPlayer()
    {
        if (enemyController != null)
        {
            enemyController.Catch_Collider.SetActive(true);
        }
        Invoke("HideCatch", 0.2f);
    }
    void HideCatch()
    {
        if (enemyController != null)
        {
            enemyController.Catch_Collider.SetActive(false);
        }
    }//抓取碰撞体消失

    public void OnThrowCapturedPlayer()
    {
        if (enemyController != null)
        {
            enemyController.ThrowCapturedPlayer();

        }
    }
    #endregion

    #region  敌人防御反击
  
    public void StartBlockCounter()
    {
        if (enemyController is Enemy_4 biker)
        {
            biker.StartCounterAttack();
        }
    }  //敌人防御反击

    
    public void EndBlockCounter()
    {
        if (enemyController is Enemy_4 biker)
        {
            biker.EndCounterAttack();
        }
    }//敌人防御反击后
    #endregion

    #region 敌人下落攻击

    //敌人跳起消失,产生瞄准
    public void StartJumpStrikeAim()
    {
        if (enemyController is Enemy_5 enemy5)
        {
            enemy5.jumpStrikeState.StartAim(enemy5);
        }

    }

    //敌人下落结束攻击
    public void StartJumpStrikeAttack()
    {
        if (enemyController is Enemy_5 enemy5)
        {
            enemy5.jumpStrikeState.StartAttack(enemy5);
        }
    }

    //特殊大范围攻击
    public void JumpStrikeHit()
    {
        if (enemyController is Enemy_5 enemy5)
        {
            if (enemy5.jumpStrikeCollider != null)
            {
                enemy5.jumpStrikeCollider.SetActive(true);

                CancelInvoke(nameof(HideJumpStrikeHit));
                Invoke(nameof(HideJumpStrikeHit), 0.2f);
            }
        }
    }

    private void HideJumpStrikeHit()
    {
        if (enemyController is Enemy_5 enemy5 &&
            enemy5.jumpStrikeCollider != null)
        {
            enemy5.jumpStrikeCollider.SetActive(false);
        }
    }


    //攻击结束
    public void EndJumpStrike()
    {
        if (enemyController is Enemy_5 enemy5)
        {
            enemy5.jumpStrikeState.EndSkill(enemy5);
        }
    }

    #endregion


    #region 双方攻击动画触发
    [Header("攻击动画触发")]
    public GameObject AttackArea_1;
    public GameObject AttackArea_2;
    public GameObject AttackArea_3;
    public void Attack_1()
    {
        StartCoroutine(AttackRoutine(AttackArea_1));
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

    #region 双方射击投掷帧事件触发
  
  

    public void LandFX()
    {
        //跳落第一帧触发
        playerController.LandFX();
    }

    public void Throw()
    {

        if (playerController != null)
        {
            playerController.ThrowWeapon();
        }

   

        AimThrowHeldObject();//和玩家共用Throw


    }

    public void Shoot()
    {
        if (enemyController != null)
        {
            enemyController.Shoot();

            if (enemyController.attackType == -2)
            {
                Invoke(nameof(RifleBullet), 0.2f);
            }
        }
        if (playerController != null)
        {
            playerController.Shoot();

            if (playerController.attackType == -2)
            {
                Invoke(nameof(RifleBullet), 0.2f);
            }
        }

    }

    void RifleBullet()
    {
        if (enemyController != null)
        {
            enemyController.Shoot();


        }
        if (playerController != null)
        {
            playerController.Shoot();


        }
    }





    public void Reload()
    {
        playerController.OnReloadAnimationEnd();
    }

    #endregion



    #region 剧情CG相关帧事件
    public void Undressing_Up()
    {
        Debug.Log("脱掉上半身");

        playerController.clothesIndex = 0;
        playerController.glovesIndex = 0;
        playerController.hatIndex = 0;
        playerController.maskIndex = 0;

        playerController.RefreshPlayerSkin();
    }
    public void Undressing_Down() 
    {
        playerController.beltIndex = 0;
        playerController.stockingsIndex = 0;
        playerController.pantiesIndex = 0;
        playerController.shoesIndex = 0;
        playerController.skirtIndex = 0;

        playerController.RefreshPlayerSkin();

       
    }

  

    public void SetPlayer_Clothes_01()
    {
        playerController.beltIndex = 0;
        playerController.clothesIndex = 10;
        playerController.glovesIndex = 0;
        playerController.pantiesIndex = 0;
        playerController.shoesIndex = 10;
        playerController.skirtIndex = 0;
        playerController.stockingsIndex = 0;
        playerController.bondageType = 0;
        playerController.RefreshPlayerSkin();

    }//赤裸绳子捆绑状态

    public void SetPlayer_Clothes_02()
    {
        playerController.beltIndex = 0;
        playerController.clothesIndex = 0;
        playerController.glovesIndex = 0;
        playerController.pantiesIndex = 0;
        playerController.shoesIndex = 0;
        playerController.skirtIndex = 0;
        playerController.stockingsIndex = 0;

        playerController.RefreshPlayerSkin();

    }//赤裸状态

    public void SetPlayer_Clothes_03()
    {
        //Debug.Log("第一章衣物");

        playerController.beltIndex = 1;
        playerController.stockingsIndex = 1;
        playerController.pantiesIndex = 4;
        playerController.shoesIndex = 1;
        playerController.skirtIndex = 0;

        playerController.clothesIndex = 1;
        playerController.glovesIndex = 1;
        playerController.hatIndex = 0;
        playerController.maskIndex = 0;

        playerController.RefreshPlayerSkin();
    }//第一章事务所剧情(白色内裤无裙子)


    public void Story_Clothes_Man_01()
    {
        //赤裸的叶语嫣被捆绑
        enemyController.clothesIndex = 10;
        enemyController.glovesIndex = 0;
        enemyController.hatIndex = 0;
        enemyController.maskIndex = 0;
        enemyController.beltIndex = 0;
        enemyController.stockingsIndex = 0;
        enemyController.pantiesIndex = 0;
        enemyController.shoesIndex = 10;
        enemyController.skirtIndex = 0;


        //男性小偷
        enemyController.Man_clothesIndex = 1;
        enemyController.Man_hairIndex = 2;


        enemyController.RefreshPlayerSkin();


    }

    #endregion

    public void SetPlayer_Bondage_1()
    {

        enemyController.clothesIndex = 10;
        enemyController.shoesIndex = 10;

        enemyController.RefreshPlayerSkin();

    }//战败尸体调用



    public void SetRBQ_Bondage_1() 
    {

        //赤裸的叶语嫣被捆绑
        rescueTarget.clothesIndex = 10;
        rescueTarget.glovesIndex = 0;
        rescueTarget.hatIndex = 0;
        rescueTarget.maskIndex = 0;
        rescueTarget.beltIndex = 0;
        rescueTarget.stockingsIndex = 0;
        rescueTarget.pantiesIndex = 0;
        rescueTarget.shoesIndex = 10;
        rescueTarget.skirtIndex = 0;


        //男性小偷
        rescueTarget.Man_clothesIndex = 1;
        rescueTarget.Man_hairIndex = 2;


        rescueTarget.RefreshPlayerSkin();
    }

}
