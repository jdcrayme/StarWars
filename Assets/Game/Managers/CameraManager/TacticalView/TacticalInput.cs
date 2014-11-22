using System.Collections.Generic;
using System.Linq;
using Assets.Game.Ships.Scripts;
using Assets.Ships.Scripts.AI;
using UnityEngine;

namespace Assets.Game.Managers.CameraManager.TacticalView
{
	public class TacticalInput : MonoBehaviour
	{
		enum State { Normal, Azmuth, Height }

		#region Public Fields
		public static TacticalInput Instance;
		public List<Emitter> SelectedObjects = new List<Emitter>();

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
		
		public Vector3 CompassOrigen = new Vector3(0, 0, 0);
		public Vector3 CompassTarget = new Vector3(50, 50, 50);
		#endregion

		#region Private Fields
		private static Material _lineMaterial;
		private Vector3 _delta;
		private float _size;

		private State _state = State.Normal;
		private Vector3 _selectionStart;

		#endregion

		public Vector3 SelectedObjectsCenterOfMass
		{
			get
			{
				if (SelectedObjects.Count < 1) 
					return Vector3.zero;

				var pos = SelectedObjects.Aggregate(Vector3.zero, (current, selectedObject) => current + selectedObject.transform.position);
				pos /= SelectedObjects.Count;
				return pos;
			}
		}

		/// <summary>
		/// Standard Initialization
		/// </summary> 
		public void Start ()
		{
			Instance = this;
		}

		/// <summary>
		/// Update 
		/// </summary>
		public void Update()
		{
            //If the user holds the shift key then draw the movement compass
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                _state = State.Azmuth;
            }else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                _state = State.Normal;
            }

		    //On mouse down
			if (Input.GetMouseButtonDown(0) && GUIUtility.hotControl == 0)
			{
			    switch (_state)
			    {
                    case State.Normal:
                        _selectionStart = Input.mousePosition;
			            break;
			    }
			}

            //On mouse up
            if (Input.GetMouseButtonUp(0))
            {
                switch (_state)
                {
                    case State.Height:
                        {
                            //If we were in height mode, then move the selected objects to the new point
                            _state = State.Normal;
                            var order = new AIPilot.Order();
                            var target = new GameObject("MoveTarget");
                            target.transform.position = CompassTarget;
                            order.Targets = new List<GameObject> {target};
                            order.OrderAction = AIPilot.Order.Action.Move;

                            foreach (
                                var pilot in
                                    SelectedObjects.Select(obj => (AIPilot) obj.GetComponent(typeof (AIPilot))).Where(
                                        pilot => pilot != null))
                            {
                                pilot.Orders.Clear();
                                pilot.Orders.Add(order);
                            }
                        }
                        break;
                    case State.Azmuth:
                        _state = State.Height;
                        break;
                    case State.Normal:
                        SelectedObjects.Clear();

                        foreach (var emmiter in from emmiter in Emitter.TheaterEmitters
                                                let minX = Mathf.Min(_selectionStart.x, Input.mousePosition.x)
                                                let maxX = Mathf.Max(_selectionStart.x, Input.mousePosition.x)
                                                let minY = Mathf.Min(_selectionStart.y, Input.mousePosition.y)
                                                let maxY = Mathf.Max(_selectionStart.y, Input.mousePosition.y)
                                                let screenPos = Game.Managers.CameraManager.CameraManager.Instance.CameraWorldToScreenPoint(emmiter.transform.position)
                                                where screenPos.x > minX && screenPos.x < maxX && screenPos.y > minY && screenPos.y < maxY select emmiter)
                        {
                            SelectedObjects.Add(emmiter);
                        }

                        _selectionStart = Vector2.zero;
                        break;
                }
            }
            
