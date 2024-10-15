using System;
using System.Collections.Generic;
using System.Globalization;
using Controller;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings;
using SargeUniverse.Scripts.UI.Elements;
using SargeUniverse.Scripts.Utils;
using TMPro;
using UI;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Tools = SargeUniverse.Scripts.Utils.Tools;

namespace SargeUniverse.Scripts.UI
{
    public class UIBuildingUpgradeScreen : UIScreen
    {
        private const string LockMessage = "You need to upgrade your HQ to level {0}!";

        [Header("Header")] [SerializeField] private TMP_Text _titleText = null;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Image _previewImage = null;

        [Header("Stats")] [SerializeField] private StatsSlider _hitPointsStatsSlider = null;
        [SerializeField] private StatsSlider _suppliesCapacityStatsSlider = null;
        [SerializeField] private StatsSlider _suppliesStorageStatsSlider = null;
        [SerializeField] private StatsSlider _powerCapacityStatsSlider = null;
        [SerializeField] private StatsSlider _powerStorageStatsSlider = null;
        [SerializeField] private StatsSlider _housingCapacityStatsSlider = null;
        [SerializeField] private StatsSlider _damageStatsSlider = null;

        [Header("Unlock")] [SerializeField] private GameObject _unlockLabel = null;
        [SerializeField] private GameObject _unlockScroll = null;
        [SerializeField] private Transform _content = null;
        [SerializeField] private UIUnlockCard _unlockCard = null;

        [Header("Footer")] [SerializeField] private GameObject _timeGroup = null;
        [SerializeField] private TMP_Text _upgradeTimeText = null;

        [SerializeField] private GameObject _upgradeGroup = null;
        [SerializeField] private Button _upgradeButton = null;
        [SerializeField] private TMP_Text _upgradeCostText = null;

        [SerializeField] private GameObject _lockGroup = null;
        [SerializeField] private TMP_Text _lockText = null;

        [SerializeField] private GameObject _okGroup = null;
        [SerializeField] private Button _okButton = null;

        private List<UIUnlockCard> _unlockCards = new();

        private int _upgradeCost = 0;
        private long _databaseId = 0;
        private bool _isMaxLevel = false;

        private NetworkPacket _networkPacket;
        private GameController _gameController;
        private BuildingsManager _buildingsManager;
        private PlayerSyncController _playerSyncController;

        [Inject]
        private void Construct(
            NetworkPacket networkPacket,
            GameController gameController,
            BuildingsManager buildingsManager,
            PlayerSyncController playerSyncController)
        {
            _networkPacket = networkPacket;
            _gameController = gameController;
            _buildingsManager = buildingsManager;
            _playerSyncController = playerSyncController;
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            UIManager.Instanse.SetMenuActive(false);

            _hitPointsStatsSlider.Show();
            _suppliesCapacityStatsSlider.Hide();
            _suppliesStorageStatsSlider.Hide();
            _powerCapacityStatsSlider.Hide();
            _powerStorageStatsSlider.Hide();
            _housingCapacityStatsSlider.Hide();
            _damageStatsSlider.Hide();

            _unlockLabel.SetActive(false);
            _unlockScroll.SetActive(false);
            foreach (var card in _unlockCards)
            {
                card.Dispose();
            }
            _unlockCards.Clear();

            InitScreenInfo();
        }

        protected override void SubscribeForEvents()
        {
            _closeButton.onClick.AddListener(CloseBuildingUpgradeScreen);
            _upgradeButton.onClick.AddListener(UpgradeBuilding);
            _okButton.onClick.AddListener(CloseBuildingUpgradeScreen);
        }

        protected override void UnSubscribeFromEvents()
        {
            _closeButton.onClick.RemoveListener(CloseBuildingUpgradeScreen);
            _upgradeButton.onClick.RemoveListener(UpgradeBuilding);
            _okButton.onClick.RemoveListener(CloseBuildingUpgradeScreen);
        }

