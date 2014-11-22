using Assets.Game.Ships.Scripts;
using Assets.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.Interface
{
/*    public class PilotHud : MonoBehaviour {

        public UILabel SpeedWidget;
        private static Material _lineMaterial;      //The material that we use to draw the lines

        // Use this for initialization
        public void Start () {
	
        }
	
        // Update is called once per frame
        public void Update ()
        {
            if (Managers.CameraManager.CameraManager.Instance.CameraTarget != null)
            {
                var inertialDrive =
                    (InertialDrive)
                    Managers.CameraManager.CameraManager.Instance.CameraTarget.GetComponent(typeof (InertialDrive));
                if (inertialDrive != null)
                {
                    if (SpeedWidget != null)
                        SpeedWidget.text = "" + (int) inertialDrive.Speed + " m/s";
                }
            }
        }

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
                shader = { hideFlags = HideFlags.HideAndDontSave }
            };
        }
        #endregion

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

            foreach (var wpts in Waypoint.Waypoints)
            {
                DrawRectangle(wpts.transform.position, 0.02f);
            }

            foreach (var emtr in Emitter.TheaterEmitters)
            {
                DrawRectangle(emtr.transform.position, 0.02f);
            }
            DrawRectangle(new Vector3(0, 0, -1), 0.02f);


            GL.End();

            GL.PopMatrix();
        }

        /// <summary>
        /// Renders a rectangle widget
        /// <param name="pos">The location to draw</param>
        /// <param name="size">The size to draw</param>
        /// </summary>
        public void DrawRectangle(Vector3 pos, float size)
        {
            size *= (Managers.CameraManager.CameraManager.Instance.PilotCamera.transform.position - pos).magnitude;

            var up = Managers.CameraManager.CameraManager.Instance.PilotCamera.transform.up;
            var right = Managers.CameraManager.CameraManager.Instance.PilotCamera.transform.right * 2;

            var p1 = (up + right) * size + pos;
            var p2 = (up - right) * size + pos;
            var p3 = (-up - right) * size + pos;
            var p4 = (-up + right) * size + pos;

            GL.Color(Color.white);

            GL.Vertex3(p1.x, p1.y, p1.z);
            GL.Vertex3(p2.x, p2.y, p2.z);

            GL.Vertex3(p2.x, p2.y, p2.z);
            GL.Vertex3(p3.x, p3.y, p3.z);

            GL.Vertex3(p3.x, p3.y, p3.z);
            GL.Vertex3(p4.x, p4.y, p4.z);

            GL.Vertex3(p4.x, p4.y, p4.z);
            GL.Vertex3(p1.x, p1.y, p1.z);
        }
    }*/
}