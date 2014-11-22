using UnityEngine;

namespace Assets
{
    public class ShieldParticles : MonoBehaviour
    {
        public float DeadTime;

        private bool _alive=true;
        private bool _destroyed = false;

        public void KeepAlive()
        {
            _alive = true;
        }

        public void Update()
        {
            if (!_alive&&!_destroyed)
            {
                Destroy(gameObject, DeadTime);
                _destroyed = true;
            }
            _alive = false;
        }
    }
}
