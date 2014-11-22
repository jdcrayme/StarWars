using Assets.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.Ships.Scripts
{
    [RequireComponent(typeof(ImprovedTrail))]
    [AddComponentMenu("Game/Ship Thruster Trail")]
    public class ThrusterTrailControler : MonoBehaviour
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
            mTrail.alpha = Mathf.Max(0f, alpha);
        }
    }
}