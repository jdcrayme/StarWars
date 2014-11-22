using Assets.Game.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.Managers.CameraManager
{
/*    public class PilotCam : MonoBehaviour, CameraManager.ICamera
    {
        private const float MIN_HORIZ_LOOK_ANGLE = -132;
        private const float MAX_HORIZ_LOOK_ANGLE = 132;

        private const float MIN_VERT_LOOK_ANGLE = -76;
        private const float MAX_VERT_LOOK_ANGLE = 41;

        private const float LOOK_SPEED = 0.1f; //Look speed in degrees per frame

        private Vector3 _defaultEulerAngles;
        private Vector3 _currentEulerAngles;
        private Material _splatterMaterial;
        private Texture2D _texture;
        private float _offset = 0;

        private SpaceUnit _currentUnit;

        public Camera Camera { get { return (Camera)gameObject.GetComponentInChildren(typeof(Camera)); } }

        public bool ShowWaypoints { get { return true; } }

        public void Start()
        {
            _currentEulerAngles = _defaultEulerAngles = transform.localEulerAngles;

            _currentEulerAngles = _defaultEulerAngles = new Vector3(0, 0, 0);

            //GameObject player = GameObject.Find("MainPanel");

            //Debug.Log(player.renderer.materials.Length);

            //_splatterMaterial = player.renderer.material;

            //_texture = new Texture2D(128, 128); //(Texture2D)Instantiate(splatterMaterial.mainTexture);
        }

        public void Activate(GameObject target)
        {
            if (target == null)
                return;

            if (_currentUnit != null)
                _currentUnit.ShowExterior();

            _currentUnit = target.GetComponent<SpaceUnit>();
            if(_currentUnit != null)
                _currentUnit.ShowInterior();

            gameObject.transform.parent = target.transform;
            gameObject.transform.localPosition = new Vector3(0,0,0);
        }

        public void Deactivate()
        {
            if (_currentUnit != null)
                _currentUnit.ShowExterior();
        }

        public void LateUpdate()
        {
            if (Input.GetAxis("LookLeft")!=0)
            {
                _currentEulerAngles += new Vector3(0, 0.5f, 0) * Input.GetAxis("LookLeft");
                //_currentEulerAngles.y = Mathf.Clamp(_currentEulerAngles.y, MIN_HORIZ_LOOK_ANGLE, MAX_HORIZ_LOOK_ANGLE);

            }

            if (Input.GetAxis("LookUp") != 0)
            {
                _currentEulerAngles += new Vector3(0.5f, 0, 0) * Input.GetAxis("LookUp");
                //_currentEulerAngles.x = Mathf.Clamp(_currentEulerAngles.x, MIN_VERT_LOOK_ANGLE, MAX_VERT_LOOK_ANGLE);

            }
            transform.localEulerAngles = _defaultEulerAngles + _currentEulerAngles;
        }

        public void OnPostRender()
        {
            _offset+=0.001f;

            // Create a new texture and assign it to the renderer's material
             //new Texture2D(128, 128);
//            Texture2D texture = (Texture2D) splatterMaterial.mainTexture;
            // Fill the texture with Sierpinski's fractal pattern!
/*            for (var y = 0; y < _texture.height; y++) {
                for (var x = 0; x < _texture.width; x++)
                {
                    if (((int) (x*_offset) & (int) (y*_offset)) != 0)
                    {
                        _texture.SetPixel(x, y, Color.blue);
                    }
                    else
                    {
                        _texture.SetPixel(x, y, Color.white);
                    }
                    //var color = (x&y)!=0 ? Color.white : Color.gray;
                }
            }
            // Apply all SetPixel calls
            _texture.Apply();

            _splatterMaterial.SetTexture("_MainTex", _texture);*/
 //       }
 //   }
}
