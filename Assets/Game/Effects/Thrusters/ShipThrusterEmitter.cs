using Assets.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.Effects.Thrusters
{
    [RequireComponent(typeof(ParticleEmitter))]
    [AddComponentMenu("Game/Ship Thruster Emitter")]
    public class ShipThrusterEmitter : MonoBehaviour
    {
        ParticleEmitter mEmitter;
        InertialDrive mControl;
        Vector2 mEmission;
        Vector3 mDir;
        Vector3 mVel;

        void Start()
        {
            mEmitter = GetComponent<ParticleEmitter>();
            mControl = Tools.FindInParents<InertialDrive>(transform);
            mEmission = new Vector2(mEmitter.minEmission, mEmitter.maxEmission);
            mVel = mEmitter.localVelocity;
            mDir = transform.rotation * Vector3.back;
        }

        void Update()
        {
            if (mControl.ThrottleControl < 0.01f)
            {
                mEmitter.emit = false;
            }
            else
            {
                mEmitter.emit = true;
            }


            Vector3 move = Vector3.forward*mControl.ThrottleControl;
            float dot = Mathf.Min(1f + move.z, 1f) * Vector3.Dot(move, mDir);
            //mEmitter.localVelocity = new Vector3(0,0,-5) * 1;
            mEmitter.minEnergy = mEmitter.maxEnergy = (mControl.ThrottleControl);
            transform.localScale = new Vector3(1,1, mControl.ThrottleControl);
        }
    }
}