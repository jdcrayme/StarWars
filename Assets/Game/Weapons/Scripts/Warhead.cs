using System.Collections.Generic;
using Assets.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.Weapons.Scripts
{
    public class Warhead : MonoBehaviour {

        public GameObject ExplosionPrefab;

        public float Force = 200f;
        public float Radius = 20f;

        // Use this for initialization
        public void Start () {
	
        }
	
        // Update is called once per frame
        public void Update () {
	
        }

        /// <summary>
        /// Explode on collision.
        /// </summary>

        public void OnCollisionEnter(Collision col) { Explode(); }

        /// <summary>
        /// Explode the explosive, adding an explosion force and creating an explosion prefab.
        /// </summary>

        public void Explode()
        {
            Rigidbody myRigidbody = rigidbody;
            Vector3 pos = transform.position;

            // Get a list of colliders caught int he blast
            Collider[] cols = Physics.OverlapSphere(pos, Radius);

            // Convert the list of colliders into a list of rigidbodies
            List<Rigidbody> rbs = Tools.GetRigidbodies(cols);

            // Apply the explosion force to all rigidbodies caught in the blast
            foreach (Rigidbody rb in rbs)
            {
                if (rb != myRigidbody)
                {
                    rb.AddExplosionForce(Force, pos, Radius, 0f);
                    var hull = rb.gameObject.GetComponent<Hull>();
                    if (hull != null)
                        hull.Damage(Force);
                }
            }

            // Instantiate an explosion prefab
            if (ExplosionPrefab != null)
            {
                Instantiate(ExplosionPrefab, pos, transform.rotation);
            }

            // Destroy this game object
            Destroy(gameObject);
        }
    }
}
