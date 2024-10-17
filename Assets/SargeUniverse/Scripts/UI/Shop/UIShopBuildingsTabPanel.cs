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
using UI.Elements;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts.UI.Shop
{
    public class UIShopBuildingsTabPanel : UIShopTabPanel
    {
        [SerializeField] private ShopTabsScroll _shopTabsScroll = null;
        [SerializeField] private UIBuildingCard _buildingCardPrefab = null;
        
        private readonly List<UIBuildingCard> _cards = new();

        private BuildingsConfig _buildingsConfig;
        private GameController _gameController;
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
            _playerSyncController = playerSyncController;
        }
        
        protected override void InitCards()
        {
            var buildingData = GameConfig.instance.BuildingConfig;
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Special), _content, true);
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Military), _content);
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Resources), _content);
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Defence), _content);
            InitBuildingCards(buildingData.GetBuildingDataOfType(BuildingType.Traps), _content);
        }

        protected override void UpdateCards()
        {
            foreach (var card in _cards)
            {
                var buildingId = card.GetId();
                var limit = _gameController.GetBuildingCount(buildingId);
                var hqUnlockLevel = _gameController.GetBuildingUnlockLevel(buildingId);
                
                card.UpdateCardInfo(limit, hqUnlockLevel);
            }
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
                _cards.Add(card);
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
                _uiShop.BuildingPanelCallback(buildingId);
            }
        }
        
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