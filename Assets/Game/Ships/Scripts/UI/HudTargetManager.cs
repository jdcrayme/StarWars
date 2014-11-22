using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Ships.Scripts.UI
{
    public class HudTargetManager : MonoBehaviour {

        public GameObject HudTargetPrefab;
        public GameObject RadarTargetPrefab;

        public GameObject HUDRadar;

        public Sensors Sensors;

        private class Target
        {
            private Emitter _emmitterObject;
            private HudTrack _hudTrack;
            private GameObject _radarTarget;

            public Target(Emitter emmitterObject, GameObject hud, GameObject hudRadar, GameObject hudTargetPrefab, GameObject radarTargetPrefab)
            {
                _emmitterObject = emmitterObject;

                //Add to the hud
                var obj = (GameObject)Instantiate(hudTargetPrefab);
                obj.transform.parent = hud.transform;
                _hudTrack = obj.GetComponent<HudTrack>();

                //Add to the radar
                _radarTarget = (GameObject)Instantiate(radarTargetPrefab);
                _radarTarget.transform.parent = hudRadar.transform;
            }

            public void Update(Transform sensorTransform, float scale)
            {
                var relPosition = _emmitterObject.gameObject.transform.position - sensorTransform.position;

                //Update the Hud
                _hudTrack.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                _hudTrack.gameObject.transform.LookAt(relPosition, sensorTransform.right);

                //Update the radar
                var trans = sensorTransform.InverseTransformPoint(_emmitterObject.gameObject.transform.position);

                double x = trans.x;
                double y = -trans.y;
                double z = trans.z;
                double x1, y1;
                double kot;

                kot = Math.Atan2(y, x);
                var dist = x * x + y * y;
                dist = Math.Sqrt(dist);

                var k2 = Math.Atan2(dist, z);
                k2 *= 0.5f;
                dist = Math.Sin(k2);
                x1 = Math.Cos(kot);
                y1 = Math.Sin(kot);
                if (true) { x1 *= dist; y1 *= dist; }

                _radarTarget.gameObject.transform.localPosition = new Vector3((float)x1 * scale, (float)-y1 * scale, (float)dist * scale);
                _radarTarget.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);

            }
        }

        private readonly Dictionary<Sensors.SensorTrack, HudTrack> _activeTracks = new Dictionary<Sensors.SensorTrack, HudTrack>();

        // Use this for initialization
        public void Start () {
	
        }
	
        // Update is called once per frame
        public void Update () {
            foreach (var track in Sensors.CurrentTracks.Values)
            {
                //If we havent cataloged this one yet, then add it
                if (!_activeTracks.ContainsKey(track))
                {
                    //Add to the hud
                    var obj = (GameObject)Instantiate(HudTargetPrefab);
                    obj.transform.parent = gameObject.transform;
                    _activeTracks.Add(track, obj.GetComponent<HudTrack>());
                }


                var hudTarget = _activeTracks[track];

                var relPosition = track.Emitter.gameObject.transform.position - Sensors.gameObject.transform.position;

                //Update the Hud
                hudTarget.gameObject.transform.localPosition = new Vector3(0, 0, 0);
                hudTarget.gameObject.transform.LookAt(relPosition, Sensors.gameObject.transform.right);
            }

            foreach (var oldTrack in Sensors.OldTracks)
            {
                Destroy(_activeTracks[oldTrack].gameObject);
                _activeTracks.Remove(oldTrack);
            }
        }
    }
}
