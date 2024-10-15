using SargeUniverse.Scripts.Controller;
using UnityEngine;

namespace UI.Elements
{
    public class UICollectButton : UIButton
    {
        [SerializeField] private Sprite _defaultSprite = null;
        [SerializeField] private Sprite _fullSprite = null;

        private PlayerSyncController _playerSyncController;

        public void Init(PlayerSyncController playerSyncController)
        {
            _playerSyncController = playerSyncController;
        }
        
        public void ObserveSuppliesValues()
        {
            _playerSyncController.Supplies.SubscribeForUpdate(CheckSuppliesValues);
            _playerSyncController.MaxSupplies.SubscribeForUpdate(CheckSuppliesValues);
        }

        public void ObservePowerValues()
        {
            _playerSyncController.Power.SubscribeForUpdate(CheckPowerValues);
            _playerSyncController.MaxPower.SubscribeForUpdate(CheckPowerValues);
        }

        private void CheckSuppliesValues()
        {
            if (_playerSyncController.Supplies.GetValue() <
                _playerSyncController.MaxSupplies.GetValue())
            {
                button.image.sprite = _defaultSprite;
                button.interactable = true;
            }
            else
            {
                button.image.sprite = _fullSprite;
                button.interactable = false;
            }
        }
        
        private void CheckPowerValues()
        {
            if (_playerSyncController.Power.GetValue() <
                _playerSyncController.MaxPower.GetValue())
            {
                button.image.sprite = _defaultSprite;
                button.interactable = true;
            }
            else
            {
                button.image.sprite = _fullSprite;
                button.interactable = false;
            }
        }
    }
}