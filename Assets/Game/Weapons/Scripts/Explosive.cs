using UnityEngine;

namespace Assets.Weapons
{
    [AddComponentMenu("Game/Explosive")]
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(NetworkView))]
    public class Explosive : MonoBehaviour
    {
        public GameObject ExplosionPrefab;
        public float Force = 200f;
        public float Radius = 20f;

        /// <summary>
        /// We need to know if this explosive is ours or not. If it is, we'll be the ones exploding it.
        /// </summary>

        public void Start ()
        {
        }

        /// <summary>
        /// Explode on collision.
        /// </summary>

        public void OnCollisionEnter (Collision col) { Explode(); }

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
            //List<Rigidbody> rbs = GetRigidbodies(cols);

            // Apply the explosion force to all rigidbodies caught in the blast
            //foreach (Rigidbody rb in rbs)
            //{
              //  if (rb != myRigidbody)
                //{
                    // TODO: Apply damage and force here
                    //Rigidbody nrb = Rigidbody.Find(rb);
                    //if (nrb != null) nrb.AddExplosionForce(Force, pos, Radius, 0f);
                //}
            //}

            // Instantiate an explosion prefab
            if (ExplosionPrefab != null)
            {
                Instantiate(ExplosionPrefab, pos,Quaternion.identity);
            }

            // Destroy this game object
            Destroy(gameObject);
        }
    }
}