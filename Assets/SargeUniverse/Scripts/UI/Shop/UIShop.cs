using System.Collections.Generic;
using System.Linq;
using CityBuildingKit.Scripts.UI;
using Controller;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Sound;
using TMPro;
using UI.Base;
using UI.Elements;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Button = UnityEngine.UI.Button;

namespace SargeUniverse.Scripts.UI.Shop
{
    public class UIShop : UIScreen
    {
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private ShopTabsGroup _shopTabsGroup = null;
        [SerializeField] private List<UIShopTabPanel> _shopTabPanels = new();
        
        [Header("Bottom Group")]
        [SerializeField] private Slider _supplySlider = null;
        [SerializeField] private TMP_Text _supplyText = null;
        [SerializeField] private Slider _powerSlider = null;
        [SerializeField] private TMP_Text _powerText = null;
        [SerializeField] private GameObject _tachyonGroup = null;
        [SerializeField] private Slider _tachyonSlider = null;
        [SerializeField] private TMP_Text _tachyonText = null;
        [SerializeField] private TMP_Text _gemsText = null;
        
        private BuildingsManager _buildingsManager;
        private PlayerSyncController _playerSyncController;
        private NetworkPacket _networkPacket;

        [Inject]
        private void Construct(
            BuildingsManager buildingsManager, 
            PlayerSyncController playerSyncController,
            NetworkPacket networkPacket)
        {
            _buildingsManager = buildingsManager;
            _playerSyncController = playerSyncController;
            _networkPacket = networkPacket;
        }

        private void Awake()
        {
            foreach (var tab in _shopTabPanels)
            {
                tab.Init(this);
            }
        }
        
        public override void ShowScreen()
        {
            base.ShowScreen();
            UIManager.Instanse.LockScreenMove = true;
            _buildingsManager.DeselectBuilding();
            
            _shopTabsGroup.GetActivePanel().ShopPanel();
        }

        public override void HideScreen()
        {
            base.HideScreen();
            UIManager.Instanse.LockScreenMove = false;
            
            _shopTabsGroup.GetActivePanel().HidePanel();
        }
        
        protected override void SubscribeForEvents()
        {
            _playerSyncController.MaxSupplies.SubscribeForUpdate(UpdateSuppliesInfo);
            _playerSyncController.Supplies.SubscribeForUpdate(UpdateSuppliesInfo);
            _playerSyncController.MaxPower.SubscribeForUpdate(UpdatePowerInfo);
            _playerSyncController.Power.SubscribeForUpdate(UpdatePowerInfo);
            _playerSyncController.MaxEnergy.SubscribeForUpdate(UpdateTachyonInfo);
            _playerSyncController.Energy.SubscribeForUpdate(UpdateTachyonInfo);
            _playerSyncController.Gems.SubscribeForUpdate(UpdateGemsInfo);
            
            _closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        protected override void UnSubscribeFromEvents()
        {
            _playerSyncController.MaxSupplies.UnSubscribeFromUpdate(UpdateSuppliesInfo);
            _playerSyncController.Supplies.UnSubscribeFromUpdate(UpdateSuppliesInfo);
            _playerSyncController.MaxPower.UnSubscribeFromUpdate(UpdatePowerInfo);
            _playerSyncController.Power.UnSubscribeFromUpdate(UpdatePowerInfo);
            _playerSyncController.MaxEnergy.UnSubscribeFromUpdate(UpdateTachyonInfo);
            _playerSyncController.Energy.UnSubscribeFromUpdate(UpdateTachyonInfo);
            _playerSyncController.Gems.UnSubscribeFromUpdate(UpdateGemsInfo);
            
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        private void OnCloseButtonClick()
        {
            SoundSystem.Instance.PlaySound("click");
            InterfaceSwitcher.SwitchPanel<UIMain>();
            HideScreen();
        }
        
        private void UpdateSuppliesInfo()
        {
            _supplySlider.value = 1f * 
                                  _playerSyncController.Supplies.GetValue() / 
                                  _playerSyncController.MaxSupplies.GetValue();
            _supplyText.text = _playerSyncController.Supplies.GetValue().ToString();
        }

        private void UpdatePowerInfo()
        {
            _powerSlider.value = 1f * 
                                 _playerSyncController.Power.GetValue() / 
                                 _playerSyncController.MaxPower.GetValue();
            _powerText.text = _playerSyncController.Power.GetValue().ToString();
        }
        
        private void UpdateTachyonInfo()
        {
            _tachyonGroup.SetActive(_playerSyncController.MaxEnergy.GetValue() > 0);
            _tachyonSlider.value = 1f * 
                                 _playerSyncController.Energy.GetValue() / 
                                 _playerSyncController.MaxEnergy.GetValue();
            _tachyonText.text = _playerSyncController.Energy.GetValue().ToString();
        }

        private void UpdateGemsInfo()
        {
            _gemsText.text = _playerSyncController.Gems.GetValue().ToString();
        }
        
        // --- Panels callbacks ---

        public void BuildingPanelCallback(BuildingID buildingId)
        {
            OnCloseButtonClick();
            _buildingsManager.CreateBuilding(buildingId);
            InterfaceSwitcher.SwitchModalPanel<UiBuild>();
        }

        public void BuyItemCallback(string id, float price)
        {
            _networkPacket.SendBuyPacketRequest(id);
        }
    }
}