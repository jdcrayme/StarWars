//#define TRIANGLE_STRIP

using UnityEngine;
using System.Collections.Generic;

namespace Assets.Effects.Thrusters
{
    /// <summary>
    /// Creates a visible trail behind the object.
    /// </summary>

    [AddComponentMenu("Game/Improved Trail")]
    public class ImprovedTrail : MonoBehaviour
    {
        // Material used by the trail renderer
        public Material Material;

        // Allowed length of each segment before a new point gets created
        public float MaxSegmentLength = 1f;

        // Allowed angle difference between segments before a new point gets created
        public float MaxSegmentAngleVariance = 10f;

        // Maximum allowed change in alpha before a new point gets created
        public float MaxSegmentAlphaVariance = 0.1f;

        // Desired lifetime of each emitted point
        public float SegmentLifetime = 1f;

        // Alpha applied to newly emitted points
        public float Alpha = 1f;

        // Velocity applied to each point in units per second
        public Vector3 LocalVelocity = Vector3.zero;

        // Velocity falloff -- the higher it is the quicker the velocity fades away
        public float VelocityFalloffPower = 1f;

        // Alpha based on time
        public AnimationCurve AlphaCurve;

        // Curve used to adjust the size of the trail with time
        public AnimationCurve SizeCurve;

        // Colors used for interpolation
        public Color[] Colors;

        /// <summary>
        /// Normal defining a plane on which all points will be placed.
        /// If left as zero, Camera's plane will be used instead.
        /// For 3D trails, leave this at its default value.
        /// For 2D trails, specify the normal (for example Vector3.up for a flat XZ plane). 
        /// </summary>

        public Vector3 PlaneNormal = Vector3.zero;

        // Destroy the game object when no longer visible
        [HideInInspector]
        public bool DestroyWhenInvisible;

        GameObject _child;
        Transform _transform;
        Transform _camera;
        Mesh _mesh;
        MeshFilter _filter;
        MeshRenderer _meshRenderer;

        class Point
        {
            public Vector3 Pos;
            public Vector3 Dir;
            public Vector3 Cross;
            public Vector3 Vel;
            public Color Col;
            public float Life;
            public float Alpha;
            public float CreationTime;
        }

        readonly List<Point> _points = new List<Point>();
        readonly List<Point> _unused = new List<Point>();

        /// <summary>
        /// Helper function that returns the appropriate color for the specified 0-1 range life.
        /// </summary>
        Color GetColor (float life)
        {
            if (Colors == null || Colors.Length == 0) return Color.white;

            life = Mathf.Clamp01(life);
            life = 1.0f - life;
            life *= (Colors.Length - 1);

            var index = Mathf.FloorToInt(life);
            life -= index;

            return index + 1 < Colors.Length ? Color.Lerp(Colors[index], Colors[index + 1], life) : Colors[index];
        }

        /// <summary>
        /// Add a new point to the list.
        /// </summary>
        Point Add ()
        {
            if (_unused.Count > 0)
            {
                var last = _unused.Count - 1;
                var p = _unused[last];
                p.CreationTime = Time.time;
                p.Vel = LocalVelocity;
                p.Life = 1f;
                _unused.RemoveAt(last);
                _points.Add(p);
                return p;
            }

            var newPoint = new Point
                               {
                                   CreationTime = Time.time,
                                   Vel = LocalVelocity,
                                   Life = 1f
                               };
            _points.Add(newPoint);
            return newPoint;
        }

        /// <summary>
        /// Discard the specified point, placing it into the unused list.
        /// </summary>
        void Discard (Point p) { if (_points.Remove(p)) { _unused.Add(p); } }

