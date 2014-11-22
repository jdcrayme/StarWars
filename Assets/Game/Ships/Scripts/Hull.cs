using UnityEngine;

namespace Assets.Ships.Scripts
{
	[RequireComponent(typeof(Rigidbody))]
	[AddComponentMenu("Ship/Hull")]
	public class Hull : MonoBehaviour {

		public Object Explosion;
	    private bool _dead;

	    // Use this for initialization
		public void Start () {
	
		}
	
		// Update is called once per frame
		public void Update () {
	
		}

        public void Damage(float damage)
        {
            
        }

		public void OnCollisionEnter(Collision collision)
		{
            if (!_dead)
            {
                SpawnExplosion();
                _dead = true;

            }
		}

		void SpawnExplosion()
		{
		
			var exp = (GameObject)Instantiate(Explosion, transform.position, Quaternion.identity);
		    Destroy(gameObject);
			Destroy(exp,15f); 
		}
	}
}
