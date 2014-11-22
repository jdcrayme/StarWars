using System.Linq;
using Assets.Game.Ships.Scripts;
using Assets.Ships.Scripts.AI;
using UnityEngine;

namespace Assets.Game.Managers.CameraManager.TacticalView
{
    public class CarteasianGrid : MonoBehaviour
    {
        #region Constants
        public enum GridIcon { Circle, Box, Dimond, Triangle, Rectangle }
        #endregion

        #region Public Fields
        public static CarteasianGrid Instance;  //The singleton instance of the grid

        public int NumLines = 100;              //Number of grid lines
        public int MinorLineRatio = 5;          //Minor lines per major line
        public float CellSize = 20f;            //The spacing of the lines from eachother

        // Line colors
        public Color MajorLineColor = new Color(1.0f, 1.0f, 1.0f, 0.05f);
        public Color MinorLineColor = new Color(1.0f, 1.0f, 1.0f, 0.025f);
        public Color LeaderLineColor = new Color(0.5f, 0.5f, 0.5f, 0.05f);

        //public GameObject Label3D;              //The label prefab used for contacts

        //public float GridStartFadeIn = 1000;    //The camera distance at which the grid will begin to fade in
        //public float GridEndFadeIn = 1500;      //The camera distance at which the grid will be fully faded in
        //[HideInInspector]public float Alpha;    //The grid alpha

        //public Material Background;             //The grid background
        //public FreeOrbitCamera OrbitCam;        //The camera that we use to look around 
        #endregion

        #region Private Fields
        private static Material _lineMaterial;      //The material that we use to draw the lines

        //Various alphas
        private float _gridMajAlpha;
        private float _grifMinAlpha;
        private float _gridLeadAlpha;
        
        private float _corner;                      //The starting corner of the grid
        private float _verticalOffset;              //How far below the origen the grid is
        #endregion

        #region Override Methods
        /// <summary>
        /// Initializes the grid
        /// </summary>
        public void Start()
        {
            Instance = this;

            _gridMajAlpha = MajorLineColor.a;
            _grifMinAlpha = MinorLineColor.a;
            _gridLeadAlpha = LeaderLineColor.a;

            _corner = -NumLines * CellSize / 2;
            _verticalOffset = -_corner / 5;
        }

        /// <summary>
        /// Updates the grid
        /// </summary>
        public void Update()
        {
            MajorLineColor.a = _gridMajAlpha;
            MinorLineColor.a = _grifMinAlpha;
            LeaderLineColor.a = _gridLeadAlpha;
        }

