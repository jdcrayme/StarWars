using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game
{
	public class Waypoint : MonoBehaviour {

		public static List<Waypoint> Waypoints = new List<Waypoint>();
        
		public void Start () {
			Waypoints.Add(this);
		}
	
		public void Update () {
		}
	}
}
