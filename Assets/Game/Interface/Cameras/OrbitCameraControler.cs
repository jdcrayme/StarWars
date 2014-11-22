using System;
using UnityEngine;

namespace Assets.Game.Interface.Cameras
{
    [Serializable]
    public class OrbitCameraControler : ICameraControler
    {
        public GameObject Target;

        public float PanForwardRate;
        public float PanLateralRate;
        public float PanVerticleRate;

        public float RollRate;
        public float PitchRate;
        public float YawRate;

        private Quaternion _rotation = Quaternion.identity;
        private float _orbitDist = 10;

        /// <summary>
        /// Returns a data string describing what this camera is doing
        /// </summary>
        public string DataString { get { return "Orbit Cam " + (Target == null ? "" : ("Target: " + Target.name)); } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="panForwardRate">The rate at which we move forward and back in units/sec </param>
        /// <param name="panLateralRate">The rate at which we move to the side in units/sec</param>
        /// <param name="panVerticleRate">The rate at which we move up and down in units/sec</param>
        /// <param name="rollRate">The roll rate in degrees/sec</param>
        /// <param name="pitchRate">The pitch rate in degrees/sec</param>
        /// <param name="yawRate">The yaw rate in degrees/sec</param>
        public OrbitCameraControler(float panForwardRate, float panLateralRate, float panVerticleRate, float rollRate, float pitchRate, float yawRate)
        {
            PanForwardRate = panForwardRate;
            PanLateralRate = panLateralRate;
            PanVerticleRate = panVerticleRate;

            RollRate = rollRate;
            PitchRate = pitchRate;
            YawRate = yawRate;
        }

        /// <summary>
        /// Updates a camera's position
        /// </summary>
        /// <param name="cam">The camera to update</param>
        public void Update(Camera cam)
        {
            if(Target==null)
                return;

            var dt = Time.deltaTime;

            _rotation = Quaternion.AngleAxis(RollRate * dt * Input.GetAxis("CameraRollLeft"), cam.transform.forward) * _rotation;
            _rotation = Quaternion.AngleAxis(PitchRate * dt * Input.GetAxis("CameraPitchUp"), cam.transform.right) * _rotation;
            _rotation = Quaternion.AngleAxis(YawRate * dt * Input.GetAxis("CameraYawLeft"), cam.transform.up) * _rotation;

            _orbitDist += PanForwardRate * dt * Input.GetAxis("CameraPanForward");

            _orbitDist = _orbitDist < 1 ? 1 : _orbitDist; // don't get any closer than 1 unit

            cam.transform.rotation = _rotation;
            cam.transform.position = Target.transform.position-cam.transform.forward*_orbitDist;
        }
    }
}
