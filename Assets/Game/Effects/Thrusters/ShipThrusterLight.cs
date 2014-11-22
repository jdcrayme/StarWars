using Assets.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.Effects.Thrusters
{
    [RequireComponent(typeof(Light))]
    [AddComponentMenu("Game/Ship Thruster Light")]
    public class ShipThrusterLight : MonoBehaviour
    {
        public Vector2 VariationPercent = new Vector2(0.9f, 1.1f);
        public float ChangeFrequency = 0.01f;
        public float ChangeSpeed = 30f;

        Light _light;
        float _original;
        float _target;
        float _nextChange;

        InertialDrive _control;

        public void Start()
        {
            _light = GetComponent<Light>();
            _original = _light.intensity;
            _control = Tools.FindInParents<InertialDrive>(transform);
        }

        public void Update()
        {
            _light.intensity = _control.ThrottleControl*_original;
            _light.enabled = (_light.intensity > 0.01f);
        }
    }
}