using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("拖入玩家或实际跟随目标")]
    Transform followTarget;

    [Range(0f, 1f)]
    float parallaxFactor = 0.07f;

    private Vector3 startTargetPos;
    private Vector3 startLayerPos;

    private void Start()
    {

        followTarget = GameManager.instance.player.gameObject.transform;

        if (followTarget == null)
        {
            Debug.LogError($"{name} 没有设置 followTarget");
            enabled = false;
            return;
        }

        startTargetPos = followTarget.position;
        startLayerPos = transform.position;
    }

    private void LateUpdate()
    {
        float deltaX = followTarget.position.x - startTargetPos.x;

        transform.position = new Vector3(
            startLayerPos.x + deltaX * parallaxFactor,
            startLayerPos.y,
            startLayerPos.z
        );
    }
}