using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpFX : MonoBehaviour
{
    public void Finish() 
    {
        gameObject.SetActive(false);
    }

    public void DestroyThis()
    {
        Destroy(this.gameObject);
    }
}
