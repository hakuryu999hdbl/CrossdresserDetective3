using System.Collections;
using UnityEngine;

namespace SimpleFOV.Example
{
    /// <summary>
    /// This is used in examples. This will rotate the object based on the rotationStops (a list of angle Vector3)
    /// </summary>
    public class RotateSelf : MonoBehaviour
    {
        [SerializeField] private Vector3[] rotationStops = new Vector3[] { };   // will loop through and repeat
        [SerializeField] private float rotatingDuration = 1f;
        [SerializeField] private float stopTime = 0.5f;
        [SerializeField] private bool randomInterval;

        protected virtual void Start()
        {
            StartCoroutine(RandomDelayStartRotate());
        }

        private IEnumerator RandomDelayStartRotate()
        {
            if (rotationStops.Length > 0)
                transform.rotation = Quaternion.Euler(rotationStops[0]);
            if (randomInterval)
                yield return new WaitForSeconds(Random.Range(0f, 0.5f));
            StartCoroutine(RotateRoutine());
        }

        protected IEnumerator RotateRoutine()
        {
            int ind = 0;
            while (ind < rotationStops.Length)
            {
                if (ind + 1 < rotationStops.Length)
                {
                    yield return StartCoroutine(
                        RotateBetweenTwoAnglesRoutine(rotationStops[ind], rotationStops[ind + 1], rotatingDuration));
                    ind++;
                }
                else
                {
                    yield return StartCoroutine(
                        RotateBetweenTwoAnglesRoutine(rotationStops[ind], rotationStops[0], rotatingDuration));
                    ind = 0;
                }
                yield return new WaitForSeconds(stopTime);
            }
        }

        protected IEnumerator RotateBetweenTwoAnglesRoutine(Vector3 fromRot, Vector3 toRot, float duration)
        {
            Quaternion fromQ = Quaternion.Euler(fromRot);
            Quaternion toQ = Quaternion.Euler(toRot);

            float elapsedTime = 0f;
            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(fromQ, toQ, elapsedTime / duration);
                yield return null;
            }
            transform.rotation = toQ;
        }
    }
}