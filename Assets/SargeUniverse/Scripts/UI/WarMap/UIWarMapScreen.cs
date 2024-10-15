using Assets.Scripts;
using UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace SargeUniverse.Scripts.UI
{
    public class UIWarMapScreen : UIScreen
    {
        [SerializeField] private Button _infoButton = null;
        [SerializeField] private Button _returnHomeButton = null;
        [SerializeField] private Button _warDetailsButton = null;

        private LevelLoader _levelLoader;

        [Inject]
        private void Construct(LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
        }
        
        protected override void SubscribeForEvents()
        {
            _infoButton.onClick.AddListener(OpenInfo);
            _returnHomeButton.onClick.AddListener(ReturnToBase);
            _warDetailsButton.onClick.AddListener(ShowWarDetails);
        }

        protected override void UnSubscribeFromEvents()
        {
            _infoButton.onClick.RemoveListener(OpenInfo);
            _returnHomeButton.onClick.RemoveListener(ReturnToBase);
            _warDetailsButton.onClick.RemoveListener(ShowWarDetails);
        }

        private void OpenInfo()
        {
            Debug.Log("OpenInfo");
        }

        private void ReturnToBase()
        {
            //SceneManager.LoadSceneAsync(Assets.Scripts.Constants.GAME);
            _levelLoader.LoadScene(Constants.GameScene);
        }

        private void ShowWarDetails()
        {
            InterfaceSwitcher.ShowPanel<UIWarStatusScreen>();
        }
    }
}