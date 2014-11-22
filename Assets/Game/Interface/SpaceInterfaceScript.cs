using UnityEngine;

namespace Assets.Game.Interface
{
    public class SpaceInterfaceScript : MonoBehaviour
    {
        public GameObject MainPanel;
        public GameObject OptionsPanel;


        // Use this for initialization
        public void Start()
        {
            //Hide the menus to start off with
            MainPanel.SetActive(false);
            OptionsPanel.SetActive(false);
        }

        // Update is called once per frame
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleMainMenu();
            }
        }

        public void ToggleMainMenu()
        {
            MainPanel.SetActive(!MainPanel.activeSelf);

            if (OptionsPanel.activeSelf)
                OptionsPanel.SetActive(false);
        }

        public void ToggleOptionsMenu()
        {
            OptionsPanel.SetActive(!OptionsPanel.activeSelf);

            if (MainPanel.activeSelf)
                MainPanel.SetActive(false);
        }

        public void OnExit()
        {
            Debug.Log("Exiting");
            Application.Quit();
        }
    }
}

