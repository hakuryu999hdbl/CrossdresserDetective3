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
    public class FieldOfView2D : MonoBehaviour
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
        [Tooltip("The layer mask that fov should detect the obstacle using the raycast.")]
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

        [Header("Other settings")]
        [SerializeField]
        [Tooltip("Enable this to show brief lines representing the fov area in editor scene.")]
        private bool editorPreview = true;

        [SerializeField]
        [Tooltip("Enable this to update the polygon collider attached based on the fov mesh. " +
            "You need a polygon collider on the same component to get it work.")]
        private bool shouldUpdateCollider = false;

        private Mesh mesh;  // the mesh we will draw the fov
        private PolygonCollider2D fovCollider2D;  // the 2d collider in this component, can be null

        /// <summary>
        /// Call this to manually update the FOV mesh.
        /// If using this, consider disable "autoUpdateFov"
        /// </summary>
        public void UpdateFOV()
        {
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

        private void Start()
        {
            mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = mesh;
            fovCollider2D = GetComponent<PolygonCollider2D>();

            UpdateFOV();
        }

        private void FixedUpdate()
        {
            if (autoUpdateFov)
            {
                UpdateFOV();
            }
        }

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
        }

        /// <summary>
        /// Draw a simple sector shape FOV
        /// </summary>
        private void DrawSimpleFOVSector()
        {
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
                RaycastHit2D rayHit = Physics2D.Raycast(
                    transform.position,
                    direction,
                    viewRadius, layerMask);    // mesh is in local space

                if (rayHit.collider == null)
                {
                    vertices[i] = transform.InverseTransformPoint(transform.position + direction * viewRadius);
                }
                else
                {
                    Vector3 hitPoint = rayHit.point;
                    vertices[i] = transform.InverseTransformPoint(hitPoint);    // mesh is in local space
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

            UpdateCollider(vertices, indices);
        }

        /// <summary>
        /// Draw a circle + sector shape FOV
        /// </summary>
        private void DrawFOVSectorWithCircle()
        {
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
            for (int i = 1; i < rayStepCount + 2; i++)
            {
                Vector3 direction = transform.TransformDirection(GetVectorForAngle(angle)).normalized;
                RaycastHit2D rayHit = Physics2D.Raycast(
                    transform.position,
                    direction,
                    viewRadius, layerMask);    // mesh is in local space

                if (rayHit.collider == null)
                {
                    vertices[i] = transform.InverseTransformPoint(transform.position + direction * viewRadius);
                }
                else
                {
                    Vector3 hitPoint = rayHit.point;
                    vertices[i] = transform.InverseTransformPoint(hitPoint);    // mesh is in local space
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

            int startingIndex = rayStepCount + 2;
            angle += angleIncrease;
            for (int j = 0; j < circleStepCount + 1; j++)
            {
                Vector3 directionCircle = transform.TransformDirection(GetVectorForAngle(angle)).normalized;
                RaycastHit2D rayHit = Physics2D.Raycast(
                    transform.position, directionCircle,
                    circleRadius, layerMask);    // mesh is in local space

                if (rayHit.collider == null)
                {
                    vertices[j + startingIndex] = transform.InverseTransformPoint(transform.position + directionCircle * circleRadius);
                }
                else
                {
                    Vector3 hitPoint = rayHit.point;
                    vertices[j + startingIndex] = transform.InverseTransformPoint(hitPoint);    // mesh is in local space
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

            UpdateCollider(vertices, indices);
        }

        /// <summary>
        /// Update collider, if shouldUpdateCollider and collider exists
        /// </summary>
        private void UpdateCollider(Vector3[] vertices, int[] triangles)
        {
            if (!shouldUpdateCollider || fovCollider2D == null )
                return;

            Vector2[] colliderVertices = new Vector2[vertices.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                colliderVertices[i] = vertices[i];
            }

            fovCollider2D.SetPath(0, colliderVertices);
        }

        private Vector3 GetVectorForAngle(float angle)
        {
            float angleRad = angle * Mathf.PI / 180f;
            return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad), 0);
        }
    }
}