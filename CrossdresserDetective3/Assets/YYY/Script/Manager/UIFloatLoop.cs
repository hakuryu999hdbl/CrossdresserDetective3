using UnityEngine;

public class UIFloatLoop : MonoBehaviour
{
    [Header("Î»ÖÃÆ¯¸¡")]
    public bool move = true;
    public float moveX = 5f;
    public float moveY = 8f;
    public float moveSpeed = 1f;

    [Header("Ðý×ª")]
    public bool rotate = false;
    public float rotateAngle = 1f;
    public float rotateSpeed = 0.8f;

    [Header("Ëõ·ÅºôÎü")]
    public bool scale = false;
    public float scaleAmount = 0.015f;
    public float scaleSpeed = 1f;

    [Header("´í¿ª¶¯»­")]
    public float phase = 0f;

    private RectTransform rect;
    private Vector2 startPos;
    private Vector3 startScale;
    private Quaternion startRotation;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();

        startPos = rect.anchoredPosition;
        startScale = rect.localScale;
        startRotation = rect.localRotation;
    }

    private void Update()
    {
        float time = Time.unscaledTime;

        // --------------------
        // Î»ÖÃ
        // --------------------
        if (move)
        {
            float x = Mathf.Sin(time * moveSpeed + phase) * moveX;
            float y = Mathf.Sin(time * moveSpeed * 0.73f + phase) * moveY;

            rect.anchoredPosition = startPos + new Vector2(x, y);
        }

        // --------------------
        // Ðý×ª
        // --------------------
        if (rotate)
        {
            float angle =
                Mathf.Sin(time * rotateSpeed + phase)
                * rotateAngle;

            rect.localRotation =
                startRotation * Quaternion.Euler(0, 0, angle);
        }

        // --------------------
        // ºôÎüËõ·Å
        // --------------------
        if (scale)
        {
            float amount =
                1f + Mathf.Sin(time * scaleSpeed + phase)
                * scaleAmount;

            rect.localScale = startScale * amount;
        }
    }
}