        /// <summary>
        /// Create the first 2 points, add a mesh renderer and a mesh filter.
        /// </summary>
        public void Start ()
        {
            _transform = transform;
            _child = new GameObject(GetType() + " for " + name);
            _camera = Camera.main.transform;
            _filter = _child.AddComponent<MeshFilter>();
            _meshRenderer = _child.AddComponent<MeshRenderer>();
            _mesh = new Mesh
                        {
                            name = gameObject.name + " (" + GetType() + ")"
                        };

            // It's always a good idea to name dynamically created meshes
            _filter.mesh = _mesh;
            _meshRenderer.material = Material;

            // Add the first 2 points forming a line
            {
                var p = Add();
                p.Pos = _transform.position;
                p.Dir = Vector3.forward;
                p.Cross = Vector3.right;
                p.Alpha = 0f;

                p = Add();
                p.Pos = _transform.position;
                p.Dir = Vector3.forward;
                p.Cross = Vector3.right;
                p.Alpha = 0f;
            }
        }

        /// <summary>
        /// Enable/disable the trail when this script gets the event.
        /// </summary>

        public void OnEnable () { if (_child != null) _child.SetActive(true); }
        public void OnDisable () { if (_child != null) _child.SetActive(false); }

        /// <summary>
        /// Release all resources we created.
        /// </summary>

        public void OnDestroy ()
        {
            Destroy(_meshRenderer);
            Destroy(_filter);
            Destroy(_child);
            Destroy(_mesh);
        }

        /// <summary>
        /// Emit or cull points, update the mesh.
        /// </summary>

        public void LateUpdate ()
        {
            // Update the last point's position
            {
                Point prev = _points[_points.Count - 2];
                Point curr = _points[_points.Count - 1];

                // Calculate the new position and direction
                Vector3 newPos = _transform.position;
                Vector3 newDir = newPos - prev.Pos;

                // Calculate the distance traveled since the last point creation
                float distance = newDir.magnitude;

                if (distance > 0.001f)
                {
                    // We will want to create a new point if the magnitude exceeded the max length
                    bool create = (distance > MaxSegmentLength);

                    // Normalize the direction
                    newDir *= (1.0f / distance);

                    if (_points.Count == 2)
                    {
                        // We only have 2 points
                        prev.Dir = newDir;
                        prev.Alpha = 0f;
                        create = true;
                    }
                    else if (!create && distance > MaxSegmentLength * 0.5f)
                    {
                        // Check to see if the angle has changed significantly enough
                        float angle = Vector3.Angle(newDir, prev.Dir);
                        create = (angle > MaxSegmentAngleVariance);

                        if (!create)
                        {
                            float alphaChange = Mathf.Abs(Alpha - prev.Alpha);
                            create = (alphaChange > MaxSegmentAlphaVariance);
                        }
                    }

                    // Update the last point
                    curr.Pos = newPos;
                    curr.Dir = newDir;
                    curr.Alpha = Alpha;
                    curr.CreationTime = Time.time;
                    curr.Life = 1f;

                    if (create)
                    {
                        // Add a new point
                        Point newPoint = Add();
                        newPoint.Pos = newPos;
                        newPoint.Dir = newDir;
                        newPoint.Alpha = Alpha;
                    }
                }
            }

            if (_points.Count > 2)
            {
                // How much each emitted point decays this update
                float decay = (SegmentLifetime > 0f) ? Time.deltaTime / SegmentLifetime : 1f;

                // Reduce the life and alpha of all points
                var i = 0;
                int imax;
                for (imax = _points.Count - 1; i < imax; ++i)
                {
                    var p = _points[i];
                    p.Life -= decay;
                    p.Alpha -= decay;
                    if (p.Life < 0f || p.Alpha < 0f) p.Alpha = 0f;
                }

                // Eliminate expired points
                while (_points.Count > 2)
                {
                    var curr = _points[0];
                    var next = _points[1];
                    if (curr.Alpha == 0f && next.Alpha == 0f) Discard(curr);
                    else break;
                }
            }

            // Since the last point is always temporary, we need at least 3 points in order to draw anything
            if (_points.Count > 2)
            {
                var dynamicNormal = (PlaneNormal.x + PlaneNormal.y + PlaneNormal.z != 1f);
                var camPos = _camera.position;

                // Recalculate the cross products
                for (var i = 0; i < _points.Count; ++i)
                {
                    var p = _points[i];
                    p.Col = GetColor(p.Life);

                    // All points except the last should be affected by velocity
                    if (i + 1 < _points.Count)
                    {
                        if (VelocityFalloffPower != 1f && (p.Vel.sqrMagnitude != 0f))
                        {
                            var factor = 1.0f - (Time.time - p.CreationTime) / SegmentLifetime;
                            factor = Mathf.Pow(factor, VelocityFalloffPower);
                            factor *= Time.deltaTime;
                            p.Pos += _transform.TransformDirection(p.Vel) * factor;
                        }
                        else
                        {
                            p.Pos += _transform.TransformDirection(p.Vel) * Time.deltaTime;
                        }
                    }

                    if (dynamicNormal)
                    {
                        // Dynamic normal vector -- use the camera
                        var eyeDir = (camPos - p.Pos).normalized;
                        p.Cross = Vector3.Cross(p.Dir, eyeDir).normalized;
                    }
                    else
                    {
                        // Fixed normal vector -- use the specified value
                        p.Cross = Vector3.Cross(p.Dir, PlaneNormal).normalized;
                    }
                }

                // Update the mesh
                UpdateMesh();
            }
            else
            {
                _mesh.Clear();
                if (DestroyWhenInvisible) Destroy(gameObject);
            }
        }

