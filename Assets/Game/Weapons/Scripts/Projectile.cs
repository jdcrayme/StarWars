using Assets.Game.Ships.Scripts;
using UnityEngine;

namespace Assets.Game.Weapons.Scripts
{
    [AddComponentMenu("Game/Projectile")]
    public class Projectile : DestroyableObject
    {
        /// <summary>
        /// How long the object will live from the time it has been created.
        /// </summary>
        public float TimeToLive = 10f;

        /// <summary>
        /// The velocity of this object refrence the launching platform
        /// </summary>
        public float FiringVelocity = 150f;

        /// <summary>
        /// Current lifetime progress of the fired object.
        /// </summary>
        public float Lifetime { get { return (Time.time - _spawnTime) / TimeToLive; } }

        float _spawnTime;
        float _destroyTime;

        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            _spawnTime = Time.time;
            _destroyTime = _spawnTime + TimeToLive;
        }

        /// <summary>
        /// Update
        /// </summary>
        public override void Update ()
        {
            base.Update();

            var time = Time.time;

            if (time > _destroyTime)
                Destroy(gameObject);
        }
    }
}