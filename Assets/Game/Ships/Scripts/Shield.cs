using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Ships.Scripts
{
    public class Shield : MonoBehaviour {

        public GameObject ShieldHitPrefab;

        private readonly Dictionary<GameObject, ShieldParticles> _activeCollisions = new Dictionary<GameObject, ShieldParticles>();
        
        /// <summary>
        /// Locate the spaceship and the power generator components.
        /// </summary>
        public void Start()
        {
        }

        public void OnCollisionEnter(Collision collision)
        {
            // Rotate the object so that the y-axis faces along the normal of the surface
            var contact = collision.contacts[0];
            var rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            var pos = contact.point;
            var obj = (GameObject)Instantiate(ShieldHitPrefab, pos, rot);
            var particles = obj.GetComponent<ShieldParticles>();

            _activeCollisions.Add(collision.gameObject, particles);
        }

        void OnCollisionStay(Collision collision)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                Debug.DrawRay(contact.point, contact.normal, Color.white);
            }

            if (_activeCollisions.ContainsKey(collision.gameObject)) 
                _activeCollisions[collision.gameObject].KeepAlive();
        }

        public void OnCollisionExit(Collision collision)
        {
            
        }

        // Update is called once per frame
        public void Update () {
        }
    }
}
