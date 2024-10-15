using SargeUniverse.Scripts.Controller;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class UIWarBaseArmyScreen : UIScreen
    {
        [SerializeField] private TMP_Text _baseName = null;
        [SerializeField] private Button _donateButton = null;
        [SerializeField] private Button _scoutButton = null;
        [SerializeField] private TMP_Text _sliderValue = null;
        [SerializeField] private Slider _troopsSlider = null;

        public override void ShowScreen()
        {
            base.ShowScreen();

            if (WarMapManager.Instanse.selectedWarBase == null)
            {
                HideScreen();
                return;
            }

            _baseName.text = WarMapManager.Instanse.selectedWarBase.GetBaseName();
        }
        
        protected override void SubscribeForEvents()
        {
            _donateButton.onClick.AddListener(OnDonate);
            _scoutButton.onClick.AddListener(OnScout);
        }

        protected override void UnSubscribeFromEvents()
        {
            _donateButton.onClick.RemoveListener(OnDonate);
            _scoutButton.onClick.RemoveListener(OnScout);
        }

        private void OnScout()
        {
            Debug.Log("OnScout");
        }

        private void OnDonate()
        {
            Debug.Log("OnDonate");
        }
    }
}