using Controller;
using SargeUniverse.Scripts.Sound;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UIAttackScreen : UIScreen
    {
        [SerializeField] private Button _closeButton = null;

        public override void ShowScreen()
        {
            base.ShowScreen();
            UIManager.Instanse.LockScreenMove = true;
        }
        
        public override void HideScreen()
        {
            base.HideScreen();
            UIManager.Instanse.LockScreenMove = false;
        }
        
        protected override void SubscribeForEvents()
        {
            _closeButton.onClick.AddListener(CloseScreen);
        }

        protected override void UnSubscribeFromEvents()
        {
            _closeButton.onClick.RemoveListener(CloseScreen);
        }

        private void CloseScreen()
        {
            SoundSystem.Instance.PlaySound("click");
            InterfaceSwitcher.ClosePanel<UIAttackScreen>();
        }
    }
}