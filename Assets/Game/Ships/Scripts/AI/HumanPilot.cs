using Assets.Game.Weapons;
using Assets.Ships.Scripts;
using Assets.Ships.Scripts.AI;
using UnityEngine;

namespace Assets.Game.Ships.Scripts.AI
{
	[AddComponentMenu("Game/AI/Player")]
	public class HumanPilot : MonoBehaviour {

        public static HumanPilot Instance;

	    private AIPilot _ai;
		private InertialDrive _inertialDrive;
	    private Weapon[] _weapons;
		private Sensors _sensors;

	    /// <summary>
		/// Determine whether we own this ship, input type, and ensure we have a network observer.
		/// </summary>

		public void Start()
		{
            Instance = this;
		}


        public void SetShip(GameObject ship)
        {
            var ai = ship.GetComponent<AIPilot>();
            var drive = ship.GetComponent<InertialDrive>();
            var sensors = ship.GetComponent<Sensors>();
            var weapons = ship.GetComponentsInChildren<Weapon>();

            if(drive==null)
                return;

            //If we are currently driving a ship, then turn it's ai back on
            if(_ai!=null)
                _ai.enabled = true;

            //...Then turn off the new AI so that we can drive the new ship
            _ai = ai;
            _ai.enabled = false;

            _inertialDrive = drive;
            _sensors = sensors;
            _weapons = weapons;
        }

		// Update is called once per frame
        public void Update()
        {
            if (_inertialDrive != null)
            {
                _inertialDrive.YawControl = Input.GetAxis("Yaw");
                _inertialDrive.PitchControl = Input.GetAxis("Pitch");
                _inertialDrive.RollControl = Input.GetAxis("Roll");

                _inertialDrive.ThrottleControl = (Input.GetAxis("Throttle")/2 + 0.5f);

            }

            if(_weapons!=null&&_weapons.Length>0&&Input.GetKey(KeyCode.JoystickButton0))
                foreach (var weapon in _weapons)
                {
                    weapon.Fire();
                }
        }

	}
}
