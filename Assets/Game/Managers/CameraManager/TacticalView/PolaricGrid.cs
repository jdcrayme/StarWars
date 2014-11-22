using System.Collections.Generic;
using System.Linq;
using Assets.Game.Ships.Scripts;
using Assets.Ships.Scripts.AI;
using UnityEngine;

/**
 * Simple example of creating a procedural 6 sided cube
 */
namespace Assets.Game.Managers.CameraManager.TacticalView
{
    //[RequireComponent(typeof(MeshFilter))]
    //[RequireComponent(typeof(MeshRenderer))]
    public class PolaricGrid : MonoBehaviour
    {
        public Camera DrawCamera;

        public Color MajorLineColor = new Color(1.0f, 1.0f, 1.0f, 0.75f);
        public Color BackgroundColor = new Color(0.5f, 0.5f, 0.5f, 0.75f);

        public int MajorLineStepSize = 10;
        public int MajorLineTickSize = 1;

        public int MinorLineStepSize = 5;
        public int MinorLineTickSize = 1;

        public int TextStepSize = 30;
        public float TextSize = 50;

        public int CircleTesselation = 3;


        public Vector3 Position = new Vector3(0, 0, 0);
        public Vector3 Target = new Vector3(50, 50, 50);

       
        private static Material _lineMaterial;
        private Vector3 _delta;
        private float _size;

        internal bool height=false;
        //private AIPilot pilot;

        public void Start()
        {
        }
        
        public void setTarget(List<Emitter> selection)
        {
            enabled = true;
            height = false;
            //pilot = (AIPilot)selection.GetComponent(typeof(AIPilot));
            //Target = selection.transform.position;
            
            //pilot.ClearOrders();

        }

        public void Clear()
        {
            enabled = false;
        }
        
        public void Update()
        {
            if (height)
            {
                float enter;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var plane = new Plane(Vector3.Cross(new Vector3(0f, 0f, 1f), Target - Position).normalized, Position);
                plane.Raycast(ray, out enter);
                Target.z = ray.GetPoint(enter).z;
            }
            else
            {
                float enter;
                float z = Target.z;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                var plane = new Plane(new Vector3(0f, 0f, 1f), new Vector3(Position.x, Position.y, Position.z));
                plane.Raycast(ray, out enter);
                Target = ray.GetPoint(enter);

                //Target.z = z;
            }

            if (Input.GetMouseButtonDown(0))
                if (height)
                {
                    height = false;
                    var order = new AIPilot.Order();
                    var target = new GameObject("MoveTarget");
                    target.transform.position = Target;
                    order.Targets = new List<GameObject> { target };
                    order.OrderAction = AIPilot.Order.Action.Move;

                    foreach (var pilot in TacticalInput.Instance.SelectedObjects.Select(obj => (AIPilot) obj.GetComponent(typeof (AIPilot))).Where(pilot => pilot != null))
                    {
                        pilot.Orders.Clear();
                        pilot.Orders.Add(order);
                    }

                }
                else
                {
                    height = true;
                }
        }

        static void CreateLineMaterial()
        {
            if (!_lineMaterial)
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
        }

