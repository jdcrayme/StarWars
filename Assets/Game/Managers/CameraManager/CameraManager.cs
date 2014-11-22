using UnityEngine;

/*
 * This class manages the transition between cameras in our scene
 */
namespace Assets.Game.Managers.CameraManager
{
	public class CameraManager : MonoBehaviour
    {
        #region Public Fields
        public static CameraManager Instance;   //Evil singleton
        #endregion

        #region Private Fields
        private Camera _currentCamera;  //The current camera
	    #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public CameraManager()
        {
            Instance = this;
        }
        #endregion

        #region Getters
        /// <summary>
        /// Returns the rotation of the user camera
        /// </summary>
	    public Quaternion CameraRotation { get { return _currentCamera.transform.rotation; }  }
        
        /// <summary>
        /// Returns the field of view of the user camera
        /// </summary>
        public float CameraFieldOfView { get { return _currentCamera.fieldOfView; } }

        /// <summary>
        /// Returns the position of the user camera
        /// </summary>
        public Vector3 CameraPosition { get { return _currentCamera.transform.position; } }

        /// <summary>
        /// The user camera up vector
        /// </summary>
        public Vector3 CameraUp { get { return _currentCamera.transform.up; } }

        /// <summary>
        /// The user camera forward vector
        /// </summary>
        public Vector3 CameraForward { get { return _currentCamera.transform.forward; } }

        /// <summary>
        /// The user camera right vector
        /// </summary>
        public Vector3 CameraRight { get { return _currentCamera.transform.right; } }
        #endregion


        #region Methods
        /// <summary>
        /// World to screen coords
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Vector3 CameraWorldToScreenPoint(Vector3 position)
        {
            return _currentCamera.WorldToScreenPoint(position);
        }
        
        /// <summary>
        /// Set the current camera
        /// </summary>
        /// <param name="cam"></param>
        internal void SetPrimaryCamera(Camera cam)
        {
            _currentCamera = cam;
        }
        #endregion
    }
}