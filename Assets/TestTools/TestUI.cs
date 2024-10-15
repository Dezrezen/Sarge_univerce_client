using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TestTools
{
    public class TestUI : MonoBehaviour
    {
        public GameObject UI;
        
        public Button QuitButton;
        public Button MainBuildings;
        public Button ResourcesBuildings;
        public Button DefenceBuildings;

        public GameObject MainBuildingsTab;
        public GameObject ResourcesBuildingsTab;
        public GameObject DefenceBuildingsTab;

        private bool _hideUI = false;
        
        private void Start()
        {
            QuitButton.onClick.AddListener(OnQuitClick);
            MainBuildings.onClick.AddListener(OnMainBuildingsClick);
            ResourcesBuildings.onClick.AddListener(OnResourcesBuildingsClick);
            DefenceBuildings.onClick.AddListener(OnDefenceBuildingsClick);

            OnMainBuildingsClick();
        }
        
        private void OnDestroy()
        {
            QuitButton.onClick.RemoveAllListeners();
            MainBuildings.onClick.RemoveAllListeners();
            ResourcesBuildings.onClick.RemoveAllListeners();
            DefenceBuildings.onClick.RemoveAllListeners();
        }

        private void Update()
        {
            if (Keyboard.current[Key.H].wasPressedThisFrame)
            {
                _hideUI = !_hideUI;
                UI.SetActive(!_hideUI);
            }
        }

        private void OnQuitClick()
        {
            Application.Quit();
        }

        private void OnMainBuildingsClick()
        {
            MainBuildingsTab.SetActive(true);
            ResourcesBuildingsTab.SetActive(false);
            DefenceBuildingsTab.SetActive(false);
        }

        private void OnResourcesBuildingsClick()
        {
            MainBuildingsTab.SetActive(false);
            ResourcesBuildingsTab.SetActive(true);
            DefenceBuildingsTab.SetActive(false);
        }
        
        private void OnDefenceBuildingsClick()
        {
            MainBuildingsTab.SetActive(false);
            ResourcesBuildingsTab.SetActive(false);
            DefenceBuildingsTab.SetActive(true);
        }
    }
}