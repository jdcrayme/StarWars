using System;
using Assets.Game.Ships.Scripts;
using Assets.Game.Weapons.Scripts;
using UnityEngine;

namespace Assets.Game.Weapons
{
    public class Weapon : MonoBehaviour
    {
        public GameObject Projectile;
        public ParticleSystem MuzzleFlash;

        public AudioClip FireSound;

        public float ReloadTimeInSeconds;
        public float RechargeRatePerSecond;
        public float ChargePerShot;
        public float MaxCharge;

        private SpaceUnit _launchingUnit;

        private float _currentCharge;
        private float _lastShotTime;
        private Rigidbody _launcherRigidbody;
        private bool _disabled;

        // Use this for initialization
        public void Start()
        {
            _launchingUnit = gameObject.transform.parent.GetComponent<SpaceUnit>();

            if (_launchingUnit != null)
                _launcherRigidbody = _launchingUnit.GetComponent<Rigidbody>();
            else {
                Debug.LogError("Launching unit not found");
                _disabled = true;
            }

            if (_launcherRigidbody == null)
                Debug.LogError("Launcher Rigedbody not found");

            if (Projectile == null){
                Debug.LogError("Weapon Projectile not assigned");
                _disabled = true;
            }
        }

        // Update is called once per frame
        public void Update()
        {
            //If the weapon is disabled then do nothing
            if (_disabled)
                return;

            //If we are not fully charged, then charge
            if (_currentCharge < MaxCharge)
            {
                _currentCharge += Time.deltaTime * RechargeRatePerSecond;
                _currentCharge = Math.Min(_currentCharge, MaxCharge);
            }


        }

        public void Fire()
        {
            //If the weapon is disabled then do nothing
            if(_disabled)
                return;

            //If we don't have enough charge or have not met the reload time, then we can't fire
            if (_currentCharge < ChargePerShot || Time.time <= _lastShotTime + ReloadTimeInSeconds) 
                return;
            
            _currentCharge -= ChargePerShot;
            _lastShotTime = Time.time;

            // Instantiate a new object
            var go = (GameObject)Instantiate(Projectile, transform.position, transform.rotation);
            // The weapon's initial velocity should match the launcher's
            if (go != null)
            {
                var projectileRigedBody = go.GetComponent<Rigidbody>();
                var projectile = go.GetComponent<Projectile>();

                if (projectileRigedBody != null)
                {
                    if (_launcherRigidbody != null)
                    {
                        projectileRigedBody.velocity = _launcherRigidbody.velocity + transform.rotation* (Vector3.forward*(projectile.FiringVelocity));
                    }
                    else
                    {
                        projectileRigedBody.velocity = transform.rotation * (Vector3.forward * (projectile.FiringVelocity / 3.6f));
                    }

                    if(FireSound!=null)
                        AudioSource.PlayClipAtPoint(FireSound, transform.position);

                    if(MuzzleFlash!=null)
                        MuzzleFlash.Emit(10);
                }
                else
                {
                    Debug.LogError("Error instantiating projectile");
                }
            }
        }
    }
}