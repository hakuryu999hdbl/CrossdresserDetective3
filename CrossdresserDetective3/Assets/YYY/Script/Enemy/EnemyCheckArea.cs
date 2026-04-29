using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCheckArea : MonoBehaviour
{
    [Header("传递消息")]
    public EnemyController enemy;

    [Header("视野颜色")]
    public Color normalColor = new Color(0f, 1f, 0f, 0.35f);
    public Color alertColor = new Color(1f, 0f, 0f, 0.35f);
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
            targetColor = normalColor;
            fovMaterial.color = normalColor;
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
        if (!IsValidTarget(other)) return;

        enemy.OnCheckAreaStay(other);
        targetColor = alertColor;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!IsValidTarget(other)) return;

        enemy.OnCheckAreaExit(other);

        if (enemy.attackList.Count <= 0)
        {
            targetColor = normalColor;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsValidTarget(other)) return;

        if (!enemy.isDead && !GameManager.instance.gameOver)
        {
            targetColor = alertColor;
            StartCoroutine(OnAlarm());
        }
    }

    private bool IsValidTarget(Collider2D other)
    {
        if (enemy == null) return false;
        if (enemy.isDead) return false;
        if (GameManager.instance != null && GameManager.instance.gameOver) return false;

        return other.CompareTag("Player") || other.CompareTag("Bomb");
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

}
