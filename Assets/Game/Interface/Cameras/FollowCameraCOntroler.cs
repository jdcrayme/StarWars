using System;
using UnityEngine;

namespace Assets.Game.Interface.Cameras
{
    [Serializable]
    public class FollowCameraControler : ICameraControler
    {
        public GameObject Target;

        public float FollowDist;
        public float PositionSnap;
        public float RotationSnap;
        
        /// <summary>
        /// Returns a data string describing what this camera is doing
        /// </summary>
        public string DataString{ get { return "Follow Cam " + (Target == null ? "" : ("Target: " + Target.name)); }}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="followDistance"> How far away from our target to stay </param>
        /// <param name="positionSnap"> How many seconds to get into position </param>
        /// <param name="rotationSnap"> How many seconds to look correctly </param>
        public FollowCameraControler(float followDistance, float positionSnap, float rotationSnap)
        {
            FollowDist = followDistance;
            PositionSnap = positionSnap;
            RotationSnap = rotationSnap;
        }

        /// <summary>
        /// Updates a camera's position
        /// </summary>
        /// <param name="cam">The camera to update</param>
        public void Update(Camera cam)
        {
            if(Target== null)
                return;

            var desiredPosition = (cam.transform.position - Target.transform.position).normalized*FollowDist + Target.transform.position;

            cam.transform.position = Vector3.Lerp(cam.transform.position, desiredPosition, PositionSnap);

            cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation,
                Quaternion.LookRotation(Target.transform.position - cam.transform.position), RotationSnap);
        }
    }
}