        /// <summary>
        /// Renders the widgets
        /// </summary>
        public void OnRenderObject()
        {
            //If this is the first time through, then create the line material
            if (!_lineMaterial)
                CreateLineMaterial();

            //Set the line material
            GL.PushMatrix();
            _lineMaterial.SetPass(0);
            GL.Begin(GL.LINES);

            //Draw Horizontal lines
            for (int i = 0; i < NumLines + 1; i++)
            {
                GL.Color(i % MinorLineRatio == 0 ? MajorLineColor : MinorLineColor);

                GL.Vertex3(_corner + i * CellSize, _corner, _verticalOffset);
                GL.Vertex3(_corner + i * CellSize, -_corner, _verticalOffset);
            }

            //Draw verticle lines
            for (var j = 0; j < NumLines + 1; j++)
            {
                GL.Color(j % MinorLineRatio == 0 ? MajorLineColor : MinorLineColor);

                GL.Vertex3(_corner, _corner + j * CellSize, _verticalOffset);
                GL.Vertex3(-_corner, _corner + j * CellSize, _verticalOffset);
            }

            // Draw the top square
            GL.Color(MajorLineColor);

            GL.Vertex3(-_corner, _corner, -_verticalOffset);
            GL.Vertex3(_corner, _corner, -_verticalOffset);

            GL.Vertex3(-_corner, -_corner, -_verticalOffset);
            GL.Vertex3(_corner, -_corner, -_verticalOffset);

            GL.Vertex3(_corner, -_corner, -_verticalOffset);
            GL.Vertex3(_corner, _corner, -_verticalOffset);

            GL.Vertex3(-_corner, -_corner, -_verticalOffset);
            GL.Vertex3(-_corner, _corner, -_verticalOffset);


            // Draw the vertical corner lines
            GL.Vertex3(_corner, _corner, -_verticalOffset);
            GL.Vertex3(_corner, _corner, _verticalOffset);

            GL.Vertex3(-_corner, _corner, -_verticalOffset);
            GL.Vertex3(-_corner, _corner, _verticalOffset);

            GL.Vertex3(_corner, -_corner, -_verticalOffset);
            GL.Vertex3(_corner, -_corner, _verticalOffset);

            GL.Vertex3(-_corner, -_corner, -_verticalOffset);
            GL.Vertex3(-_corner, -_corner, _verticalOffset);
            
            // Draw the waypoints
            foreach (var waypoint in Waypoint.Waypoints)
            {
                DrawCircle(waypoint.transform.position, 0.0035f);
            }

            // Draw the ship icons
            foreach (var emiter in Emitter.TheaterEmitters)
            {
                DrawTriangle(emiter.transform.position, 0.01f);

                /*switch(emiter.GridIcon)
                {
                    case GridIcon.Dimond:
                        DrawDimond(emiter.transform.position, 0.01f);
                        break;

                    case GridIcon.Triangle:
                        DrawTriangle(emiter.transform.position, 0.01f);
                        break;

                    case GridIcon.Circle:
                        DrawCircle(emiter.transform.position, 0.01f);
                        break;

                    case GridIcon.Rectangle:
                        DrawRectangle(emiter.transform.position, 0.01f);
                        break;

                    case GridIcon.Box:
                        DrawBox(emiter.transform.position, 0.01f);
                        break;
                }*/
            }

            // Draw the ship icons
/*            foreach (var emiter in TacticalInput.Instance.SelectedObjects)
            {
                var pilot = (AIPilot) emiter.GetComponent(typeof (AIPilot));
                if (pilot == null) 
                    continue;

                GL.Vertex3(emiter.transform.position.x, emiter.transform.position.y, emiter.transform.position.z);
                GL.Vertex3(pilot.TargetPoint.x, pilot.TargetPoint.y, pilot.TargetPoint.z);
            }*/


            //Draw the leader lines
            GL.Color(LeaderLineColor);
            foreach (var pos in Emitter.TheaterEmitters.Select(tgt => tgt.transform.position))
            {
                GL.Vertex3(pos.x, pos.y, pos.z);
                GL.Vertex3(pos.x, pos.y, _verticalOffset);
            }
 
            GL.End();

            GL.PopMatrix();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Creates the line material
        /// </summary>
        static void CreateLineMaterial()
        {
            _lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                                         "SubShader { Pass { " +
                                         "    Blend SrcAlpha OneMinusSrcAlpha " +
                                         "    ZWrite Off Cull Off Fog { Mode Off } " +
                                         "    BindChannels {" +
                                         "      Bind \"vertex\", vertex Bind \"color\", color }" +
                                         "} } }")
                                {
                                    hideFlags = HideFlags.HideAndDontSave,
                                    shader = {hideFlags = HideFlags.HideAndDontSave}
                                };
        }
        #endregion

        #region Draw widgets
        /// <summary>
        /// Renders a dimond widget
        /// <param name="pos">The location to draw</param>
        /// <param name="size">The size to draw</param>
        /// </summary>
        public void DrawDimond(Vector3 pos, float size)
        {
            size *= (CameraManager.Instance.CameraPosition - pos).magnitude;

            var up = CameraManager.Instance.CameraUp;
            var right = CameraManager.Instance.CameraRight * 0.5f;

            var p1 = (up) * size + pos;
            var p2 = (-right) * size + pos;
            var p3 = (-up) * size + pos;
            var p4 = (right) * size + pos;


            GL.Color(MajorLineColor);

            GL.Vertex3(p1.x, p1.y, p1.z);
            GL.Vertex3(p2.x, p2.y, p2.z);

            GL.Vertex3(p2.x, p2.y, p2.z);
            GL.Vertex3(p3.x, p3.y, p3.z);

            GL.Vertex3(p3.x, p3.y, p3.z);
            GL.Vertex3(p4.x, p4.y, p4.z);

            GL.Vertex3(p4.x, p4.y, p4.z);
            GL.Vertex3(p1.x, p1.y, p1.z);

        }

