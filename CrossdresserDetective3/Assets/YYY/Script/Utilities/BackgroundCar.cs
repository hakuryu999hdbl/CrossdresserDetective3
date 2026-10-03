using UnityEngine;

public class BackgroundCar : MonoBehaviour
{
    [Header("图片原本的车头是否朝右")]
    public bool originallyFacesRight = false;

    private float targetX;
    private float speed;
    private float direction;
    private bool initialized;

    [Header("生成高度偏移")]
    public float spawnYOffset = 0f;


    public void Initialize(float receiverX, float moveSpeed)
    {
        targetX = receiverX;
        speed = Mathf.Abs(moveSpeed);
        direction = targetX >= transform.position.x ? 1f : -1f;

        // 翻转整辆车，车身和轮子一起翻转。
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

        // 只改变 X，保持生成点的 Y、Z。
        position.x = Mathf.MoveTowards(
            position.x, targetX, speed * Time.deltaTime);

        transform.position = position;

        if (position.x == targetX)
            Destroy(gameObject);
    }
}