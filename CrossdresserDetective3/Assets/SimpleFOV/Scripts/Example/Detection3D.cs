using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleFOV.Example
{
    /// <summary>
    /// This is used in examples.
    /// This script shows how the detection of objects inside FOV can be potentially done.
    /// </summary>
    public class Detection3D : MonoBehaviour
    {
        [Serializable]
        public enum ReportStyle
        {
            ReportAll,
            ReportWhenChanged,
        }

        [SerializeField]
        [Tooltip("This shows example of two ways of getting detected objects.")]
        private ReportStyle detectionLoggingStyle;

        private FieldOfView3D fov3D;
        private HashSet<GameObject> savedDetectedObjects = new HashSet<GameObject>();

        private void Start()
        {
            fov3D = GetComponent<FieldOfView3D>();

            // **** You can either bind to unity events *****
            // blocker detection
            // in the example, it's the "obstacle" objects
            fov3D.BlockerDetected.AddListener(OnBlockerDetected);

            // non-blocker detection
            // Since this costs extra time, you need to enable "shouldDetectNonBlockers" in FOV3D component,
            // and define your layer mask "nonBlockerLayerMask" to receive this event
            fov3D.NonBlockerDetected.AddListener(OnNonBlockerDetected);
        }

        private void Update()
        {
            // ******* Or you can just get the list of detected objects in Update() and do whatever ******
            CheckDetectedObjectsPerScan();
        }

        private void CheckDetectedObjectsPerScan()
        {
            // in this example, if you have ReportWhenChanged mode, then you only get a log when 
            // the detected objects change
            var currentDetectedObjects = new HashSet<GameObject>();
            foreach (var info in fov3D.DetectedBlockersList)
            {
                currentDetectedObjects.Add(info.DetectedObject);
            }

            foreach (var info in fov3D.DetectedNonBlockersList)
            {
                currentDetectedObjects.Add(info.DetectedObject);
            }

            if(!savedDetectedObjects.SetEquals(currentDetectedObjects))
            {
                savedDetectedObjects = currentDetectedObjects;
                PrintObjects(savedDetectedObjects.ToList());
            }
        }

        private void OnBlockerDetected(GameObject fovObj, GameObject dectectedObj, Vector3 hitPoint)
        {
            if (detectionLoggingStyle == ReportStyle.ReportAll)
            {
                Debug.Log($"{fovObj.name} detects blocker {dectectedObj.name} at point {hitPoint}");
            }
        }

        private void OnNonBlockerDetected(GameObject fovObj, GameObject dectectedObj, Vector3 hitPoint)
        {
            if (detectionLoggingStyle == ReportStyle.ReportAll)
            {
                Debug.Log($"{fovObj.name} detects non-blocker {dectectedObj.name} at point {hitPoint}");
            }
        }

        // a helper to print the detected obj when debugging
        private void PrintObjects(List<GameObject> objects)
        {
            string res = "All detected objects: ";
            foreach (var obj in objects)
            {
                res += obj.name + "; ";
            }

            Debug.Log(res);
        }
    }
}