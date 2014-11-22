using System;
using System.Collections.Generic;
using Assets.Game.Ships.Scripts;
using UnityEngine;

namespace Assets.Ships.Scripts.AI
{
    [RequireComponent(typeof(Sensors))]
    [RequireComponent(typeof(InertialDrive))]
    [AddComponentMenu("Game/AIPilot")]
    public class AIPilot : MonoBehaviour {

        [System.Serializable]
        public class Order
        {
            public enum Action {Stop, Move, Patrol, Attack, Scout}

            public Action OrderAction = Action.Move;

            public List<GameObject> Targets;
        }

        //Order list
        public List<Order> Orders = new List<Order>();
        
        //Rendering stuff
        private static Material _lineMaterial;

        //Spaceship stuff
        private const float TURN_DISTANCE = 15;

        Sensors _sensors;
        InertialDrive _inertialDrive;
        Vector3 _delta;
        int _steeringTouch = -1;
        float _nextPrimary;
        float _nextSecondary;

        //readonly List<Weapon> _primaryWeapon = new List<Weapon>();
        //readonly List<Weapon> _secondaryWeapon = new List<Weapon>();
        //private Vector3 targetPoint;

        public Vector3 TargetPoint
        {
            get;
            private set;
        }

        // Use this for initialization
        public void Start()
        {

            // We need to know the ship we're controlling.
            _sensors = GetComponent<Sensors>();

            // Get the controls
            _inertialDrive = GetComponent<InertialDrive>();

            // Find all weapons
            /*Weapon[] weapons = GetComponentsInChildren<Weapon>();

            foreach (Weapon w in weapons)
            {
                if (w.group == Weapon.Group.Primary) _primaryWeapon.Add(w);
                if (w.group == Weapon.Group.Secondary) _secondaryWeapon.Add(w);
            }*/
        }

        // Update is called once per frame
        public void Update()
        {
            Order order= null;
            var orderComplete = false;

            if (Orders.Count > 0)
                order = Orders[0];

            if (order != null)
            {
                switch (order.OrderAction)
                {
                    case Order.Action.Move:
                        orderComplete = Move(order);
                        break;

                    case Order.Action.Patrol:
                        orderComplete = Patrol(order);
                        break;

                    default:
                        Stop();
                        break;
                }

                //If we have comleated the required action remove the order from the list
                if (orderComplete)
                    Orders.Remove(order);
            }
            else
            {
                Stop();
            }

            /*var curentOrder = Orders.GetCurrentOrder();

            if (curentOrder != null)
            {
                //We are supposed to move towards a point or object
                if (curentOrder.Task == OrderList.Order.AITask.Move)
                {
                    //Get the apropriate coordiantes
                    if (curentOrder.Target != null)
                    {
                        Attack(curentOrder.Target);
                    }
                    else
                    {
                        var point = curentOrder.GetPoint();

                        //If this is the last order in the chain, sop when we get there..
                        if (Orders.IsLast(curentOrder))
                            FlyTo(point);

                        //...otherwise, if we fly through the point, move on to the next order
                        else if (FlyThrough(point))
                            Orders.DeleteCurrentOrder();
                    }
                    //We are supposed to patrol an area, or protect an object
                }
                else if (curentOrder.Task == OrderList.Order.AITask.Patrol)
                {
                    //Get the apropriate coordiantes
                    var point = curentOrder.GetPoint();

                    //If this is the last order in the chain, sop when we get there..
                    if (FlyThrough(point))
                    {
                        Orders.SelectNextOrder();
                        if (Orders.GetCurrentOrder() == null)
                            Orders.SelectFirstOrder();
                    }
                }
            }
            else
            {
                Stop();
                return;
            }*/
        }

        public void Stop()
        {
            if (_inertialDrive != null && _sensors != null)
            {
                _inertialDrive.ThrottleControl = 0.0f;
            }
        }

        /// <summary>
        /// Flys past/through a list of targets 
        /// </summary>
        public bool Move(Order order)
        {
            //if there are no targets left to move to then the order is complete
            if (order.Targets == null || order.Targets.Count < 1)
                return true;

            var target = order.Targets[0];
            TargetPoint = target.transform.position;

            //If the target has an emiter then it is a ship/object
            if (target.GetComponent(typeof(Emitter)))
            {
                //TODO add code allowing the figher to fly upto but not into the target. Bounding sphere +10% is probably a good turn distance
            }
            //If no, then it is a waypoint, just fly right through
            else
            {
                //If we do fly through, then remove the target from the list
                if (FlyThrough(target.transform.position))
                    order.Targets.Remove(target);
            }
            return false;
        }

        /// <summary>
        /// Repeatedly flys past/through a list of targets 
        /// </summary>
        public bool Patrol(Order order)
        {
            //if there are no targets left to move to then the order is complete
            if (order.Targets == null || order.Targets.Count < 1)
                return true;

            var target = order.Targets[0];
            TargetPoint = target.transform.position;

            //If the target has an emiter then it is a ship/object
            if (target.GetComponent(typeof(Emitter)))
            {
                //TODO add code allowing the figher to fly upto but not into the target. Bounding sphere +10% is probably a good turn distance
            }
            //If no, then it is a waypoint, just fly right through
            else
            {
                //If we do fly through, then remove the target from the list
                if (FlyThrough(target.transform.position))
                {
                    //remove it from the front of the list...
                    order.Targets.Remove(target);
                    //...add it to the back
                    order.Targets.Add(target);
                }
            }
            return false;
        }

