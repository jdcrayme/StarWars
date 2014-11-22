using Assets.Ships.Scripts;
using UnityEngine;

namespace Assets.Effects.Thrusters
{
    [RequireComponent(typeof(ImprovedTrail))]
    [AddComponentMenu("Game/Ship Thruster Trail")]
    public class ShipThrusterTrail : MonoBehaviour
    {
        ImprovedTrail mTrail;
        InertialDrive mControl;

        void Start ()
        {
            mTrail = GetComponent<ImprovedTrail>();
            mControl = Tools.FindInParents<InertialDrive>(transform);
        }

        void Update ()
        {
            float alpha = (mControl.ThrottleControl);
            mTrail.Alpha = Mathf.Max(0f, alpha);
        }
    }
}