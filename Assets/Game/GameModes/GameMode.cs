using UnityEngine;

namespace Assets.Game.GameModes
{
    public abstract class GameMode : MonoBehaviour
    {

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        public abstract void Initialize();
        public abstract void Shutdown();
    }
}
