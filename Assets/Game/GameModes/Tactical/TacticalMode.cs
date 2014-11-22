using Assets.Game.Interface.Cameras;
using Assets.Game.Managers.CameraManager;
using Assets.Game.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.GameModes.Tactical
{
    public class TacticalMode : GameMode {

        public float PanForwardRate = 10;
        public float PanLateralRate = 10;
        public float PanVerticleRate = 10;

        public float RollRate = 100;
        public float PitchRate = 100;
        public float YawRate = 100;

        public Texture2D FighterIcon;
        public Texture2D CorvetteIcon;

        private Camera _cam;

        private OrbitCameraControler _orbitCamera;
        private int _targetIndex;

        private const int ButtonWidth = 25;

        public void Start () {
            _orbitCamera = new OrbitCameraControler(
            PanForwardRate,
            PanLateralRate,
            PanVerticleRate,
            RollRate,
            PitchRate,
            YawRate);
        }

        public void Update () {
            _orbitCamera.Update(_cam);

            if (Input.GetButtonDown("CycleViewTarget"))
            {
                //Cycle to the next unit

                _targetIndex = _targetIndex >= SpaceUnit.Units.Count - 1 ? 0 : _targetIndex + 1;
                _orbitCamera.Target = SpaceUnit.Units[_targetIndex].gameObject;
            }
        }

        public void OnGUI()
        {
            GUI.Box(new Rect(0, 0, Screen.width, 20), "" + _orbitCamera.DataString);

            foreach (var unit in SpaceUnit.Units)
            {
                var pos = CameraManager.Instance.CameraWorldToScreenPoint(unit.transform.position);

                GUI.Button(new Rect(pos.x - ButtonWidth / 2, pos.y - ButtonWidth / 2, ButtonWidth, ButtonWidth), FighterIcon);
            }

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
