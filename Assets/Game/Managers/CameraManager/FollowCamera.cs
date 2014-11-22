using UnityEngine;

namespace Assets.Game.Managers.CameraManager
{
/*    [RequireComponent(typeof(Camera))]
    public class FollowCamera : MonoBehaviour, CameraManager.ICamera
    {
        public Camera Camera { get { return (Camera) gameObject.GetComponent(typeof (Camera)); } }

        public bool ShowWaypoints { get { return false; } }

        // Use this for initialization
        public void Start () {
        }
	
        // Update is called once per frame
        public void Update () {
	
        }

        public void Activate(GameObject target) {
            if(target==null)
                return;

            gameObject.transform.parent = target.transform;

            gameObject.transform.localPosition = new Vector3(0, 3, -12);
            gameObject.transform.localRotation = Quaternion.identity;
            gameObject.transform.localScale = Vector3.one;
        }

        public void Deactivate()
        {
        }
    }*/
}