        private void InitScreenInfo()
        {
            if (BuildingsManager.Instanse.selectedBuilding == null)
            {
                HideScreen();
            }

            var building = BuildingsManager.Instanse.selectedBuilding;
            var limit = _gameController.GetBuildingCount(building.buildingId);
            _databaseId = building.BuildingData.databaseID;

            var canUpgrade = building.BuildingData.level < limit.maxLevel;
            _timeGroup.SetActive(canUpgrade);
            _lockGroup.SetActive(!canUpgrade);
            _upgradeGroup.SetActive(canUpgrade);
            _okGroup.SetActive(!canUpgrade);

            var sBuilding = _gameController.GetServerBuilding(building.buildingId,
                canUpgrade ? building.BuildingData.level + 1 : building.BuildingData.level);

            var buildingMaxLevel = _gameController.GetBuildingMaxLevel(building.buildingId);
            var buildingLevel = Math.Min(buildingMaxLevel, building.BuildingData.level + 1);

            _titleText.text = sBuilding.name + "(lvl " + buildingLevel + ")";
            _previewImage.sprite = GameConfig.instance.BuildingConfig
                .GetBuildingData(building.buildingId)
                .GetBuildingSprite(buildingLevel);

            if (canUpgrade)
            {
                _upgradeCost = sBuilding.requiredSupplies;
                _upgradeTimeText.text = Tools.SecondsToTimeFormat(sBuilding.buildTime);
                _upgradeCostText.text = TextUtils.NumberToTextWithSeparator(sBuilding.requiredSupplies);
            }
            else
            {
                var unlockLevel = _gameController.GetBuildingUnlockLevel(building.buildingId, buildingLevel);
                _lockText.text = unlockLevel < 0 ? "Max level reached" : string.Format(LockMessage, unlockLevel);
            }

            _unlockLabel.SetActive(building.buildingId == BuildingID.hq);
            _unlockScroll.SetActive(building.buildingId == BuildingID.hq);

            _hitPointsStatsSlider.SetValues(building.BuildingData.health,
                sBuilding.health - building.BuildingData.health
            );

            UpdateBuildingStats(building, sBuilding);
        }

