using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Game.Ships.Scripts
{
    [AddComponentMenu("ShipComponants/Sensors")]
    public class Sensors : MonoBehaviour {

        public Dictionary<Emitter, SensorTrack> CurrentTracks = new Dictionary<Emitter, SensorTrack>();
        public List<SensorTrack> OldTracks = new List<SensorTrack>();

        private Emitter _selfEmiter;

        public class SensorTrack{
            public Emitter Emitter { get; private set; }
            public int LastUpdatedFrame { get; private set; }

            public SensorTrack(Emitter emiter)
            {
                Emitter = emiter;
            }

            protected internal void Update(int currentFrame)
            {
                LastUpdatedFrame = currentFrame;
            }
        }

        // Use this for initialization
        public void Start () {
            //Get a copy of the ships own emiter so we don't see ourselves on radar.
            _selfEmiter = GetComponent<Emitter>();
        }
	
        // Update is called once per frame
        public void Update ()
        {
            OldTracks.Clear();

            var currentFrame = Time.frameCount;

            //Cycle through the theater emitters)
            foreach (var emiter in Emitter.TheaterEmitters)
            {
                //If we are seeing ourselves, or we can't see the target, then move on
                if (emiter == _selfEmiter)
                    continue;

                //If we havent cataloged this one yet, then add it
                if (!CurrentTracks.ContainsKey(emiter))
                {
                    var newTarget = new SensorTrack(emiter);

                    CurrentTracks.Add(emiter, newTarget);
                }

                CurrentTracks[emiter].Update(currentFrame);
            }

            //If a tracks emitter is not found then it was not updated. remove it.
            foreach (var track in CurrentTracks.Where(track => track.Value.LastUpdatedFrame != currentFrame))
            {
                OldTracks.Add(track.Value);
            }

            foreach (var track in OldTracks)
            {
                CurrentTracks.Remove(track.Emitter);
            }
        }
        /*
        public List<Vector3> GetContacts(SensorMode mode)
        {
            var contacts = new List<Vector3>();

            foreach (var emitter in Emitter.TheaterEmitters)
            {
                //Do not create a contact for the ship that we are a part of
                if (emitter.transform == _transform)
                    continue;

                var pos = emitter.transform.position;

                switch (mode)
                {
                    case SensorMode.World:
                        contacts.Add(pos);
                        break;
                    case SensorMode.RelativePosition:
                        contacts.Add(pos - _transform.position);
                        break;
                    case SensorMode.RelativePositionAndOrientation:
                        contacts.Add(ToRadarCoord(pos,true));
                        break;
                }
            }
            return contacts;
        }*/

        public Vector3 ToRadarCoord(Vector3 target, bool norm)
        {
            var trans = transform.InverseTransformPoint(target);

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
            if (norm) { x1 *= dist; y1 *= dist; }

            return new Vector3((float) x1, (float) -y1, (float) dist);
        }

        public double GetDistanceToPoint(Vector3 point)
        {
            return (point - transform.position).magnitude;
        }
    }
}