        public bool FlyThrough(Vector3 target)
        {
            if (_inertialDrive == null || _sensors == null)
                return false;

            PointAtTarget(target);
            _inertialDrive.ThrottleControl = 0.5f;

            var dist = (float)_sensors.GetDistanceToPoint(target);
            return dist < TURN_DISTANCE ? true : false;
        }

        public void FlyTo(Vector3 target)
        {
            if (_inertialDrive == null || _sensors == null)
                return;

            PointAtTarget(target);

            var dist = (float)_sensors.GetDistanceToPoint(target);
            _inertialDrive.ThrottleControl = dist < TURN_DISTANCE ? dist / (TURN_DISTANCE * 10) : 0.5f;
        }

        private void PointAtTarget(Vector3 target)
        {
            var dif = _sensors.ToRadarCoord(target, true)*1000;

            _inertialDrive.RollControl = dif.x;
            _inertialDrive.YawControl = dif.x;
            _inertialDrive.PitchControl = dif.y;
        }

        /*
        private void Attack(HeatSource target)
        {
            const float trigerCircle = 0.25f;
            const float desiredDistance = 20;

            var fireVelocity = _primaryWeapon[0].firedObject.firingVelocity;
            targetPoint = CalculatePursutePoint(target.rigidbody.position, target.rigidbody.velocity, rigidbody.position, fireVelocity);

            PointAtTarget(targetPoint);

            var dist = (float)_sensors.GetDistanceToPoint(target.mTrans.position);
            _inertialDrive.throttle = dist < desiredDistance ? dist / (desiredDistance * 10) : 1.0f;


            //If the target is not close to our shoot reticle then exit..
            var dif = _sensors.ToRadarCoord(targetPoint, true);
            if (Mathf.Abs(dif.x) >= trigerCircle || Mathf.Abs(dif.y) >= trigerCircle)
                return;

            //...if it is, then fire
            foreach (var weapon in _primaryWeapon)
            {
                weapon.Fire();
            }
        }



        static void CreateLineMaterial()
        {
            if (!_lineMaterial)
            {
                _lineMaterial = new Material("Shader \"Lines/Colored Blended\" {" +
                                             "SubShader { Pass { " +
                                             "    Blend SrcAlpha OneMinusSrcAlpha " +
                                             "    ZWrite Off Cull Off Fog { Mode Off } " +
                                             "    BindChannels {" +
                                             "      Bind \"vertex\", vertex Bind \"color\", color }" +
                                             "} } }")
                {
                    hideFlags = HideFlags.HideAndDontSave,
                    shader = { hideFlags = HideFlags.HideAndDontSave }
                };
            }
        }

        public void OnRenderObject()
        {
            CreateLineMaterial();
            // set the current material
            GL.PushMatrix();
            _lineMaterial.SetPass(0);

            GL.Begin(GL.LINES);

            Vector3 p1;
            Vector3 p2;

            //If we are moving towards an objective draw a line from 
            //the vehicle to the objective
            var nextPoint = Orders.GetCurrentOrder();
            if (nextPoint != null)
            {
                GL.Color(OrderList.CurrentLineColor);

                p1 = transform.position;
                p2 = nextPoint.GetPoint();
                GL.Vertex3(p1.x, p1.y, p1.z);
                GL.Vertex3(p2.x, p2.y, p2.z);

                GL.Color(OrderList.AttackLineColor);
                if (nextPoint.Target != null)
                {
                    p1 = targetPoint;// CalculatePursutePoint(nextPoint.Target.rigidbody.position, nextPoint.Target.rigidbody.velocity, rigidbody.position, rigidbody.velocity.magnitude);
                    p2 = transform.position;
                    GL.Vertex3(p1.x, p1.y, p1.z);
                    GL.Vertex3(p2.x, p2.y, p2.z);

                    //Draw the pursute dimond
                    DrawHelpers.DrawDimond(p1);
                }
            }

            Orders.DrawPath();

            GL.End();
            GL.PopMatrix();
        }

        private static Vector3 CalculatePursutePoint(Vector3 targetPos, Vector3 targetVel, Vector3 shooterPos, float bulletSpeed)
        {
            //var delta = p2 - p1;
            //var range = delta.magnitude;
            //var time = range/speed;

            //return p1 + v1*time;

            Vector3 totarget = targetPos - shooterPos;

            float a = Vector3.Dot(targetVel, targetVel) - (bulletSpeed * bulletSpeed);
            float b = 2 * Vector3.Dot(targetVel, totarget);
            float c = Vector3.Dot(totarget, totarget);

            float p = -b / (2 * a);
            float q = (float)Math.Sqrt((b * b) - 4 * a * c) / (2 * a);

            float t1 = p - q;
            float t2 = p + q;
            float t;

            if (t1 > t2 && t2 > 0)
            {
                t = t2;
            }
            else
            {
                t = t1;
            }

            Vector3 aimSpot = targetPos + targetVel * t;
            Vector3 bulletPath = aimSpot - shooterPos;
            //float timeToImpact = bulletPath.Length() / bulletSpeed;//speed must be in units per second
            return aimSpot;
        }

        public void AddOrders(OrderList.Order order)
        {
            Orders.AddOrder(order);
        }

        public void ClearOrders()
        {
            Orders.ClearOrder();
        }*/
    }

}
