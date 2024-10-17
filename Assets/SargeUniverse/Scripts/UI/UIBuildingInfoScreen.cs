using System;
using System.Globalization;
using CityBuildingKit.Scripts.UI;
using Controller;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.UI.Elements;
using TMPro;
using UI;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SargeUniverse.Scripts.UI
{
    public class UIBuildingInfoScreen : UIScreen
    {
        [Header("Header")]
        [SerializeField] private TMP_Text _titleText = null;
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Image _previewImage = null;

        [Header("Stats")]
        [SerializeField] private StatsSlider _hitPointsStatsSlider = null;
        [SerializeField] private StatsSlider _suppliesCapacityStatsSlider = null;
        [SerializeField] private StatsSlider _suppliesStorageStatsSlider = null;
        [SerializeField] private StatsSlider _powerCapacityStatsSlider = null;
        [SerializeField] private StatsSlider _powerStorageStatsSlider = null;
        [SerializeField] private StatsSlider _housingCapacityStatsSlider = null;
        [SerializeField] private StatsSlider _damageStatsSlider = null;
        
        [Header("Info")]
        [SerializeField] private TMP_Text _buildingInfoText = null;

        private GameController _gameController;
        private BuildingsManager _buildingsManager;

        [Inject]
        private void Construct(GameController gameController, BuildingsManager buildingsManager)
        {
            _gameController = gameController;
            _buildingsManager = buildingsManager;
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
            
            InitScreenInfo();
        }
        
        protected override void SubscribeForEvents()
        {
            _closeButton.onClick.AddListener(CloseBuildingInfoScreen);
        }

        protected override void UnSubscribeFromEvents()
        {
            _closeButton.onClick.RemoveListener(CloseBuildingInfoScreen);
        }

        private void InitScreenInfo()
        {
            if (BuildingsManager.Instanse.selectedBuilding == null)
            {
                HideScreen();
            }
            
            var building = BuildingsManager.Instanse.selectedBuilding;
            var serverBuilding = _gameController.GetServerBuilding(building.buildingId, building.BuildingData.level);
            _titleText.text = serverBuilding.name + "(lvl " + building.BuildingData.level + ")";
            _previewImage.sprite = GameConfig.instance.BuildingConfig
                .GetBuildingData(building.buildingId)
                .GetBuildingSprite(building.BuildingData.level);
            
            _hitPointsStatsSlider.SetValues(building.BuildingData.health);
            
            switch (building.buildingId)
            {
                case BuildingID.hq:
                    _suppliesStorageStatsSlider.SetValues(building.BuildingData.suppliesCapacity);
                    _suppliesStorageStatsSlider.Show();
                    
                    _powerStorageStatsSlider.SetValues(building.BuildingData.powerCapacity);
                    _powerStorageStatsSlider.Show();
                    break;
                case BuildingID.supplydrop:
                    _suppliesCapacityStatsSlider.SetValues(building.BuildingData.speed);
                    _suppliesCapacityStatsSlider.Show();

                    _suppliesStorageStatsSlider.SetValues(building.BuildingData.suppliesCapacity);
                    _suppliesStorageStatsSlider.Show();
                    break;
                case BuildingID.supplyvault:
                    _suppliesStorageStatsSlider.SetValues(building.BuildingData.suppliesCapacity);
                    _suppliesStorageStatsSlider.Show();
                    break;
                case BuildingID.powerplant:
                    _powerCapacityStatsSlider.SetValues(building.BuildingData.speed);
                    _powerCapacityStatsSlider.Show();

                    _powerStorageStatsSlider.SetValues(building.BuildingData.powerCapacity);
                    _powerStorageStatsSlider.Show();
                    break;
                case BuildingID.powerstorage:
                    _powerStorageStatsSlider.SetValues(building.BuildingData.powerCapacity);
                    _powerStorageStatsSlider.Show();
                    break;
                case BuildingID.builderstation:
                    break;
                case BuildingID.trainingcamp:
                    _housingCapacityStatsSlider.SetValues(building.BuildingData.capacity);
                    _housingCapacityStatsSlider.Show();
                    break;
                case BuildingID.barracks:
                    break;
                case BuildingID.armoury:
                    break;
                case BuildingID.wall:
                    break;
                case BuildingID.watchtower:
                    _damageStatsSlider.SetValues(building.BuildingData.damage);
                    _damageStatsSlider.Show();
                    break;
                case BuildingID.rocketturret:
                    _damageStatsSlider.SetValues(building.BuildingData.damage);
                    _damageStatsSlider.Show();
                    break;
                case BuildingID.motor:
                    _damageStatsSlider.SetValues(building.BuildingData.damage);
                    _damageStatsSlider.Show();
                    break;
                case BuildingID.mine:
                    _damageStatsSlider.SetValues(building.BuildingData.damage);
                    _damageStatsSlider.Show();
                    break;
                case BuildingID.trapfloor:
                    break;
                case BuildingID.sargelocation:
                    break;
            }
        }

        private void CloseBuildingInfoScreen()
        {
            _buildingsManager.DeselectBuilding();
            UIManager.Instanse.SetMenuActive(true);
            InterfaceSwitcher.SwitchPanel<UIMain>();
        }
    }
}