        private void UpdateBuildingStats(Building building, ServerBuilding sBuilding)
        {
            switch (building.buildingId)
            {
                case BuildingID.hq:
                    _suppliesStorageStatsSlider.SetValues(
                        building.BuildingData.suppliesCapacity,
                        sBuilding.suppliesCapacity - building.BuildingData.suppliesCapacity
                    );
                    _suppliesStorageStatsSlider.Show();

                    _powerStorageStatsSlider.SetValues(
                        building.BuildingData.powerCapacity,
                        sBuilding.powerCapacity - building.BuildingData.powerCapacity
                    );
                    _powerStorageStatsSlider.Show();

                    GenerateUnlocks();
                    break;
                case BuildingID.supplydrop:
                    _suppliesCapacityStatsSlider.SetValues(
                        building.BuildingData.speed,
                        sBuilding.speed - building.BuildingData.speed
                    );
                    _suppliesCapacityStatsSlider.Show();

                    _suppliesStorageStatsSlider.SetValues(
                        building.BuildingData.suppliesCapacity,
                        sBuilding.suppliesCapacity - building.BuildingData.suppliesCapacity
                    );
                    _suppliesStorageStatsSlider.Show();
                    break;
                case BuildingID.supplyvault:
                    _suppliesStorageStatsSlider.SetValues(
                        building.BuildingData.suppliesCapacity,
                        sBuilding.suppliesCapacity - building.BuildingData.suppliesCapacity
                    );
                    _suppliesStorageStatsSlider.Show();
                    break;
                case BuildingID.powerplant:
                    _powerCapacityStatsSlider.SetValues(
                        building.BuildingData.speed,
                        sBuilding.speed - building.BuildingData.speed
                    );
                    _powerCapacityStatsSlider.Show();

                    _powerStorageStatsSlider.SetValues(
                        building.BuildingData.powerCapacity,
                        sBuilding.powerCapacity - building.BuildingData.powerCapacity
                    );
                    _powerStorageStatsSlider.Show();
                    break;
                case BuildingID.powerstorage:
                    _powerStorageStatsSlider.SetValues(
                        building.BuildingData.powerCapacity,
                        sBuilding.powerCapacity - building.BuildingData.powerCapacity
                    );
                    _powerStorageStatsSlider.Show();
                    break;
                case BuildingID.builderstation:
                    break;
                case BuildingID.trainingcamp:
                    _housingCapacityStatsSlider.SetValues(building.BuildingData.capacity,
                        sBuilding.capacity - building.BuildingData.capacity
                    );
                    _housingCapacityStatsSlider.Show();
                    break;
                case BuildingID.barracks:
                    break;
                case BuildingID.armoury:
                    break;
                case BuildingID.wall:
                    break;
                case BuildingID.watchtower:
                    _damageStatsSlider.SetValues(
                        building.BuildingData.damage,
                        sBuilding.damage - building.BuildingData.damage
                    );
                    _damageStatsSlider.Show();
                    break;
                case BuildingID.rocketturret:
                    _damageStatsSlider.SetValues(
                        building.BuildingData.damage,
                        sBuilding.damage - building.BuildingData.damage
                    );
                    _damageStatsSlider.Show();
                    break;
                case BuildingID.motor:
                    _damageStatsSlider.SetValues(
                        building.BuildingData.damage,
                        sBuilding.damage - building.BuildingData.damage
                    );
                    _damageStatsSlider.Show();
                    break;
                case BuildingID.mine:
                    _damageStatsSlider.SetValues(
                        building.BuildingData.damage,
                        sBuilding.damage - building.BuildingData.damage
                    );
                    _damageStatsSlider.Show();
                    break;
                case BuildingID.trapfloor:
                    break;
                case BuildingID.sargelocation:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void GenerateUnlocks()
        {
            var currentLimits = _gameController.GetBuildingLimits(BuildingsManager.Instanse.GetHqLevel());
            var nextLevelLimits = _gameController.GetBuildingLimits(BuildingsManager.Instanse.GetHqLevel() + 1);

            foreach (var limit in nextLevelLimits)
            {
                var currentLimit = currentLimits.Find(l => l.buildingId == limit.buildingId);
                if (currentLimit == null)
                {
                    var card = Instantiate<UIUnlockCard>(_unlockCard, _content);
                    card.InitCard(Enum.Parse<BuildingID>(limit.buildingId), limit.count, true);
                    _unlockCards.Add(card);
                }
                else if (limit.count > currentLimit.count)
                {
                    var card = Instantiate<UIUnlockCard>(_unlockCard, _content);
                    card.InitCard(Enum.Parse<BuildingID>(limit.buildingId), limit.count - currentLimit.count);
                    _unlockCards.Add(card);
                }
            }
        }

        private void UpgradeBuilding()
        {
            // TODO: check builders available
            var availableBuilders = 1;
            if (availableBuilders == 0)
            {
                UIManager.Instanse.ShowMessage("No Builder Available");
            }
            else if (_playerSyncController.Supplies.GetValue() < _upgradeCost)
            {
                UIManager.Instanse.ShowMessage("Not enough supplies");
            }
            else if (_playerSyncController.Workers.GetValue() == 0)
            {
                UIManager.Instanse.ShowMessage("No free worker");
            }
            else
            {
                _networkPacket.SendUpgradeRequest(_databaseId);
                CloseBuildingUpgradeScreen();
            }
        }

        private void CloseBuildingUpgradeScreen()
        {
            _buildingsManager.DeselectBuilding();
            UIManager.Instanse.SetMenuActive(true);
            InterfaceSwitcher.SwitchPanel<UIMain>();
        }
    }
}