        /// <summary>
        /// Update the mesh, creating triangles based on our points and the camera's orientation.
        /// </summary>

        void UpdateMesh ()
        {
            // It takes 2 vertices per point in order to draw it
            int pointCount = _points.Count;
            int vertexCount = pointCount << 2;

#if TRIANGLE_STRIP
		int indexCount = vertexCount;
#else
            int indexCount = (pointCount - 1) * 6;
#endif

            var vertices = new Vector3[vertexCount];
            var uvs = new Vector2[vertexCount];
            var cols = new Color[vertexCount];
            var indices = new int[indexCount];

            for (int i = 0; i < pointCount; ++i)
            {
                var p = _points[i];

                // Calculate the combined cross product
                var cross = (i + 1 < pointCount) ? (p.Cross + _points[i + 1].Cross) * 0.5f : p.Cross;

                var time = 1.0f - p.Life;
                cross *= 0.5f * SizeCurve.Evaluate(time);

                var i0 = i << 1;
                var i1 = i0 + 1;

                vertices[i0] = p.Pos + cross;
                vertices[i1] = p.Pos - cross;

                var y = 1.0f - p.Life;
                uvs[i0] = new Vector2(0f, y);
                uvs[i1] = new Vector2(1f, y);

                var c = p.Col;
                c.a *= p.Alpha * AlphaCurve.Evaluate(time);
                cols[i0] = c;
                cols[i1] = c;

#if TRIANGLE_STRIP
			indices[i0] = i0;
			indices[i1] = i1;
#endif
            }

#if !TRIANGLE_STRIP
            // Calculate the indices
            for (int i = 0, index = 0; index < indexCount; i += 2)
            {
                indices[index++] = i;
                indices[index++] = i + 2;
                indices[index++] = i + 1;

                indices[index++] = i + 1;
                indices[index++] = i + 2;
                indices[index++] = i + 3;
            }
#endif
            // Update the mesh
            _mesh.Clear();
            _mesh.vertices = vertices;
            _mesh.uv = uvs;
            _mesh.colors = cols;

            // NOTE: Triangle strip approach seems to connect the last triangle with the first. Unity bug?
#if TRIANGLE_STRIP
		    mMesh.SetTriangleStrip(indices, 0);
#else
            _mesh.triangles = indices;
#endif
            _mesh.RecalculateBounds();

            // Cleanup so Unity crashes less
            vertices = null;
            uvs = null;
            cols = null;
            indices = null;
        }
    }
}