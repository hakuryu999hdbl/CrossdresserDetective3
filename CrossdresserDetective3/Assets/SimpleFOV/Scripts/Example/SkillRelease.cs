using System.Collections;
using UnityEngine;

namespace SimpleFOV.Example
{
    public class SkillRelease : MonoBehaviour
    {
        [SerializeField] private FieldOfView3D fov3D;
        [SerializeField] private float targetRadius = 2f;
        [SerializeField] private float skillReleaseTime = 1.5f;
        [SerializeField] private float stopTime = .3f;

        private bool routineStarted;

        private void Update()
        {
            // I do it in update, but you can prioritize the FieldOfView3D script to execute earlier than default
            // so then you won't get errors if you start this coroutine in Start()
            if (!routineStarted)
            {
                routineStarted = true;
                StartCoroutine(SkillReleaseRoutine());
            }
        }

        private IEnumerator SkillReleaseRoutine()
        {
            while (true)    // loop forever
            {
                float elapsedTime = 0f;
                while (elapsedTime < skillReleaseTime)
                {
                    elapsedTime += Time.deltaTime;
                    fov3D.ViewRadius = targetRadius * (elapsedTime / skillReleaseTime);
                    fov3D.UpdateFOV();
                    yield return null;
                }

                yield return new WaitForSeconds(stopTime);
            }
        }
    }
}
