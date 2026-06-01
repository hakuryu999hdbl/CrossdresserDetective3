using UnityEngine;

public class MagazineCase : MonoBehaviour
{
    public float lifeTime = 1.5f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}