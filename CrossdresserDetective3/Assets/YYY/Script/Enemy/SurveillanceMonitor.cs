using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class SurveillanceMonitor : MonoBehaviour
{
    [Min(1)] public float maxSuspicion = 100;
    [Min(0)] public float suspicionPerSecond = 20;
    [Min(0)] public float recoveryPerSecond = 10;
    private EnemyController owner;
    private PolygonCollider2D vision;
    private Material material;

    private void Start()
    {
        owner = GetComponentInParent<EnemyController>();
        vision = GetComponent<PolygonCollider2D>();
        if (owner != null && !owner.isDead)
        {
            owner.chasePlayerOnSpawn = false;
            if (owner.CheckArea != null) owner.CheckArea.SetActive(false);
            owner.attackList.Clear();
            owner.targetPoint = null;
            owner.TransitionToState(owner.patrolState);
        }
        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            material = renderer.material;
            // Same alpha-zero colour used by EnemyCheckArea's hidden hitColor.
            material.color = new Color(1, 0, 0, 0);
        }
        var player = GameManager.instance != null ? GameManager.instance.player : null;
        if (player == null) return;
        var suspicion = player.GetComponent<PlayerSuspicion>();
        if (suspicion == null) suspicion = player.gameObject.AddComponent<PlayerSuspicion>();
        suspicion.Register(this);
    }

    public bool CanSee(PlayerController player)
    {
        if (!isActiveAndEnabled || vision == null || !vision.enabled ||
            (owner != null && (owner.isDead || owner.IsSuspended))) return false;
        // SimpleFOV rebuilds this polygon using its wall/door obstruction mask.
        foreach (var collider in player.GetComponentsInChildren<Collider2D>())
            if (collider.enabled && !collider.isTrigger && vision.Distance(collider).isOverlapped)
                return true;
        return false;
    }

    public void Reveal()
    {
        if (owner != null) owner.RevealAsHostile();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        if (material != null) Destroy(material);
    }
}
