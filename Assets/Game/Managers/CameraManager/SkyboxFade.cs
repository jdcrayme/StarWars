using Assets.Game.Managers.CameraManager.TacticalView;
using UnityEngine;

namespace Assets.Game.Managers.CameraManager
{
    public class SkyboxFade : MonoBehaviour
    {
        public float GridStartFadeIn = 1000;
        public float GridEndFadeIn = 1500;

        public Material Background;
        public FreeOrbitCamera OrbitCam;

        private CarteasianGrid _grid;
        private float _gridMajAlpha;
        private float _grifMinAlpha;
        private float _gridLeadAlpha;
        private float _alpha;
        private Component[] _cameraParticles;


        // Use this for initialization
        public void Start()
        {
            _grid = (CarteasianGrid) GetComponent(typeof (CarteasianGrid));
            //_orbitCam = (FreeOrbitCamera) GetComponent(typeof (FreeOrbitCamera));

            _cameraParticles = GetComponentsInChildren(typeof(SU_SpaceParticles));

            _gridMajAlpha = _grid.MajorLineColor.a;
            _grifMinAlpha = _grid.MinorLineColor.a;
            _gridLeadAlpha = _grid.LeaderLineColor.a;
        }

        // Update is called once per frame
        public void Update ()
        {

            //Fade the certesian grid
            _alpha = Mathf.Clamp((OrbitCam.Distance - GridStartFadeIn)/(GridEndFadeIn - GridStartFadeIn), 0, 1);
            _grid.MajorLineColor.a = _gridMajAlpha*_alpha;
            _grid.MinorLineColor.a = _grifMinAlpha*_alpha;
            _grid.LeaderLineColor.a = _gridLeadAlpha*_alpha;

            foreach (var system in _cameraParticles)
            {
                //(system as SU_SpaceParticles).alphaMultiplier = (1 - _alpha);
            }

            Background.SetFloat("_Fade", (1 - _alpha));
        }
    }
}