			switch (_state)
			{
			    case State.Azmuth:
			        {
			            //Draw movment compass
			            float enter;
			            CompassOrigen = SelectedObjectsCenterOfMass;
			            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			            var plane = new Plane(new Vector3(0f, 0f, 1f), new Vector3(CompassOrigen.x, CompassOrigen.y, CompassOrigen.z));
			            plane.Raycast(ray, out enter);
			            CompassTarget = ray.GetPoint(enter);
			        }
			        break;
			    case State.Height:
			        {
			            // Draw the movement compass
			            float enter;
			            CompassOrigen = SelectedObjectsCenterOfMass;
			            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			            var plane = new Plane(Vector3.Cross(new Vector3(0f, 0f, 1f), CompassTarget - CompassOrigen).normalized, CompassOrigen);
			            plane.Raycast(ray, out enter);
			            CompassTarget.z = ray.GetPoint(enter).z;
			        }
			        break;
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
					shader = { hideFlags = HideFlags.HideAndDontSave }
				};
			}
		}

		public void OnRenderObject()
		{
			if (Camera.current == null || Camera.current != DrawCamera)
				return;

			_delta = CompassTarget - CompassOrigen;
			_size = Mathf.Sqrt(_delta.x * _delta.x + _delta.y * _delta.y);

			CreateLineMaterial();
			// set the current material
			GL.PushMatrix();
			_lineMaterial.SetPass(0);

			//Draw the move compass
			DrawMovementCompass();

			GL.PopMatrix();

			//Draw the selection box in screen space
			DrawSelectionBox();

		}

		private void DrawMovementCompass()
		{
			GL.Begin(GL.TRIANGLES);
			GL.Color(BackgroundColor);

			//Draw the shaded disk
			if(_state==State.Azmuth||_state==State.Height)
				for (var i = 0; i < 360; i += CircleTesselation)
				{
					GL.Vertex3(CompassOrigen.x + Mathf.Sin(i * Mathf.Deg2Rad) * _size, CompassOrigen.y + Mathf.Cos(i * Mathf.Deg2Rad) * _size, CompassOrigen.z);
					GL.Vertex3(CompassOrigen.x + Mathf.Sin((i + CircleTesselation) * Mathf.Deg2Rad) * _size, CompassOrigen.y + Mathf.Cos((i + CircleTesselation) * Mathf.Deg2Rad) * _size, CompassOrigen.z);
					GL.Vertex3(CompassOrigen.x, CompassOrigen.y, CompassOrigen.z);
				}

			//Draw the shaded elevation triangle
			if (_state == State.Height)
			{
				GL.Vertex3(CompassTarget.x, CompassTarget.y, CompassTarget.z);
				GL.Vertex3(CompassTarget.x, CompassTarget.y, CompassOrigen.z);
				GL.Vertex3(CompassOrigen.x, CompassOrigen.y, CompassOrigen.z);
			}

			GL.End();

			GL.Begin(GL.LINES);
			GL.Color(MajorLineColor);

			//Draw the oulines
			if (_state == State.Azmuth || _state == State.Height)
			{
				//Draw the lines arround the disk
				for (var i = 0; i < 360; i += CircleTesselation)
				{
					GL.Vertex3(CompassOrigen.x + Mathf.Sin(i*Mathf.Deg2Rad)*_size,
							   CompassOrigen.y + Mathf.Cos(i*Mathf.Deg2Rad)*_size, CompassOrigen.z);
					GL.Vertex3(CompassOrigen.x + Mathf.Sin((i + CircleTesselation)*Mathf.Deg2Rad)*_size,
							   CompassOrigen.y + Mathf.Cos((i + CircleTesselation)*Mathf.Deg2Rad)*_size, CompassOrigen.z);
				}

				//Draw the major tick marks
				for (var i = 0; i < 360; i += MajorLineStepSize)
				{
					GL.Vertex3(CompassOrigen.x + Mathf.Sin(i*Mathf.Deg2Rad)*(_size - MajorLineTickSize),
							   CompassOrigen.y + Mathf.Cos(i*Mathf.Deg2Rad)*(_size - MajorLineTickSize), CompassOrigen.z);
					GL.Vertex3(CompassOrigen.x + Mathf.Sin(i*Mathf.Deg2Rad)*(_size + MajorLineTickSize),
							   CompassOrigen.y + Mathf.Cos(i*Mathf.Deg2Rad)*(_size + MajorLineTickSize), CompassOrigen.z);
				}

				//Draw the minor tick marks
				for (var i = 0; i < 360; i += MinorLineStepSize)
				{
					GL.Vertex3(CompassOrigen.x + Mathf.Sin(i*Mathf.Deg2Rad)*(_size),
							   CompassOrigen.y + Mathf.Cos(i*Mathf.Deg2Rad)*(_size), CompassOrigen.z);
					GL.Vertex3(CompassOrigen.x + Mathf.Sin(i*Mathf.Deg2Rad)*(_size + MinorLineTickSize),
							   CompassOrigen.y + Mathf.Cos(i*Mathf.Deg2Rad)*(_size + MinorLineTickSize), CompassOrigen.z);
				}
			}

			//Draw the elevation triangle outline
			if (_state == State.Height)
			{

				GL.Vertex3(CompassTarget.x, CompassTarget.y, CompassTarget.z);
				GL.Vertex3(CompassTarget.x, CompassTarget.y, CompassOrigen.z);
				GL.Vertex3(CompassTarget.x, CompassTarget.y, CompassOrigen.z);
				GL.Vertex3(CompassOrigen.x, CompassOrigen.y, CompassOrigen.z);
				GL.Vertex3(CompassOrigen.x, CompassOrigen.y, CompassOrigen.z);
				GL.Vertex3(CompassTarget.x, CompassTarget.y, CompassTarget.z);
			}
			GL.End();
		}

		private void DrawSelectionBox()
		{
            //Draw the oulines
		    if (_state != State.Normal || _selectionStart == Vector3.zero) 
                return;

		    GL.PushMatrix();
		    GL.LoadOrtho();
		    GL.Begin(GL.LINES);
		    GL.Color(MajorLineColor);

		    const int depth = 0;
		    var topLeft = new Vector2(_selectionStart.x/Screen.width, _selectionStart.y/Screen.height);
		    var bottomRight = new Vector2(Input.mousePosition.x/Screen.width, Input.mousePosition.y/Screen.height);

		    GL.Vertex3(topLeft.x, topLeft.y, depth);
		    GL.Vertex3(topLeft.x, bottomRight.y, depth);
		    GL.Vertex3(topLeft.x, bottomRight.y, depth);
		    GL.Vertex3(bottomRight.x, bottomRight.y, depth);
		    GL.Vertex3(bottomRight.x, bottomRight.y, depth);
		    GL.Vertex3(bottomRight.x, topLeft.y, depth);
		    GL.Vertex3(bottomRight.x, topLeft.y, depth);
		    GL.Vertex3(topLeft.x, topLeft.y, depth);

		    GL.End();
		    GL.PopMatrix();
		}
        
		public static Vector3 ToPolar(Vector3 input)
		{
			var rad = Mathf.Sqrt(input.x * input.x + input.y * input.y + input.z * input.z);
			var inc = Mathf.Acos(input.z / rad) * Mathf.Rad2Deg;
			var az = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;
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
