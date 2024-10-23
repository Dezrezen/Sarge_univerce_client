using Controller;
using SargeUniverse.Scripts;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Sound;
using SargeUniverse.Scripts.UI;
using SargeUniverse.Scripts.UI.Shop;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace CityBuildingKit.Scripts.UI
{
    public class UIMain : UIScreen
    {
        [Header("Left Group")]
        [SerializeField] private Button _playerInfoButton = null;
        [SerializeField] private TMP_Text _playerNameText = null;
        [SerializeField] private Slider _playerExpSlider = null;
        [SerializeField] private TMP_Text _playerLvlText = null;

        [SerializeField] private Button _tournamentButton = null;
        [SerializeField] private TMP_Text _tournamentRankText = null;

        [SerializeField] private Button _mailButton;
        
        [Header("Center Group")]
        [SerializeField] private TMP_Text _workersText = null;
        [SerializeField] private Button _buyMoreWorkersButton = null;
        [SerializeField] private TMP_Text _shieldTimerText = null;
        [SerializeField] private Button _buyShieldTimeButton = null;
        
        [Header("Right Group")]
        [SerializeField] private Slider _supplySlider = null;
        [SerializeField] private TMP_Text _supplyText = null;
        [SerializeField] private Slider _powerSlider = null;
        [SerializeField] private TMP_Text _powerText = null;
        [SerializeField] private GameObject _tachyonGroup = null;
        [SerializeField] private Slider _tachyonSlider = null;
        [SerializeField] private TMP_Text _tachyonText = null;
        [SerializeField] private TMP_Text _gemsText = null;
        [SerializeField] private Button _buyMoreGemsButton = null;

        [Header("Bottom Group")]
        [SerializeField] private Button _shopButton = null;
        [SerializeField] private Button _settingsButton = null;
        [SerializeField] private Button _editModeButton = null;
        [SerializeField] private Button _chatButton = null;
        
        [SerializeField] private Button _attackButton = null;
        [SerializeField] private Button _reportButton = null;
        [SerializeField] private Button _armyButton = null;
        [SerializeField] private Button _warModeButton = null;

        [SerializeField] private Button _addMoney = null;
        [SerializeField] private Button _restart = null;

        private NetworkPacket _networkPacket;
        private ResponseParser _responseParser;
        private LevelLoader _levelLoader;
        private PlayerSyncController _playerSyncController;
        
        [Inject]
        private void Construct(
            NetworkPacket networkPacket,
            ResponseParser responseParser,
            LevelLoader levelLoader,
            PlayerSyncController playerSyncController)
        {
            _networkPacket = networkPacket;
            _responseParser = responseParser;
            _levelLoader = levelLoader;
            _playerSyncController = playerSyncController;
        }
        
        protected override void SubscribeForEvents()
        {
            _playerSyncController.Xp.SubscribeForUpdate(UpdateXpAndLevel);
            
            _playerSyncController.MaxSupplies.SubscribeForUpdate(UpdateSuppliesInfo);
            _playerSyncController.Supplies.SubscribeForUpdate(UpdateSuppliesInfo);
            _playerSyncController.MaxPower.SubscribeForUpdate(UpdatePowerInfo);
            _playerSyncController.Power.SubscribeForUpdate(UpdatePowerInfo);
            _playerSyncController.MaxEnergy.SubscribeForUpdate(UpdateTachyonInfo);
            _playerSyncController.Energy.SubscribeForUpdate(UpdateTachyonInfo);
            _playerSyncController.Gems.SubscribeForUpdate(UpdateGemsInfo);
            
            _playerSyncController.MaxWorkers.SubscribeForUpdate(UpdateWorkers);
            _playerSyncController.Workers.SubscribeForUpdate(UpdateWorkers);
            
            _shopButton.onClick.AddListener(OnShopButtonClick);
            _armyButton.onClick.AddListener(OnArmyButtonClick);
            _warModeButton.onClick.AddListener(ShowWarScreen);
            _attackButton.onClick.AddListener(ShowAttackScreen);
            _settingsButton.onClick.AddListener(SettingsButtonClick);
            
            _addMoney.onClick.AddListener(AddMoneyClick);
            _restart.onClick.AddListener(RestartGame);
        }

        protected override void UnSubscribeFromEvents()
        {
            _playerSyncController.Xp.UnSubscribeFromUpdate(UpdateXpAndLevel);
            
            _playerSyncController.MaxSupplies.UnSubscribeFromUpdate(UpdateSuppliesInfo);
            _playerSyncController.Supplies.UnSubscribeFromUpdate(UpdateSuppliesInfo);
            _playerSyncController.MaxPower.UnSubscribeFromUpdate(UpdatePowerInfo);
            _playerSyncController.Power.UnSubscribeFromUpdate(UpdatePowerInfo);
            _playerSyncController.MaxEnergy.UnSubscribeFromUpdate(UpdateTachyonInfo);
            _playerSyncController.Energy.UnSubscribeFromUpdate(UpdateTachyonInfo);
            _playerSyncController.Gems.UnSubscribeFromUpdate(UpdateGemsInfo);
            
            _playerSyncController.MaxWorkers.UnSubscribeFromUpdate(UpdateWorkers);
            _playerSyncController.Workers.UnSubscribeFromUpdate(UpdateWorkers);
            
            _shopButton.onClick.RemoveListener(OnShopButtonClick);
            _armyButton.onClick.RemoveListener(OnArmyButtonClick);
            _warModeButton.onClick.RemoveListener(ShowWarScreen);
            _attackButton.onClick.RemoveListener(ShowAttackScreen);
            _settingsButton.onClick.RemoveListener(SettingsButtonClick);
            
            _addMoney.onClick.RemoveListener(AddMoneyClick);
            _restart.onClick.RemoveListener(RestartGame);
        }

        private void UpdateXpAndLevel()
        {
            _playerNameText.text = _playerSyncController.PlayerName;
            _playerExpSlider.value = 1f * 
                                     _playerSyncController.Xp.GetValue() / 
                                     _playerSyncController.GetNextLevelRequiredXp();
            _playerLvlText.text = _playerSyncController.Level.ToString();
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
            _tachyonText.text = _playerSyncController.Power.GetValue().ToString();
        }

        private void UpdateGemsInfo()
        {
            _gemsText.text = _playerSyncController.Gems.GetValue().ToString();
        }

        private void UpdateWorkers()
        {
            _workersText.text = _playerSyncController.Workers.GetValue() + "/" +
                                _playerSyncController.MaxWorkers.GetValue();
        }

        private void OnShopButtonClick()
        {
            SoundSystem.Instance.PlaySound("click");
            if (InterfaceSwitcher.SwitchPanel<UIShop>())
            {
                HideScreen();
            }
        }

        private void OnArmyButtonClick()
        {
            SoundSystem.Instance.PlaySound("click");
            if (_playerSyncController.CanTrainUnits())
            {
                InterfaceSwitcher.ShowPanel<UIArmyScreen>();
            }
            else
            {
                UIManager.Instanse.ShowMessage("No Barracks found", "Ok", null);
            }
        }

        private void ShowWarScreen()
        {
            SoundSystem.Instance.PlaySound("click");
            _levelLoader.LoadScene(Constants.WarScene);
            //SceneManager.LoadSceneAsync(Constants.WarScene);
        }
        
        private void ShowAttackScreen()
        {
            SoundSystem.Instance.PlaySound("click");
            InterfaceSwitcher.ShowPanel<UIAttackScreen>();
        }

        private void SettingsButtonClick()
        {
            SoundSystem.Instance.PlaySound("click");
            InterfaceSwitcher.ShowPanel<UISettings>();
        }

        private void AddMoneyClick()
        {
            SoundSystem.Instance.PlaySound("click");
            _networkPacket.SendAddMoneyRequest();
        }

        private void RestartGame()
        {
            SoundSystem.Instance.PlaySound("click");
            UIManager.Instanse.ShowLeaveMessage((remove) =>
                {
                    _responseParser.onResetGame.AddListener(OnResetGame);
                    _networkPacket.SendRestartGameRequest(remove);
                });
        }

        private void OnResetGame()
        {
            _levelLoader.LoadScene(Constants.StartScene);
        }

        public override void ShowScreen()
        {
            base.ShowScreen();
            UIManager.Instanse.SetMenuActive(true);
        }

        public override void HideScreen()
        {
            base.HideScreen();
            UIManager.Instanse.SetMenuActive(false);
        }
    }
}