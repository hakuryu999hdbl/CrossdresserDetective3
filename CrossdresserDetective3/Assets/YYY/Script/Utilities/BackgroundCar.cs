using UnityEngine;

public class BackgroundCar : MonoBehaviour
{
    [Header("车速等")]
    public bool originallyFacesRight = false;

    private float targetX;
    private float speed;
    private float direction;
    private bool initialized;

    [Header("Y拉高")]
    public float spawnYOffset = 0f;


    public void Initialize(float receiverX, float moveSpeed)
    {
        targetX = receiverX;
        speed = Mathf.Abs(moveSpeed);
        direction = targetX >= transform.position.x ? 1f : -1f;

        // ｷｭﾗｪﾕ鈆ｾｳｵ｣ｬｳｵﾉ晗ﾍﾂﾖﾗﾓﾒｻﾆｭﾗｪ｡｣
        Vector3 scale = transform.localScale;
        bool movingRight = direction > 0f;
        bool needsFlip = movingRight != originallyFacesRight;

        scale.x = Mathf.Abs(scale.x) * (needsFlip ? -1f : 1f);
        transform.localScale = scale;

        initialized = true;
    }

    private void Update()
    {
        if (!initialized)
            return;

        Vector3 position = transform.position;

        // ﾖｻｸﾄｱ・X｣ｬｱ｣ｳﾖﾉ嵭ﾉｵ羞ﾄ Y｡｢Z｡｣
        position.x = Mathf.MoveTowards(
            position.x, targetX, speed * Time.deltaTime);

        transform.position = position;

        if (position.x == targetX)
            Destroy(gameObject);
    }
}