        /// <summary>
        /// Renders a triangle widget
        /// <param name="pos">The location to draw</param>
        /// <param name="size">The size to draw</param>
        /// </summary>
        public void DrawTriangle(Vector3 pos, float size)
        {
            size *= (CameraManager.Instance.CameraPosition - pos).magnitude;

            var up = CameraManager.Instance.CameraUp * 0.5f;
            var right = CameraManager.Instance.CameraRight * 0.5f;

            var p1 = (up) * size + pos;
            var p2 = (-up - right) * size + pos;
            var p3 = (-up + right) * size + pos;

            GL.Color(MajorLineColor);

            GL.Vertex3(p1.x, p1.y, p1.z);
            GL.Vertex3(p2.x, p2.y, p2.z);

            GL.Vertex3(p2.x, p2.y, p2.z);
            GL.Vertex3(p3.x, p3.y, p3.z);

            GL.Vertex3(p3.x, p3.y, p3.z);
            GL.Vertex3(p1.x, p1.y, p1.z);

        }

        /// <summary>
        /// Renders a circle widget
        /// <param name="pos">The location to draw</param>
        /// <param name="size">The size to draw</param>
        /// </summary>
        public void DrawCircle(Vector3 pos, float size)
        {
            const int steps = 10;

            size *= (CameraManager.Instance.CameraPosition - pos).magnitude;

            var up = CameraManager.Instance.CameraUp;
            var right = CameraManager.Instance.CameraRight;

            GL.Color(MajorLineColor);

            for (int i = 0; i < steps; i++)
            {
                var p1 = (up * Mathf.Sin(2*Mathf.PI*i/steps)  + right*Mathf.Cos(2*Mathf.PI*i/steps)) * size + pos;
                var p2 = (up * Mathf.Sin(2*Mathf.PI*(i+1)/steps)  + right*Mathf.Cos(2*Mathf.PI*(i+1)/steps)) * size + pos;

                GL.Vertex3(p1.x, p1.y, p1.z);
                GL.Vertex3(p2.x, p2.y, p2.z);                
            }
        }

        /// <summary>
        /// Renders a box widget
        /// <param name="pos">The location to draw</param>
        /// <param name="size">The size to draw</param>
        /// </summary>
        public void DrawBox(Vector3 pos, float size)
        {
            size *= (CameraManager.Instance.CameraPosition - pos).magnitude;

            var up = CameraManager.Instance.CameraUp;
            var right = CameraManager.Instance.CameraRight;

            var p1 = (up + right) * size + pos;
            var p2 = (up - right) * size + pos;
            var p3 = (-up - right) * size + pos;
            var p4 = (-up + right) * size + pos;

            GL.Color(MajorLineColor);

            GL.Vertex3(p1.x, p1.y, p1.z);
            GL.Vertex3(p2.x, p2.y, p2.z);

            GL.Vertex3(p2.x, p2.y, p2.z);
            GL.Vertex3(p3.x, p3.y, p3.z);

            GL.Vertex3(p3.x, p3.y, p3.z);
            GL.Vertex3(p4.x, p4.y, p4.z);

            GL.Vertex3(p4.x, p4.y, p4.z);
            GL.Vertex3(p1.x, p1.y, p1.z);

        }

        /// <summary>
        /// Renders a rectangle widget
        /// <param name="pos">The location to draw</param>
        /// <param name="size">The size to draw</param>
        /// </summary>
        public void DrawRectangle(Vector3 pos, float size)
        {
            size *= (CameraManager.Instance.CameraPosition - pos).magnitude;

            var up = CameraManager.Instance.CameraUp;
            var right = CameraManager.Instance.CameraRight * 2;

            var p1 = (up + right) * size + pos;
            var p2 = (up - right) * size + pos;
            var p3 = (-up - right) * size + pos;
            var p4 = (-up + right) * size + pos;

            GL.Color(MajorLineColor);

            GL.Vertex3(p1.x, p1.y, p1.z);
            GL.Vertex3(p2.x, p2.y, p2.z);

            GL.Vertex3(p2.x, p2.y, p2.z);
            GL.Vertex3(p3.x, p3.y, p3.z);

            GL.Vertex3(p3.x, p3.y, p3.z);
            GL.Vertex3(p4.x, p4.y, p4.z);

            GL.Vertex3(p4.x, p4.y, p4.z);
            GL.Vertex3(p1.x, p1.y, p1.z);
        }
        #endregion
    }
}