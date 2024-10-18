using System;
using System.Collections.Generic;
using CityBuildingKit.Scripts.UI;
using SargeUniverse.Scripts;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Environment.Buildings;
using SargeUniverse.Scripts.Model.UI;
using SargeUniverse.Scripts.Sound;
using SargeUniverse.Scripts.UI;
using UI;
using UI.Elements;
using UnityEngine;
using Zenject;

namespace Controller
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] private InterfaceSwitcher _interfaceSwitcher;

        [Header("Message Window")] [SerializeField]
        private UIMessageBox _messageBox;

        [Header("Buttons")] [SerializeField] private Transform _buttonsParent = null;
        [SerializeField] private UICollectButton _collectSuppliesButton = null;
        [SerializeField] private UICollectButton _collectPowerButton = null;
        [SerializeField] private UIBar _barBuild = null;

        public bool mainMenuActive { get; private set; }
        public static UIManager Instanse { get; private set; } = null;

        public bool LockScreenMove = false;

        private NetworkPacket _networkPacket;
        private GameController _gameController;
        private PlayerSyncController _playerSyncController;

        [Inject]
        private void Construct(
            NetworkPacket networkPacket,
            GameController gameController,
            PlayerSyncController playerSyncController)
        {
            _networkPacket = networkPacket;
            _gameController = gameController;
            _playerSyncController = playerSyncController;
        }

        private void Awake()
        {
            Instanse ??= this;
        }

        private void Start()
        {
            SoundSystem.Instance.PlayMusic("main_theme");
            _gameController.InitGameScene();
        }

        private void OnDestroy()
        {
            SoundSystem.Instance.StopMusic();
            Instanse = null;
        }

        public void SetMenuActive(bool flag)
        {
            mainMenuActive = flag;
        }

        public void ShowLogInScreen()
        {
            _interfaceSwitcher.SwitchModalPanel<UILogInScreen>();
        }

        public void ShowMainUI()
        {
            _interfaceSwitcher.SwitchPanel<UIMain>();
            LockScreenMove = false;
        }

        public void ShowBuildingInfoScreen()
        {
            _interfaceSwitcher.ShowPanel<UIBuildingOptions>();
        }

        public void HideBuildingInfoScreen()
        {
            _interfaceSwitcher.ClosePanel<UIBuildingOptions>();
        }

        public void InitBuildingUI(ConstructionBuilding building)
        {
            if (building.BuildBar == null)
            {
                building.BuildBar = Instantiate(_barBuild, _buttonsParent);
                building.BuildBar.gameObject.SetActive(false);
            }

            ResourcesBuilding resourcesBuilding;
            switch (building.buildingId)
            {
                case BuildingID.supplydrop:
                    resourcesBuilding = building as ResourcesBuilding;
                    if (resourcesBuilding != null && resourcesBuilding.CollectButton == null)
                    {
                        resourcesBuilding.CollectButton = Instantiate(_collectSuppliesButton, _buttonsParent);
                        resourcesBuilding.CollectButton.Init(_playerSyncController);
                        resourcesBuilding.CollectButton.button.onClick.AddListener(resourcesBuilding.Collect);
                        resourcesBuilding.CollectButton.ObserveSuppliesValues();
                        resourcesBuilding.CollectButton.gameObject.SetActive(false);
                        resourcesBuilding.onCollectResources.AddListener(CollectResources);
                    }

                    break;
                case BuildingID.powerplant:
                    resourcesBuilding = building as ResourcesBuilding;
                    if (resourcesBuilding != null && resourcesBuilding.CollectButton == null)
                    {
                        resourcesBuilding.CollectButton = Instantiate(_collectPowerButton, _buttonsParent);
                        resourcesBuilding.CollectButton.Init(_playerSyncController);
                        resourcesBuilding.CollectButton.button.onClick.AddListener(resourcesBuilding.Collect);
                        resourcesBuilding.CollectButton.ObservePowerValues();
                        resourcesBuilding.CollectButton.gameObject.SetActive(false);
                        resourcesBuilding.onCollectResources.AddListener(CollectResources);
                    }

                    break;
                case BuildingID.builderstation:
                    break;
            }
        }

        public void ShowMessage(string message, string buttonText = "Ok", Action callback = null)
        {
            _messageBox.Show(message, new MessageBoxButtonData(buttonText, callback));
        }

        public void ShowLeaveMessage(Action<bool> callback)
        {
            List<MessageBoxButtonData> options = new()
            {
                new MessageBoxButtonData("REMOVE", () =>
                {
                    _gameController.ClearGameData();
                    callback.Invoke(true);
                }),
                new MessageBoxButtonData("KEEP", () =>
                {
                    _gameController.ClearGameData();
                    callback.Invoke(false);
                }),
                new MessageBoxButtonData("Cancel", null)
            };

            _messageBox.Show("Do you really want to leave your base?", options);
        }

        private void CollectResources(long databaseId)
        {
            _networkPacket.SendCollectRequest(databaseId);
        }
    }
}