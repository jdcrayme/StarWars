using UnityEngine;

namespace Assets.Game.GameModes
{
    public class ModeSwitcher : MonoBehaviour
    {
        public GameMode [] GameModes;

        public KeyCode SwitchKey;

        private int _currentMode;
        private GameObject _currentGameModeLogic;
        private Managers.CameraManager.CameraManager _cameraManager;


        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            _cameraManager = GetComponent<Managers.CameraManager.CameraManager>();

            if(GameModes.Length<1)
                Debug.LogError("Mode Switcher must have at least one game mode defined");

            SetMode(GameModes[0]); //Initialize the default game mode
        }

        /// <summary>
        /// Update
        /// </summary>
        public void Update()
        {
            //If the user is not pressing the switch key then do nothing
            if (!Input.GetKeyDown(SwitchKey)) return;
            
            //...otherwise select the next mode in the array with wrapping
            _currentMode = _currentMode >= GameModes.Length - 1 ? 0 : _currentMode + 1;
            SetMode(GameModes[_currentMode]);
        }

        /// <summary>
        /// Sets a new game mode.
        /// Creates GameObjects containing mode specific logic. Delets GameObjects coresponding to the old mode
        /// </summary>
        /// <param name="gameMode">The new mode prefab</param>
        private void SetMode(GameMode gameMode)
        {
            if (gameMode == null)
            {
                Debug.LogWarning("Mode Switcher attempted to set an invalid game mode");
                return;
            }

            if (_currentGameModeLogic != null)
            {
                _currentGameModeLogic.GetComponent<GameMode>().Shutdown();
                Destroy(_currentGameModeLogic);
            }
            _currentGameModeLogic = (GameObject)Instantiate(gameMode.gameObject);
            _currentGameModeLogic.transform.parent = transform;
            _currentGameModeLogic.GetComponent<GameMode>().Initialize();
        }
    }
}
