using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Ships.Scripts.UI
{
    public class HudRadar : MonoBehaviour {

        public Sensors Sensor;
        public GameObject RadarContactPrefab;
        public float Scale = 20;

        private readonly Dictionary<Sensors.SensorTrack, RadarTrack> _activeTracks = new Dictionary<Sensors.SensorTrack, RadarTrack>();

        // Use this for initialization
        public void Start () {
	
        }
	
        // Update is called once per frame
        public void Update()
        {

            foreach (var track in Sensor.CurrentTracks.Values)
            {
                //If we havent cataloged this one yet, then add it
                if (!_activeTracks.ContainsKey(track))
                {
                    //Add to the hud
                    var obj = (GameObject) Instantiate(RadarContactPrefab);
                    obj.transform.parent = gameObject.transform;
                    _activeTracks.Add(track, obj.GetComponent<RadarTrack>());
                }


                var radarTrack = _activeTracks[track];

                //Update the Hud
                radarTrack.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                var trackPos = Sensor.ToRadarCoord(track.Emitter.gameObject.transform.position, true)*Scale;

                trackPos.z = 0;

                radarTrack.gameObject.transform.localPosition = trackPos;
            }

            foreach (var oldTrack in Sensor.OldTracks)
            {
                Destroy(_activeTracks[oldTrack].gameObject);
                _activeTracks.Remove(oldTrack);
            }
        }
    }
}
