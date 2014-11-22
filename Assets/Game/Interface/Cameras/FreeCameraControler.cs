using System;
using UnityEngine;

namespace Assets.Game.Interface.Cameras
{
    [Serializable]
    public class FreeCameraControler : ICameraControler
    {
        public float PanForwardRate;
        public float PanLateralRate;
        public float PanVerticleRate;

        public float RollRate;
        public float PitchRate;
        public float YawRate;

        /// <summary>
        /// Returns a data string describing what this camera is doing
        /// </summary>
        public string DataString { get { return "Free Cam "; } }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="panForwardRate">The rate at which we move forward and back in units/sec </param>
        /// <param name="panLateralRate">The rate at which we move to the side in units/sec</param>
        /// <param name="panVerticleRate">The rate at which we move up and down in units/sec</param>
        /// <param name="rollRate">The roll rate in degrees/sec</param>
        /// <param name="pitchRate">The pitch rate in degrees/sec</param>
        /// <param name="yawRate">The yaw rate in degrees/sec</param>
        public FreeCameraControler(float panForwardRate, float panLateralRate, float panVerticleRate, float rollRate, float pitchRate, float yawRate)
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
            var dt = Time.deltaTime;
            var cameraRotation = cam.transform.rotation;
            var cameraPosition = cam.transform.position;

            cameraRotation = Quaternion.AngleAxis(RollRate*dt*Input.GetAxis("CameraRollLeft"), cam.transform.forward) * cameraRotation;
            cameraRotation = Quaternion.AngleAxis(PitchRate * dt * Input.GetAxis("CameraPitchUp"), cam.transform.right) * cameraRotation;
            cameraRotation = Quaternion.AngleAxis(YawRate * dt * Input.GetAxis("CameraYawLeft"), cam.transform.up) * cameraRotation;

            cameraPosition += cam.transform.forward * PanForwardRate * dt * Input.GetAxis("CameraPanForward");
            cameraPosition += cam.transform.right * PanLateralRate * dt * Input.GetAxis("CameraPanLeft");
            cameraPosition += cam.transform.up * PanVerticleRate * dt * Input.GetAxis("CameraPanUp");
            
            cam.transform.rotation = cameraRotation;
            cam.transform.position = cameraPosition;
        }
    }
}
