using Controller;
using SargeUniverse.Scripts;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.UI;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Elements
{
    public class UIBuildingOptions : UIScreen
    {
        private const int Minute = 60;
        private const int Hour = 3600;
        private const int Day = 86400;
        private const int Week = 604800;
        private const int MinutePay = 1;
        private const int HourPay = 20;
        private const int DayPay = 260;
        private const int WeekPay = 1000;
        
        [SerializeField] private Button _buildingInfoButton = null;
        [SerializeField] private Button _buildingUpgradeButton = null;
        [SerializeField] private Button _instantBuildButton = null;
        [SerializeField] private TMP_Text _instantBuildPriceText = null;
        [SerializeField] private Button _trainUnitsButton = null;

        private BuildingsManager _buildingsManager;
        private PlayerSyncController _playerSyncController;
        
        private int _instantBuildCost = 0;
        
        [Inject]
        private void Construct(
            BuildingsManager buildingsManager,
            PlayerSyncController playerSyncController)
        {
            _buildingsManager = buildingsManager;
            _playerSyncController = playerSyncController;
        }
        
        protected override void SubscribeForEvents()
        {
            _buildingInfoButton.onClick.AddListener(ShowBuildingInfoScreen);
            _buildingUpgradeButton.onClick.AddListener(ShowBuildingUpgradeScreen);
            _instantBuildButton.onClick.AddListener(InstantBuildBuilding);
            _trainUnitsButton.onClick.AddListener(ShowTrainUnits);
        }

        protected override void UnSubscribeFromEvents()
        {
            _buildingInfoButton.onClick.RemoveListener(ShowBuildingInfoScreen);
            _buildingUpgradeButton.onClick.RemoveListener(ShowBuildingUpgradeScreen);
            _instantBuildButton.onClick.RemoveListener(InstantBuildBuilding);
            _trainUnitsButton.onClick.RemoveListener(ShowTrainUnits);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();

            var building = BuildingsManager.Instanse.selectedBuilding;
            _buildingInfoButton.gameObject.SetActive(building.BuildingData.isConstructing == false);
            _buildingUpgradeButton.gameObject.SetActive(building.BuildingData.isConstructing == false);
            _instantBuildButton.gameObject.SetActive(building.BuildingData.isConstructing);
            if (building.BuildingData.isConstructing)
            {
                CalculateInstantBuildPrice();
            }
            _trainUnitsButton.gameObject.SetActive(
                building.BuildingData.isConstructing == false && 
                building.BuildingData.id is BuildingID.barracks or BuildingID.trainingcamp
            );
        }

        public override void HideScreen()
        {
            base.HideScreen();
            InterfaceSwitcher.CloseModalScreen();
        }

        private void Update()
        {
            CalculateInstantBuildPrice();
        }

        private void CalculateInstantBuildPrice()
        {
            if (_instantBuildButton.gameObject.activeInHierarchy)
            {
                var time = BuildingsManager.Instanse.selectedBuilding.BuildingData.buildTime;
                _instantBuildCost = time switch
                {
                    < Minute => 1,
                    < Hour => (HourPay - MinutePay) / (Hour - Minute) * (time - Minute) + MinutePay,
                    < Day => (DayPay - HourPay) / (Day - Hour) * (time - Hour) + HourPay,
                    _ => (WeekPay - DayPay) / (Week - Day) * (time - Day) + DayPay
                };
                _instantBuildPriceText.text = _instantBuildCost.ToString();
            }
        }

        private void ShowBuildingInfoScreen()
        {
            HideScreen();
            InterfaceSwitcher.ShowPanel<UIBuildingInfoScreen>();
        }

        private void ShowBuildingUpgradeScreen()
        {
            HideScreen();
            InterfaceSwitcher.ShowPanel<UIBuildingUpgradeScreen>();
        }

        private void InstantBuildBuilding()
        {
            if (_playerSyncController.Gems.GetValue() < _instantBuildCost)
            {
                UIManager.Instanse.ShowMessage("No Gems");
            }
            else
            {
                _buildingsManager.InstantBuildBuilding();
                _buildingsManager.DeselectBuilding();
            }
        }

        private void ShowTrainUnits()
        {
            HideScreen();
            InterfaceSwitcher.ShowPanel<UIArmyScreen>();
        }
    }
}