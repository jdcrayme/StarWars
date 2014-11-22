using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Ships.Scripts
{
    public class Emitter : MonoBehaviour {
        public static List<Emitter> TheaterEmitters = new List<Emitter>();

        public string Designator = "Unknown";
        public string Name = "Unknown";
        public string Description = "Unknown";

        public Material Image;

        public float RadarCrossSection = 1;
        public float InfaredBloom = 1;
        public float RadioEmision = 1;

        /// <summary>
        /// Register / unregister this heat source with the global list.
        /// </summary>
        public void Awake() { TheaterEmitters.Add(this); }
        public void OnDestroy() { TheaterEmitters.Remove(this); }

        /// <summary>
        /// Find the target reticle, if there is one.
        /// </summary>
        public void Start()
        {
            
        }

        public void Update()
        {
        }
    }
}
