using Assets.Game.Interface.Cameras;
using Assets.Game.Managers.CameraManager;
using Assets.Game.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.GameModes.Observer
{
    public class ObserverMode : GameMode
    {

        public float PanForwardRate = 10;
        public float PanLateralRate = 10;
        public float PanVerticleRate = 10;

        public float RollRate = 100;
        public float PitchRate = 100;
        public float YawRate = 100;

        private Camera _cam;

        private FreeCameraControler _freeCamera;
        private FollowCameraControler _followCamera;
        private OrbitCameraControler _orbitCamera;

        private ICameraControler [] _cameraControlers;
        private int _controlerIndex;

        private int _targetIndex;

        // Use this for initialization
        public void Start ()
        {
            _freeCamera = new FreeCameraControler(
                PanForwardRate,
                PanLateralRate,
                PanVerticleRate,
                RollRate,
                PitchRate,
                YawRate);

            _followCamera = new FollowCameraControler(50, 0.1f, 0.1f);

            _orbitCamera = new OrbitCameraControler(
                PanForwardRate,
                PanLateralRate,
                PanVerticleRate,
                RollRate,
                PitchRate,
                YawRate);
            
            _cameraControlers = new ICameraControler[]
            {
                _freeCamera,
                _followCamera,
                _orbitCamera
            };
        }
	
        // Update is called once per frame
        public void Update ()
        {
            _cameraControlers[_controlerIndex].Update(_cam);
            if (Input.GetButtonDown("CycleViews"))
            {
                //Cycle to the next controler
                _controlerIndex = _controlerIndex >= _cameraControlers.Length - 1 ? 0 : _controlerIndex + 1;
            }

            if (Input.GetButtonDown("CycleViewTarget"))
            {
                //Cycle to the next target
                _targetIndex = _targetIndex >= SpaceUnit.Units.Count - 1 ? 0 : _targetIndex + 1;
                _followCamera.Target = _orbitCamera.Target = SpaceUnit.Units[_targetIndex].gameObject;
            }

        }

        public void OnGUI()
        {
            GUI.Box(new Rect(0, 0, Screen.width, 20), "" + _cameraControlers[_controlerIndex].DataString);
        }

        public override void Initialize()
        {
            _cam = GetComponentInChildren<Camera>();
            CameraManager.Instance.SetPrimaryCamera(_cam);
        }

        public override void Shutdown()
        {
        }
    }
}
