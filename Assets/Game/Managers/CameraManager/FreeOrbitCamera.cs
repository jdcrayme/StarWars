using UnityEngine;

namespace Assets.Game.Managers.CameraManager
{
    public class FreeOrbitCamera : MonoBehaviour
    {
        private const int ROTATE_MOUSE_BTN = 1;

        #region Public Fields
        public float Distance = 15f;        //The distance of the camera from the target
        public float MinDistance = 15;      //Min distance of the camera from the target
        public float MaxDistance = 20000;   //Max distance of the camera from the target

        public float RollSpeed = 10;        //The speed at which the camera rolls when dragged
        public float PitchSpeed = 10;       //The speed at which the camera pitches when draggded
        public float YawSpeed = 10;         //The speed at which the camera yaws when dragged

        public float PanSpeed = 0.01f;      //The speed at which the camera pans proportional to 'Distance'

        public float EdgeYaw = 0.5f;        //The speed at which the camera yaws when the cursor is on the edge
        public float EdgePitch = 0.5f;      //The speed at which the camera pitches when the cursor is on the edge
        
        public GameObject Target;           //The camera refrence object 
        #endregion

        #region Private fields
        private Vector3 _targetPosition;                                //The camera refrence point
        private Vector3? _lastMousePosition;                            //The last mouse position
        private bool _leftClick, _rightClick, _topClick, _bottomClick;  //tracks if the user clicked on an edge before moving the mouse arround
        #endregion

        /// <summary>
        /// Standard Initialization
        /// </summary> 
        public void Start()
        {
        }

        /// <summary>
        /// Update 
        /// </summary>
        public void LateUpdate()
        {
            var mouseBtn = Input.GetMouseButton(ROTATE_MOUSE_BTN);

            var mousePosn = Input.mousePosition;
            mousePosn.x = 2*(mousePosn.x/Screen.width)-1;
            mousePosn.y = 2*(mousePosn.y/Screen.height)-1;

            #region Log where the user clicks
            if (Input.GetMouseButtonDown(ROTATE_MOUSE_BTN))
            {
                _leftClick = _rightClick = _topClick = _bottomClick = false;

                _leftClick = mousePosn.x < -0.75f;
                _rightClick = mousePosn.x >  0.75f;
                _topClick = mousePosn.y < -0.75f;
                _bottomClick = mousePosn.y > 0.75f;
            }
            #endregion

            #region Zoom with the mouse
            Distance += Input.GetAxis("Mouse ScrollWheel")*Distance*-0.1f;
            Distance += Input.GetKey(KeyCode.KeypadMinus) ? 0.1f * Distance : 0;
            Distance -= Input.GetKey(KeyCode.KeypadPlus) ? 0.1f * Distance : 0;
            Distance = Mathf.Clamp(Distance, MinDistance, MaxDistance);
            #endregion

            #region Handle mouse dragging
            //TODO should probably check if we are over a control here
            if (mouseBtn)
            {
                if (_lastMousePosition.HasValue)
                {
                    float deltaV;
                    float deltaH;

                    //If we drag into any of the edges then auto spin at a speed proportional to how close we are to the edge
                    if (!_leftClick && mousePosn.x < -0.75f)
                    {
                        deltaH = (mousePosn.x + 0.75f)*EdgeYaw;
                    }
                    else if (!_rightClick && mousePosn.x > 0.75f)
                    {
                        deltaH = (mousePosn.x - 0.75f) * EdgeYaw;
                    }
                    else
                    {
                        deltaH = (mousePosn.x - _lastMousePosition.Value.x);
                    }

                    if (!_topClick && mousePosn.y < -0.75f)
                    {
                        deltaV = (mousePosn.y + 0.75f)*EdgePitch;
                    } 
                    else if (!_bottomClick && mousePosn.y > 0.75f)
                    {
                        deltaV = (mousePosn.y - 0.75f) * EdgePitch;
                    }
                    else
                    {
                        deltaV = (mousePosn.y - _lastMousePosition.Value.y);
                    }

                    //If the user clicks on the right side and stays there then we roll the camera
                    if (_rightClick && mousePosn.x > 0.75f)
                    {
                        transform.rotation *= Quaternion.Euler(0, 0, deltaV * -RollSpeed);
                    }
                    else
                    {
                        transform.rotation *= Quaternion.Euler(-deltaV * PitchSpeed, -deltaH * YawSpeed, 0);
                    }
                    _lastMousePosition = mousePosn;
                }
                else
                {
                    _lastMousePosition = mousePosn;
                }
            }
            else
            {
                _lastMousePosition = null;
            }
            #endregion

            #region Handle Panning
            if (Input.GetButton("MapPanLeft"))
            {
                Target = null;
                _targetPosition += transform.rotation * Vector3.left * Distance * PanSpeed;
            }

            if (Input.GetButton("MapPanRight"))
            {
                Target = null;
                _targetPosition -= transform.rotation * Vector3.left * Distance * PanSpeed;
            }

            if (Input.GetButton("MapPanForward"))
            {
                Target = null;
                _targetPosition += transform.rotation * Vector3.forward * Distance * PanSpeed;
            }

            if (Input.GetButton("MapPanBack"))
            {
                Target = null;
                _targetPosition -= transform.rotation * Vector3.forward * Distance * PanSpeed;
            }

            if (Input.GetButton("MapPanUp"))
            {
                Target = null;
                _targetPosition += transform.rotation * Vector3.up * Distance * PanSpeed;
            }

            if (Input.GetButton("MapPanDown"))
            {
                Target = null;
                _targetPosition -= transform.rotation * Vector3.up * Distance * PanSpeed;
            }
            #endregion

            if (Target!=null)
                _targetPosition = Target.transform.position;
            
            transform.position = transform.rotation*(Vector3.forward*-Distance) + _targetPosition;
        }
    }
}