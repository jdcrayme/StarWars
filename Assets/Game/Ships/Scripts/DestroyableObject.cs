using System.Linq;
using UnityEngine;

namespace Assets.Game.Ships.Scripts
{
    public class DestroyableObject : MonoBehaviour {

        public float HullStrength;
        public GameObject HullHitPrefab;

        public float ShieldStrength;
        public Collider ShieldCollider;
        public GameObject ShieldHitPrefab;
        
        public GameObject DebrisPrefab;


        private const float DAMAGE_FACTOR = 1f;

        public void Damage(float damage)
        {
            if (ShieldStrength - damage <= 0)
            {
                HullStrength -= (damage - ShieldStrength);
                ShieldStrength = 0;
            }
            else
                ShieldStrength -= damage;
        }

        public void OnCollisionEnter(Collision collision)
        {
            foreach (var contact in collision.contacts)
            {
                // Rotate the object so that the y-axis faces along the normal of the surface
                var rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
                var pos = contact.point;

                Damage(DAMAGE_FACTOR*collision.relativeVelocity.magnitude*collision.rigidbody.mass);

                if (contact.thisCollider == ShieldCollider)
                {
                    GenerateShieldHitEffect(pos, rot);
                }
                else
                {
                    GenerateHullHitEffect(pos, rot);
                }
            }
        }

        public void OnCollisionStay(Collision collision)
        {
            foreach (var contact in collision.contacts.Where(contact => contact.thisCollider == ShieldCollider))
            {
                DamageObjectTouchingShield(collision, contact);
            }
        }

        private void DamageObjectTouchingShield(Collision collision, ContactPoint contact)
        {
            var destroyableObject = contact.otherCollider.gameObject.GetComponent<DestroyableObject>();
            if (destroyableObject != null)
                destroyableObject.Damage(ShieldStrength * DAMAGE_FACTOR * collision.relativeVelocity.magnitude);
        }

        private void GenerateHullHitEffect(Vector3 pos, Quaternion rot)
        {
            if (HullHitPrefab == null) return;

            Instantiate(HullHitPrefab, pos, rot);
        }

        private void GenerateShieldHitEffect(Vector3 pos, Quaternion rot)
        {
            if (ShieldHitPrefab == null) return;

            Instantiate(ShieldHitPrefab, pos, rot);
        }

        public virtual void Update()
        {
            if (HullStrength >= 0) return;

            Destroy(gameObject);
            if (DebrisPrefab != null)
                Instantiate(DebrisPrefab, transform.position, transform.rotation);
        }
    }
}
