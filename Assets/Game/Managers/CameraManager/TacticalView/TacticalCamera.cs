using UnityEngine;

namespace Assets.TacticalView.Scripts
{
    public class TacticalCamera : MonoBehaviour {

        Transform mTrans = null;

        public bool fixUpVector = true;
        public double ScrollWidth = 15;
        public float ScrollSpeed = 0.5f;
        public float MaxCameraHeight = 10;
        public float MinCameraHeight = 10;
        public float RotateAmount = 10;
        public float RotateSpeed = 100;
        private Camera mCam;

        private float minOrthoSize = 5;
        private float maxOrthoSize = 200;
        private const float zoomSpeed = 10;

        // Use this for initialization
        void Start () {
            mTrans = transform;
            mCam = camera;
        }
	
        // Update is called once per frame
        void Update ()
        {
            MoveCamera();
            RotateCamera();
        }

        private void MoveCamera()
        {
            var xpos = Input.mousePosition.x;
            var ypos = Input.mousePosition.y;
            var movement = new Vector3(0, 0, 0);

            //Zoom
            mCam.orthographicSize += zoomSpeed * Input.GetAxis("Mouse ScrollWheel");

            //Limit the ortho size to keep from doing anything wierd
            if (mCam.orthographicSize < minOrthoSize)
                mCam.orthographicSize = minOrthoSize;
            else if (mCam.orthographicSize > maxOrthoSize)
                mCam.orthographicSize = maxOrthoSize;



            //horizontal camera movement
            if (xpos >= 0 && xpos < ScrollWidth)
            {
                movement.x -= ScrollSpeed * mCam.orthographicSize;
            }
            else if (xpos <= Screen.width && xpos > Screen.width - ScrollWidth)
            {
                movement.x += ScrollSpeed * mCam.orthographicSize;
            }

            //vertical camera movement
            if (ypos >= 0 && ypos < ScrollWidth)
            {
                movement.z -= ScrollSpeed * mCam.orthographicSize;
            }
            else if (ypos <= Screen.height && ypos > Screen.height - ScrollWidth)
            {
                movement.z += ScrollSpeed * mCam.orthographicSize;
            }

            //make sure movement is in the direction the camera is pointing
            //but ignore the vertical tilt of the camera to get sensible scrolling

            if (!fixUpVector)
                movement = mTrans.transform.TransformDirection(movement);

            //calculate desired camera position based on received input
            Vector3 origin = mTrans.transform.position;
            Vector3 destination = origin;
            destination.x += movement.x;
            destination.y += movement.y;
            destination.z += movement.z;

            //if a change in position is detected perform the necessary update
            if (destination != origin)
            {
                mTrans.position = Vector3.MoveTowards(origin, destination, Time.deltaTime * ScrollSpeed);
            }
        }

        private void RotateCamera()
        {
            var origin = mTrans.transform.eulerAngles;
            var destination = new Vector3(0,0,0);// origin;

            //detect rotation amount if ALT is being held and the Right mouse button is down
            if ((Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt)) && Input.GetMouseButton(1))
            {
                destination.x -= Input.GetAxis("Mouse X") * RotateAmount;
                destination.y += Input.GetAxis("Mouse Y") * RotateAmount;
            }
            destination *= 0.02f;
            //if a change in position is detected perform the necessary update
            if (destination != origin)
            {
                if (!fixUpVector)
                    mTrans.Rotate(destination.y, destination.x, 0);
                else
                {
                    mTrans.Rotate(0, 0, destination.x, Space.World);
                    mTrans.Rotate(destination.y, 0, 0, Space.Self);
                }
                //mTrans.transform.eulerAngles = Vector3.MoveTowards(origin, destination, Time.deltaTime * RotateSpeed);
            }
        }
    }
}
