using System.Collections.Generic;
using System.Linq;
using Config;
using Config.Building;
using Controller;
using Enums;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Model.UI;
using SargeUniverse.Scripts.Sound;
using SargeUniverse.Scripts.Utils;
using TMPro;
using UI.Base;
using UI.Elements;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Button = UnityEngine.UI.Button;

namespace UI
{
    public class UIShop : UIScreen
    {
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private ShopTabsScroll _shopTabsScroll = null;
        
        [SerializeField] private Transform _content = null;
        
        [SerializeField] private UIBuildingCard _buildingCardPrefab = null;
        
        [Header("Bottom Group")]
        [SerializeField] private Slider _supplySlider = null;
        [SerializeField] private TMP_Text _supplyText = null;
        [SerializeField] private Slider _powerSlider = null;
        [SerializeField] private TMP_Text _powerText = null;
        [SerializeField] private TMP_Text _gemsText = null;

        private List<UIBuildingCard> cards = new();

        private BuildingsConfig _buildingsConfig;
        private GameController _gameController;
        private BuildingsManager _buildingsManager;
        private PlayerSyncController _playerSyncController;

        [Inject]
        private void Construct(
            BuildingsConfig buildingsConfig, 
            GameController gameController,
            BuildingsManager buildingsManager,
            PlayerSyncController playerSyncController)
        {
            _buildingsConfig = buildingsConfig;
            _gameController = gameController;
            _buildingsManager = buildingsManager;
            _playerSyncController = playerSyncController;
        }
        
        private void Start()
        {
            base.Start();
            InitBuildingCards();
        }
        
        public override void ShowScreen()
        {
            base.ShowScreen();
            UIManager.Instanse.LockScreenMove = true;
            _buildingsManager.DeselectBuilding();
            
            foreach (var card in cards)
            {
                var buildingId = card.GetId();
                var limit = _gameController.GetBuildingCount(buildingId);
                var hqUnlockLevel = _gameController.GetBuildingUnlockLevel(buildingId);
                
                card.UpdateCardInfo(limit, hqUnlockLevel);
            }
        }

        public override void HideScreen()
        {
            base.HideScreen();
            UIManager.Instanse.LockScreenMove = false;
        }
        
        protected override void SubscribeForEvents()
        {
            _playerSyncController.MaxSupplies.SubscribeForUpdate(UpdateSuppliesInfo);
            _playerSyncController.Supplies.SubscribeForUpdate(UpdateSuppliesInfo);
            _playerSyncController.MaxPower.SubscribeForUpdate(UpdatePowerInfo);
            _playerSyncController.Power.SubscribeForUpdate(UpdatePowerInfo);
            _playerSyncController.Gems.SubscribeForUpdate(UpdateGemsInfo);
            
            _closeButton.onClick.AddListener(OnCloseButtonClick);
        }

        protected override void UnSubscribeFromEvents()
        {
            _playerSyncController.MaxSupplies.UnSubscribeFromUpdate(UpdateSuppliesInfo);
            _playerSyncController.Supplies.UnSubscribeFromUpdate(UpdateSuppliesInfo);
            _playerSyncController.MaxPower.UnSubscribeFromUpdate(UpdatePowerInfo);
            _playerSyncController.Power.UnSubscribeFromUpdate(UpdatePowerInfo);
            _playerSyncController.Gems.UnSubscribeFromUpdate(UpdateGemsInfo);
            
            _closeButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        private void OnCloseButtonClick()
        {
            SoundSystem.Instance.PlaySound("click");
            InterfaceSwitcher.SwitchPanel<UIMain>();
            HideScreen();
        }

        private void InitBuildingCards()
        {
            var buildingData = GameConfig.instance.BuildingConfig;
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Special), _content, true);
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Military), _content);
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Resources), _content);
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Defence), _content);
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Traps), _content);
        }

        private void InitBuildingCards(List<BuildingData> buildings, Transform content, bool skipBind = false)
        {
            RectTransform rect = null;
            foreach (var building in buildings)
            {
                var cardData = GetCardData(building.buildingId);
                var card = Instantiate(_buildingCardPrefab, content);
                card.Init(cardData, TryPlaceBuilding);

                if (!skipBind && rect == null)
                {
                    rect = card.GetComponent<RectTransform>();
                    _shopTabsScroll.AddScrollPosition(rect);
                }
                cards.Add(card);
            }
        }

        private void TryPlaceBuilding(BuildingID buildingId, int buildingCost)
        {
            SoundSystem.Instance.PlaySound("click");
            if (buildingId == BuildingID.builderstation && _playerSyncController.Gems.GetValue() < buildingCost)
            {
                UIManager.Instanse.ShowMessage("Not enough gems");
            }
            else if (_playerSyncController.Supplies.GetValue() < buildingCost)
            {
                UIManager.Instanse.ShowMessage("Not enough supplies");
            }
            else if (_playerSyncController.Workers.GetValue() == 0)
            {
                UIManager.Instanse.ShowMessage("No free worker");
            }
            else
            {
                OnCloseButtonClick();
                _buildingsManager.CreateBuilding(buildingId);
                InterfaceSwitcher.SwitchModalPanel<UiBuild>();
            }
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

        private void UpdateGemsInfo()
        {
            _gemsText.text = _playerSyncController.Gems.GetValue().ToString();
        }

        // -----
        
        private ShopCardData GetCardData(BuildingID buildingId)
        {
            var cardData = new ShopCardData();
            var buildingData = _buildingsConfig.GetBuildingData(buildingId);
            var serverBuilding = _gameController.GetServerBuilding(buildingId);

            cardData.buildingId = buildingId;
            cardData.name = serverBuilding.name;
            cardData.image = buildingData.playerLevelSprites.First();
            cardData.buildTime = Tools.SecondsToTimeFormat(serverBuilding.buildTime);
            cardData.buildCost = serverBuilding.requiredSupplies;

            cardData.limit = _gameController.GetBuildingCount(buildingId);
            cardData.hqUnlockLevel = _gameController.GetBuildingUnlockLevel(buildingId);
            
            return cardData;
        }
    }
}