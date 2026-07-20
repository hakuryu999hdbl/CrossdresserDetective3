using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckArea : MonoBehaviour
{
    [Header("传递消息")]
    public EnemyController enemy;

    [Header("视野颜色")]
    public Color patrolColor = Color.green;
    public Color searchColor = Color.yellow;
    public Color attackColor = Color.red;
    public Color hitColor = Color.white;
    public Color chargeSkillColor = Color.red;
    public Color aimThrowSkillColor = Color.red;
    public Color blockColor = Color.red;
    public float colorChangeSpeed = 6f;

    private MeshRenderer meshRenderer;
    private Material fovMaterial;
    private Color targetColor;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshRenderer != null)
        {
            fovMaterial = meshRenderer.material;
            targetColor = patrolColor;
            fovMaterial.color = Color.green;
        }
    }

    private void Update()
    {
        if (fovMaterial != null)
        {
            fovMaterial.color = Color.Lerp(
                fovMaterial.color,
                targetColor,
                Time.deltaTime * colorChangeSpeed
            );
        }
    }



    private void OnTriggerStay2D(Collider2D other)
    {
        //if (!IsValidTarget(other)) return;

        enemy.OnCheckAreaStay(other);
        //targetColor = alertColor;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        //if (!IsValidTarget(other)) return;

        enemy.OnCheckAreaExit(other);

        //if (enemy.attackList.Count <= 0)
        //{
        //    targetColor = normalColor;
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //if (!IsValidTarget(other)) return;

        
    }

    private bool IsValidTarget(Collider2D other)
    {
        if (enemy == null) return false;
        if (enemy.isDead) return false;
        if (GameManager.instance != null && GameManager.instance.gameOver) return false;

        return other.CompareTag("Player") || other.CompareTag("Bomb");
    }

    public void ShowAlarm() 
    {
        if (!enemy.isDead && !GameManager.instance.gameOver)
        {
            StartCoroutine(OnAlarm());
        }
    }

    IEnumerator OnAlarm()
    {
        enemy.alarmSign.SetActive(true);

        Animator alarmAnim = enemy.alarmSign.GetComponent<Animator>();

        if (alarmAnim != null)
        {
            yield return new WaitForSeconds(alarmAnim.GetCurrentAnimatorClipInfo(0)[0].clip.length);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        enemy.alarmSign.SetActive(false);
    }//检测该物体的动画播放完隐藏







    public void SetPatrolColor()
    {
        targetColor = patrolColor;
    }

    public void SetSearchColor()
    {
        targetColor = searchColor;
    }

    public void SetAttackColor()
    {
        targetColor = attackColor;
    }
    public void SetHitColor()
    {
        targetColor = hitColor;
    }
    public void SetChargeSkillColor()
    {
        targetColor = chargeSkillColor;
    }
    public void SetAimThrowSkillColor()
    {
        targetColor = aimThrowSkillColor;
    }
    public void SetBlockColor()
    {
        targetColor = blockColor;
    }
}
