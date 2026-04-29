using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace SimpleFOV
{
    /// <summary>
    /// The detection information
    /// </summary>
    [Serializable]
    public struct DetectionInfo 
    {
        public GameObject FovObject;   // the object which has the FieldOfView3D component
        public GameObject DetectedObject;  // the detected game object
        public Vector3 DetectedPoint;  // the "hit point" where the fov raycasting finds the object

        public DetectionInfo(GameObject fovObject, GameObject detectedObject, Vector3 detectedPoint)
        {
            FovObject = fovObject;
            DetectedObject = detectedObject;
            DetectedPoint = detectedPoint;
        }
    }

    /// <summary>
    /// Raised every time when a blocker is detected (only trigger once for unique detected game object per scan)
    /// Blocker is the ones set in "layerMask" field in FieldOfView3D component
    /// </summary>
    [Serializable]
    public class BlockerDetectedEvent : UnityEvent<GameObject, GameObject, Vector3> { };

    /// <summary>
    /// Raised every time when a non-blocker is detected (only trigger once for unique detected game object per scan)
    /// Set the non-blocker detection in the "Detection: blockers or non-blockers" section in FieldOfView3D component
    /// </summary>
    [Serializable]
    public class NonBlockerDetectedEvent : UnityEvent<GameObject, GameObject, Vector3> { };
}