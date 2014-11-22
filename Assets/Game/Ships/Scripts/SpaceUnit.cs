using System.Collections.Generic;
using UnityEngine;

namespace Assets.Game.Ships.Scripts
{
    public class SpaceUnit : DestroyableObject
    {
        public static List<SpaceUnit> Units = new List<SpaceUnit>();
        private static int _unitCounter;

        public static SpaceUnit GetNextUnit()
        {
            if (Units.Count == 0)
                return null;

            _unitCounter = (_unitCounter + 1 >= Units.Count) ? 0 : _unitCounter + 1;
            return Units[_unitCounter];
        }


        public GameObject InteriorModel { get; private set; }
        public GameObject ExteriorModel { get; private set; }

        public static void AddUnit(SpaceUnit unit)
        {
            Units.Add(unit);
        }
	
        public void Start()
        {
            //
            // Add this unit to the global list
            //
            AddUnit(this);

            //
            // Grab the interior and exterior models (if it has them)
            //
            var interior = transform.FindChild("Interior");
            if (interior != null)
                InteriorModel = interior.gameObject;

            var exterior = transform.FindChild("Exterior");
            if (exterior != null)
                ExteriorModel = exterior.gameObject;
        }

        public void ShowInterior() {
            if(InteriorModel!=null)
                InteriorModel.SetActive(true);

            if (ExteriorModel != null)
                ExteriorModel.SetActive(false);
        }

        public void ShowExterior()
        {
            if (InteriorModel != null)
                InteriorModel.SetActive(false);

            if (ExteriorModel != null)
                ExteriorModel.SetActive(true);
        }

        // Update is called once per frame
        public override void Update () {
            base.Update();
        }
    }
}
