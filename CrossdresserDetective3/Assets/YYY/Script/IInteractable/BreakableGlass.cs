using UnityEngine;

[RequireComponent(typeof(Character), typeof(BoxCollider2D))]
public class BreakableGlass : MonoBehaviour
{
    [SerializeField] private BreakDebrisEffect breakEffect;
    private Character character;

    private void Awake()
    {
        character = GetComponent<Character>();
    }

    private void OnEnable()
    {
        character.OnDie.AddListener(Break);
    }

    private void OnDisable()
    {
        character.OnDie.RemoveListener(Break);
    }

    public void Break()
    {
        if (!gameObject.activeInHierarchy)
            return;

        if (breakEffect != null)
            breakEffect.Spawn(GetComponent<BoxCollider2D>().bounds, GetComponent<SpriteRenderer>());

        gameObject.SetActive(false);
    }
}
