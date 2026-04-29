using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SimpleFOV
{
    /// <summary>
    /// A fov class to draw a sector-like fov mesh.
    /// To get started to use the script from scratch:
    /// Add a new empty child object to the object which you wish to have the FOV
    /// Add MeshRenderer component to the new object and 
    /// Add MeshFilter component to the new object
    /// The world scale of the object will also make FOV scaled.
    /// </summary>
    public class FieldOfView3D : MonoBehaviour
    {
        [Header("Basic")]
        [SerializeField]
        [Tooltip("The radius of the view")]
        private float viewRadius = 2f;

        [SerializeField]
        [Range(0, 360)]
        [Tooltip("The angle of the view. Will take the transform right as the center line to display this.")]
        private float viewAngle = 30f;

        [SerializeField]
        [Range(0, 30)]
        [Tooltip("The angle step for one triangle. Decrese the value to get a more good-looking sector/circle, but it affects performance.")]
        private float angleStep = 3f;

        [SerializeField]
        [Tooltip("The layer mask that fov should detect the obstacle (blocker object) using the raycast.")]
        private LayerMask layerMask;

        [SerializeField]
        [Tooltip("Automatically update fov. Turn this off to control the update yourself by calling UpdateFOV " +
            "or if you don't need to update the fov.")]
        private bool autoUpdateFov = true;

        [Header("Advanced: circle around the object")]
        [SerializeField]
        [Tooltip("Turn this on to also have a circle around the object.")]
        private bool createWithCircleAround = false;

        [SerializeField]
        [Tooltip("The radius of the circle around the object. This is only effective if createWithCircleAround is on.")]
        private float circleRadius = 0f;

        [Header("Detection: blockers or non-blockers")]
        [SerializeField]
        [Tooltip("Collect all the objects detected including non-blocker objects when scanning. " +
            "Note! Enabling this costs extra time runtime.")]
        private bool shouldDetectNonBlockers = false;

        [SerializeField]
        [Tooltip("The layers which need to be detected as non-blocker. Note that if you include the blocker layer mask " +
            "(defined in layerMask field), it will still report the object even though it's a blocker.")]
        private LayerMask nonBlockerLayerMask;

        [Header("Other settings for editor")]
        [SerializeField]
        [Tooltip("Enable this to show brief lines representing the fov area in editor scene.")]
        private bool editorPreview = true;

        [SerializeField]
        [Tooltip("Visualize the detected point when detecting an object inside fov. Only effective when in Unity editor.")]
        private bool visualizeDetectedPoint = false;

        [SerializeField]
        [Tooltip("Customize the debugging color for detection point.")]
        private Color detectionVisualizationColor = Color.yellow;

        [Header("Events")]
        // Triggered when blocker object is detected (at max once per detected object per scan).
        // Arguments: fov3D holder game object, detected game object, detected point
        public BlockerDetectedEvent BlockerDetected;
        // Triggered when non-blocker object is detected (at max once per detected object per scan).
        // Arguments: fov3D holder game object, detected game object, detected point
        public NonBlockerDetectedEvent NonBlockerDetected;

        private Mesh mesh;  // the mesh we will draw the fov
        private List<DetectionInfo> detectedBlockersCollection = new List<DetectionInfo>();
        private List<DetectionInfo> detectedNonBlockersCollection = new List<DetectionInfo>();

        /// <summary>
        /// Call this to manually update the FOV mesh.
        /// If using this, consider disable "autoUpdateFov"
        /// </summary>
        public void UpdateFOV()
        {
            detectedBlockersCollection.Clear();
            detectedNonBlockersCollection.Clear();

            if (viewAngle <= 0 || angleStep <= 0)
                return;

            if (!createWithCircleAround || circleRadius == 0f)
            {
                if (viewRadius <= 0f)
                    return;

                DrawSimpleFOVSector();
            }
            else
            {
                if (viewRadius < 0f || angleStep <= 0 || viewRadius < 0)
                    return;

                DrawFOVSectorWithCircle();
            }

            RaiseDetectionEvent();
        }

        /// <summary>
        /// Set or get the view radius
        /// </summary>
        public float ViewRadius
        {
            get { return viewRadius; }
            set
            {
                viewRadius = value;
            }
        }

        /// <summary>
        /// All detected blockers in previous scan
        /// </summary>
        public IReadOnlyList<DetectionInfo> DetectedBlockersList
        {
            get { return detectedBlockersCollection.ToList(); }
        }

        /// <summary>
        /// All detected non-blockers in previous scan
        /// </summary>
        public IReadOnlyList<DetectionInfo> DetectedNonBlockersList
        {
            get { return detectedNonBlockersCollection.ToList(); }
        }

        private void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;

            UpdateFOV();
        }

        private void FixedUpdate()
        {
            if (autoUpdateFov)
            {
                UpdateFOV();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// A rough preview in editor scene view
        /// </summary>
        private void OnDrawGizmos()
        {
            if (editorPreview)
            {
                var endPoint1 = viewRadius * GetVectorForAngle(viewAngle / 2);
                var endPoint2 = viewRadius * GetVectorForAngle(-viewAngle / 2);
                var midPoint = viewRadius * GetVectorForAngle(0);
                Gizmos.DrawLine(transform.position, transform.TransformPoint(endPoint1));
                Gizmos.DrawLine(transform.position, transform.TransformPoint(endPoint2));
                Gizmos.DrawLine(transform.position, transform.TransformPoint(midPoint));
            }

            if (visualizeDetectedPoint)
            {
                foreach (var detection in detectedBlockersCollection)
                {
                    DrawDebugSphere(detection.DetectedPoint);
                }

                if (shouldDetectNonBlockers)
                {
                    foreach (var detection in detectedNonBlockersCollection)
                    {
                        DrawDebugSphere(detection.DetectedPoint);
                    }
                }
            }
        }

        private void DrawDebugSphere(Vector3 detectedPoint, float sphereRadius = 0.2f)
        {
            // Draw detection point sphere
            Gizmos.color = detectionVisualizationColor;
            Gizmos.DrawWireSphere(detectedPoint, sphereRadius);

            // Cross at detection point for better visibility
            float crossSize = sphereRadius * 0.7f;
            Vector3 point = detectedPoint;
            Gizmos.DrawLine(point + Vector3.up * crossSize, point - Vector3.up * crossSize);
            Gizmos.DrawLine(point + Vector3.right * crossSize, point - Vector3.right * crossSize);
            Gizmos.DrawLine(point + Vector3.forward * crossSize, point - Vector3.forward * crossSize);
        }
#endif

        /// <summary>
        /// Raise detection events
        /// </summary>
        private void RaiseDetectionEvent()
        {
            foreach (var blockerInfo in detectedBlockersCollection)
            {
                BlockerDetected?.Invoke(blockerInfo.FovObject, blockerInfo.DetectedObject, blockerInfo.DetectedPoint);
            }

            if (shouldDetectNonBlockers)
            {
                foreach (var nonBlockerInfo in detectedNonBlockersCollection)
                {
                    NonBlockerDetected?.Invoke(nonBlockerInfo.FovObject, nonBlockerInfo.DetectedObject, nonBlockerInfo.DetectedPoint);
                }
            }
        }

        /// <summary>
        /// Draw a simple sector shape FOV
        /// </summary>
        private void DrawSimpleFOVSector()
        {
            var detectedBlockersUnique = new HashSet<GameObject>();
            var detectedNonBlockersUnique = new HashSet<GameObject>();

            int rayStepCount = Mathf.FloorToInt(viewAngle / angleStep);
            float angleIncrease = viewAngle / rayStepCount;

            Vector3[] vertices = new Vector3[rayStepCount + 2];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] indices = new int[rayStepCount * 3];

            float angle = viewAngle / 2;
            int triangleCnt = 0;
            vertices[0] = Vector3.zero;
            for (int i = 1; i < rayStepCount + 2; i++)
            {
                Vector3 direction = transform.TransformDirection(GetVectorForAngle(angle)).normalized;
                if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, viewRadius, layerMask))
                {
                    vertices[i] = transform.InverseTransformPoint(hitInfo.point);    // mesh is in local space!
                    GameObject hitObj = hitInfo.collider.gameObject;
                    if (!detectedBlockersUnique.Contains(hitObj))
                    {
                        detectedBlockersUnique.Add(hitObj);
                        detectedBlockersCollection.Add(new DetectionInfo(gameObject, hitObj, hitInfo.point));
                    }
                }
                else
                {
                    vertices[i] = transform.InverseTransformPoint(transform.position + direction * viewRadius);
                }

                // non-blocker detection
                if (shouldDetectNonBlockers)
                {
                    float distance = Vector3.Distance(transform.position, transform.TransformPoint(vertices[i]));
                    if (Physics.Raycast(transform.position, direction, out RaycastHit nonBlockerHit,
                        distance, nonBlockerLayerMask))
                    {
                        GameObject hitObj = nonBlockerHit.collider.gameObject;
                        if (!detectedNonBlockersUnique.Contains(hitObj))
                        {
                            detectedNonBlockersUnique.Add(hitObj);
                            detectedNonBlockersCollection.Add(new DetectionInfo(gameObject, hitObj,
                                nonBlockerHit.point));
                        }
                    }
                }

                if (i > 1)
                {
                    indices[triangleCnt] = 0;
                    indices[triangleCnt + 1] = i - 1;
                    indices[triangleCnt + 2] = i;
                    triangleCnt += 3;
                }

                angle -= angleIncrease;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.uv = uvs;
        }


        /// <summary>
        /// Draw a circle + sector shape FOV
        /// </summary>
        private void DrawFOVSectorWithCircle()
        {
            var detectedBlockersUnique = new HashSet<GameObject>();
            var detectedNonBlockersUnique = new HashSet<GameObject>();

            int rayStepCount = Mathf.FloorToInt(viewAngle / angleStep);
            float angleIncrease = viewAngle / rayStepCount;
            float circleAngle = 360 - viewAngle;
            int circleStepCount = Mathf.FloorToInt(circleAngle / angleStep);
            float circleAngleIncrease = circleAngle / circleStepCount;

            Vector3[] vertices = new Vector3[rayStepCount + circleStepCount + 3];
            Vector2[] uvs = new Vector2[vertices.Length];
            int[] indices = new int[(rayStepCount + circleStepCount) * 3];

            float angle = viewAngle / 2;
            int triangleCnt = 0;
            vertices[0] = Vector3.zero;
            // draw sector
            for (int i = 1; i < rayStepCount + 2; i++)
            {
                Vector3 direction = transform.TransformDirection(GetVectorForAngle(angle)).normalized;
                if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, viewRadius, layerMask))
                {
                    vertices[i] = transform.InverseTransformPoint(hitInfo.point);    // mesh is in local space!

                    GameObject hitObj = hitInfo.collider.gameObject;
                    if (!detectedBlockersUnique.Contains(hitObj))
                    {
                        detectedBlockersUnique.Add(hitObj);
                        detectedBlockersCollection.Add(new DetectionInfo(gameObject, hitObj, hitInfo.point));
                    }
                }
                else
                {
                    vertices[i] = transform.InverseTransformPoint(transform.position + direction * viewRadius);
                }

                // non-blocker detection
                if (shouldDetectNonBlockers)
                {
                    float distance = Vector3.Distance(transform.position, transform.TransformPoint(vertices[i]));
                    if (Physics.Raycast(transform.position, direction, out RaycastHit nonBlockerHit,
                        distance, nonBlockerLayerMask))
                    {
                        GameObject hitObj = nonBlockerHit.collider.gameObject;
                        if (!detectedNonBlockersUnique.Contains(hitObj))
                        {
                            detectedNonBlockersUnique.Add(hitObj);
                            detectedNonBlockersCollection.Add(new DetectionInfo(gameObject, hitObj, nonBlockerHit.point));
                        }
                    }
                }

                if (i > 1)
                {
                    indices[triangleCnt] = 0;
                    indices[triangleCnt + 1] = i - 1;
                    indices[triangleCnt + 2] = i;
                    triangleCnt += 3;
                }

                angle -= angleIncrease;
            }

            // draw circle for the rest of the angles
            int startingIndex = rayStepCount + 2;
            angle += angleIncrease;
            for (int j = 0; j < circleStepCount + 1; j++)
            {
                Vector3 direction = transform.TransformDirection(GetVectorForAngle(angle)).normalized;
                if (Physics.Raycast(transform.position, direction, out RaycastHit hitInfo, circleRadius, layerMask))
                {
                    vertices[j + startingIndex] = transform.InverseTransformPoint(hitInfo.point);    // mesh is in local space

                    GameObject hitObj = hitInfo.collider.gameObject;
                    if (!detectedBlockersUnique.Contains(hitObj))
                    {
                        detectedBlockersUnique.Add(hitObj);
                        detectedBlockersCollection.Add(new DetectionInfo(gameObject, hitObj, hitInfo.point));
                    }
                }
                else
                {
                    vertices[j + startingIndex] = transform.InverseTransformPoint(transform.position + direction * circleRadius);
                }

                // non-blocker detection
                if (shouldDetectNonBlockers)
                {
                    float distance = Vector3.Distance(transform.position, transform.TransformPoint(vertices[j]));
                    if (Physics.Raycast(transform.position, direction, out RaycastHit nonBlockerHit,
                        distance, nonBlockerLayerMask))
                    {
                        GameObject hitObj = nonBlockerHit.collider.gameObject;
                        if (!detectedNonBlockersUnique.Contains(hitObj))
                        {
                            detectedNonBlockersUnique.Add(hitObj);
                            detectedNonBlockersCollection.Add(new DetectionInfo(gameObject, hitObj, nonBlockerHit.point));
                        }
                    }
                }

                if (j >= 1)
                {
                    indices[triangleCnt] = 0;
                    indices[triangleCnt + 1] = startingIndex + j - 1;
                    indices[triangleCnt + 2] = startingIndex + j;
                    triangleCnt += 3;
                }

                angle -= circleAngleIncrease;
            }

            mesh.Clear();
            mesh.vertices = vertices;
            mesh.triangles = indices;
            mesh.uv = uvs;
        }

        private Vector3 GetVectorForAngle(float angle)
        {
            float angleRad = angle * Mathf.PI / 180f;
            return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
        }
    }
}