        public void OnRenderObject()
        {
            if (Camera.current == null || Camera.current != DrawCamera)
                return;

            _delta = Target - Position;
            _size = Mathf.Sqrt(_delta.x*_delta.x + _delta.y*_delta.y);

            CreateLineMaterial();
            // set the current material
            GL.PushMatrix();
            _lineMaterial.SetPass(0);

            GL.Begin(GL.TRIANGLES);
            GL.Color(BackgroundColor);

            //Draw the shaded disk
            for (int i = 0; i < 360; i += CircleTesselation)
            {
                GL.Vertex3(Position.x + Mathf.Sin(i * Mathf.Deg2Rad) * _size, Position.y + Mathf.Cos(i * Mathf.Deg2Rad) * _size, Position.z);
                GL.Vertex3(Position.x + Mathf.Sin((i + CircleTesselation) * Mathf.Deg2Rad) * _size, Position.y + Mathf.Cos((i + CircleTesselation) * Mathf.Deg2Rad) * _size, Position.z);
                GL.Vertex3(Position.x,Position.y,Position.z);
            }

            //Draw the shaded elevation triangle
            GL.Vertex3(Target.x, Target.y, Target.z);
            GL.Vertex3(Target.x, Target.y, Position.z);
            GL.Vertex3(Position.x, Position.y, Position.z);

            GL.End();

            GL.Begin(GL.LINES);
            GL.Color(MajorLineColor);

            //Draw the lines arround the disk
            for (int i = 0; i < 360; i += CircleTesselation)
            {
                GL.Vertex3(Position.x + Mathf.Sin(i * Mathf.Deg2Rad) * _size, Position.y + Mathf.Cos(i * Mathf.Deg2Rad) * _size, Position.z);
                GL.Vertex3(Position.x + Mathf.Sin((i + CircleTesselation) * Mathf.Deg2Rad) * _size, Position.y + Mathf.Cos((i + CircleTesselation) * Mathf.Deg2Rad) * _size, Position.z);
            }

            //Draw the major tick marks
            for (int i = 0; i < 360; i += MajorLineStepSize)
            {
                GL.Vertex3(Position.x + Mathf.Sin(i * Mathf.Deg2Rad) * (_size - MajorLineTickSize), Position.y + Mathf.Cos(i * Mathf.Deg2Rad) * (_size - MajorLineTickSize), Position.z);
                GL.Vertex3(Position.x + Mathf.Sin(i * Mathf.Deg2Rad) * (_size + MajorLineTickSize), Position.y + Mathf.Cos(i * Mathf.Deg2Rad) * (_size + MajorLineTickSize), Position.z);
            }

            //Draw the minor tick marks
            for (int i = 0; i < 360; i += MinorLineStepSize)
            {
                GL.Vertex3(Position.x + Mathf.Sin(i * Mathf.Deg2Rad) * (_size), Position.y + Mathf.Cos(i * Mathf.Deg2Rad) * (_size), Position.z);
                GL.Vertex3(Position.x + Mathf.Sin(i * Mathf.Deg2Rad) * (_size + MinorLineTickSize), Position.y + Mathf.Cos(i * Mathf.Deg2Rad) * (_size + MinorLineTickSize), Position.z);
            }

            //Draw the elevation triangle outline
            GL.Vertex3(Target.x, Target.y, Target.z);
            GL.Vertex3(Target.x, Target.y, Position.z);
            GL.Vertex3(Target.x, Target.y, Position.z);
            GL.Vertex3(Position.x, Position.y, Position.z);
            GL.Vertex3(Position.x, Position.y, Position.z);
            GL.Vertex3(Target.x, Target.y, Target.z);

            GL.End();
            GL.PopMatrix();
        }
        /*
        public void OnGUI()
        {
            if (Camera.current == null || Camera.current.tag != "MainCamera")
                return;

            var textHalfSize = TextSize/2;

            if (DrawCamera == null)
                return;

            //var centeredStyle = GUI.skin.GetStyle("Label");
            //centeredStyle.alignment = TextAnchor.MiddleCenter;

            for (int i = 0; i < 360; i += TextStepSize)
            {
                var pos = new Vector3(Position.x + Mathf.Cos(i * Mathf.Deg2Rad) * (_size + MajorLineTickSize * 4), Position.y + Mathf.Sin(i * Mathf.Deg2Rad) * (_size + MajorLineTickSize * 4), Position.z);
                var scr = DrawCamera.WorldToScreenPoint(pos);

                GUI.Label(new Rect(scr.x - textHalfSize, Screen.height - scr.y - textHalfSize, TextSize, TextSize),  string.Format("{0:000}", i), centeredStyle);
            }

            var pol = ToPolar(_delta);

            var tgt = DrawCamera.WorldToScreenPoint(Target);
            GUI.Label(new Rect(tgt.x - textHalfSize, Screen.height - tgt.y, TextSize * 3, TextSize), string.Format("{0:000}", pol.x) + " " + string.Format("{0:000}", pol.y) + " " + string.Format("{0:000}", pol.z), centeredStyle);
        }*/

        public static Vector3 ToPolar(Vector3 input)
        {
            float rad = Mathf.Sqrt(input.x * input.x + input.y * input.y + input.z * input.z);
            float inc = Mathf.Acos(input.z/rad)*Mathf.Rad2Deg;
            float az = Mathf.Atan2(input.y,input.x)*Mathf.Rad2Deg;
            az += az < 0 ? 360 : 0;
            return new Vector3(rad, inc, az);
        }

        public static Vector3 ToCartesian(Vector3 input)
        {
            return new Vector3
                             {
                                 x = input.z * Mathf.Sin(input.x * Mathf.Deg2Rad) * Mathf.Cos(input.y * Mathf.Deg2Rad),
                                 y = input.z * Mathf.Sin(input.x * Mathf.Deg2Rad) * Mathf.Sin(input.y * Mathf.Deg2Rad),
                                 z = input.z * Mathf.Cos(input.x * Mathf.Deg2Rad)
                             };
        }
    }
}