using SargeUniverse.Scripts.UI.Elements;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UIArmouryScreen : UIScreen
    {
        [SerializeField] private Button _closeButton = null;

        [SerializeField] private ArmouryUnitCard _cardPrefab = null;
        
        protected override void SubscribeForEvents()
        {
            _closeButton.onClick.AddListener(CloseButtonClick);
        }

        protected override void UnSubscribeFromEvents()
        {
            _closeButton.onClick.RemoveListener(CloseButtonClick);
        }

        private void CloseButtonClick()
        {
            HideScreen();
        }

        private void InitPanel()
        {
            
        }
    }
}