using UnityEngine;

namespace Assets.Ships.Scripts
{
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Game/InertialDrive")]
    public class InertialDrive : MonoBehaviour
    {

        public float PitchSensitivity = 15.0f;
        public float YawSensitivity = 15.0f;
        public float RollSensitivity = 15.0f;

        public float MaxForwardSpeed = 150.0f;

        public float AngularDamping = 10.0f;
        public float LinearDamping = 30.0f;

        public float YawControl { get { return _yawControl; } set { _yawControl = value; } }
        public float PitchControl { get { return _pitchControl; } set { _pitchControl = value; } }
        public float RollControl { get { return _rollControl; } set { _rollControl = value; } }

        public float ThrottleControl { get { return _throttleControl; } set { _throttleControl = value; } }

        public float PowerFactor { get { return _powerFactor; } set { _powerFactor = value; } }

        public AudioClip EngineNoise;

        private float _yawControl, _pitchControl, _rollControl, _throttleControl;
        private float _powerFactor = 1; //When this is 0 we are in hoover mode

        public float Speed { get { return _rigidbody.velocity.magnitude; } }


        Rigidbody _rigidbody;
        AudioSource _audioSource;


 //       Transform mTrans = null;
 //       NetworkView mView = null;
 //       Quaternion mCurrentRot;
 //       Vector3 mLastAngularVelocity, mLastLinearVelocity;

 //       PowerGenerator powerGenerator = null;

 //        bool mControlled = false;

        ///////////////////////////////////////////

        /// <summary>
        /// Determine whether we own this ship, input type, and ensure we have a network observer.
        /// </summary>

        public void Start()
        {
            _rigidbody = rigidbody;
            _audioSource = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        public void FixedUpdate()
        {
            float powerConsumed = ThrottleControl / 4+1;

            //float speed = _chassisBody.LinearVelocity.Length();
            _rigidbody.angularDrag = 10;

            _rigidbody.AddRelativeTorque(Vector3.up * YawControl * YawSensitivity * _powerFactor);
            _rigidbody.AddRelativeTorque(Vector3.left * PitchControl * PitchSensitivity * _powerFactor);
            _rigidbody.AddRelativeTorque(Vector3.forward * RollControl * RollSensitivity * _powerFactor);


            Vector3 desiredLinearVelocity = _rigidbody.rotation * (Vector3.forward * ThrottleControl * MaxForwardSpeed * 0.5f);

            _rigidbody.velocity += (desiredLinearVelocity - rigidbody.velocity) * 0.1f * _powerFactor;

            if (_audioSource != null)
                _audioSource.pitch = powerConsumed;

            //anti gravity
            //float antiGrav = Math.Min(Math.Max(speed / 30, 0), 1);

            //_chassisBody.AddForce(ForceType.GlobalAtLocalPos, TickDelta, -PhysicsWorld.Instance.MainScene.Gravity * _chassisBody.Mass * antiGrav, Vec3.Zero);


            //_chassisBody.LinearDamping = Type.inertial 
        }
    }
}
