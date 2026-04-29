using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleFOV.Example
{
    /// <summary>
    /// This is used in examples. This will print a debug message to say it detects something
    /// </summary>
    public class Detection2D : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.name == "Enemy")
            {
                Debug.Log("Detected");
            }
        }
    }
}