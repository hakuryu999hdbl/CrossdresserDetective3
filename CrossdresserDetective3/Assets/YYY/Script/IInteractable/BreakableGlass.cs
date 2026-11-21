using UnityEngine;

[RequireComponent(typeof(Character), typeof(BoxCollider2D))]
public class BreakableGlass : MonoBehaviour
{
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

    private void Break()
    {
        gameObject.SetActive(false);
    }
}
