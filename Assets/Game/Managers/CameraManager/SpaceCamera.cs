using UnityEngine;

namespace Assets.Game.Managers.CameraManager
{
    public class SpaceCamera : MonoBehaviour
    {
        // Whether or not SpaceCamera should change FOV if parent camera FOV is changed
        public bool InheritFieldOfView = true;
        // Relative speed if you wish to move within the space scene, 
        // use with caution as you will go through planets and beyond nebulas unless you create boundaries yourself.
        public float RelativeSpeed;

        // Private variables
        private Vector3 _originalPosition;
        private Transform _transformCache;

        // The space camera must have a reference to a parent camera so it knows how to rotate the background
        // This script allows you to specify a parent camera (parentCamera) which will act as reference
        // If you do not specify a camera, the script will assume you are using the main camera and select that as reference	
        public void Start()
        {
            // Cache the transform to increase performance
            _transformCache = transform;
        }

        public void Update()
        {
            // Update the rotation of the space camera so the background rotates		
            _transformCache.rotation = CameraManager.Instance.CameraRotation;
            if (InheritFieldOfView) camera.fieldOfView = CameraManager.Instance.CameraFieldOfView;

            // Update the relative position of the space camera so you can travel in the space scene if necessary
            // Note! You will fly out of bounds of the space scene if your relative speed is high unless you restrict the movement in your own code.
            _transformCache.position = Vector3.zero;
        }
    }
}