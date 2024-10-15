using System.Collections.Generic;
using System.Linq;
using Config;
using Controller;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Model.UI;
using SargeUniverse.Scripts.Sound;
using TMPro;
using UI.Base;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SargeUniverse.Scripts.UI
{
    public class UIArmyScreen : UIScreen
    {
        [SerializeField] private Button _closeButton = null;
        [SerializeField] private Slider _troopsSlider = null;
        [SerializeField] private TMP_Text _troopsText = null;
        
        [SerializeField] private ArmyUnitCard _armyUnitCard = null;
        [SerializeField] private AvailableUnitCard _availableUnitCard = null;
        [SerializeField] private Transform _armyContent = null;
        [SerializeField] private Transform _trainQueueContent = null;
        [SerializeField] private Transform _availableUnitsContent = null;

        [SerializeField] private TMP_Text _armyHousungText = null;
        [SerializeField] private TMP_Text _trainingHousingText = null;

        private List<ArmyUnitCard> _armyRoster = new();
        private List<ArmyUnitCard> _trainQueue = new();
        private List<AvailableUnitCard> _armyUnits = new();

        private int _armyHousing = 0;
        private int _trainingHousing = 0;

        private UnitsConfig _unitsConfig;
        private NetworkPacket _networkPacket;
        private GameController _gameController;
        private PlayerSyncController _playerSyncController;

        [Inject]
        private void Construct(
            UnitsConfig unitsConfig,
            NetworkPacket networkPacket,
            GameController gameController,
            PlayerSyncController playerSyncController)
        {
            _unitsConfig = unitsConfig;
            _networkPacket = networkPacket;
            _gameController = gameController;
            _playerSyncController = playerSyncController;
        }
        
        public override void ShowScreen()
        {
            LoadScreenData();
            base.ShowScreen();
            UIManager.Instanse.LockScreenMove = true;
        }

        public override void HideScreen()
        {
            base.HideScreen();
            ClearScreenData();
            UIManager.Instanse.LockScreenMove = false;
        }

        protected override void SubscribeForEvents()
        {
            _closeButton.onClick.AddListener(OnCloseButton);
            _playerSyncController.UnitsCapacity.SubscribeForUpdate(UpdateTroopsCountInfo);
            _playerSyncController.MaxUnitsCapacity.SubscribeForUpdate(UpdateTroopsCountInfo);
        }

        protected override void UnSubscribeFromEvents()
        {
            _closeButton.onClick.RemoveListener(OnCloseButton);
            _playerSyncController.UnitsCapacity.UnSubscribeFromUpdate(UpdateTroopsCountInfo);
            _playerSyncController.MaxUnitsCapacity.UnSubscribeFromUpdate(UpdateTroopsCountInfo);
        }

        private void Update()
        {
            if (UnitsManager.Instanse.updateUnitsGroups)
            {
                UnitsManager.Instanse.updateUnitsGroups = false;
                ClearScreenData();
                LoadScreenData();
            }

            foreach (var card in _trainQueue)
            {
                // TODO: REWORK
                card.UpdateTrainProgress(_playerSyncController, RequestSyncData);
            }
        }

        private void LoadScreenData()
        {
            var unitsData = GameConfig.instance.UnitsConfig.unitsData;
            foreach (var unitData in unitsData)
            {
                var cardData = GetAvailableUnitCardData(unitData.id);
                var card = Instantiate(_availableUnitCard, _availableUnitsContent);
                card.InitCard(cardData, TryTrainUnit);
                card.SetCardImage(unitData.image);
                _armyUnits.Add(card);
            }
            
            foreach (var group in UnitsManager.Instanse.ArmyUnits)
            {
                var data = _unitsConfig.GetUnitData(group.UnitsId);
                var card = Instantiate(_armyUnitCard, _armyContent);
                card.InitCard(group, DeleteTrainedUnit);
                card.SetCardImage(data.image);
                _armyRoster.Add(card);

                _armyHousing += _gameController.GetServerUnit(group.UnitsId).housing * group.GroupSize();
            }
            _armyHousungText.text = _armyHousing + "\\" + _playerSyncController.MaxUnitsCapacity.GetValue();
            
            foreach (var group in UnitsManager.Instanse.TrainingUnits)
            {
                var data = _unitsConfig.GetUnitData(group.UnitsId);
                var card = Instantiate(_armyUnitCard, _trainQueueContent);
                card.InitCard(group, RemoveFromTrainQuery, true);
                card.SetCardImage(data.image);
                _trainQueue.Add(card);
                
                _trainingHousing += _gameController.GetServerUnit(group.UnitsId).housing * group.GroupSize();
            }

            if (_trainQueue.Count > 0)
            {
                _trainQueue.First().ShowTrainProgress(_playerSyncController);
            }

            _trainingHousingText.text =
                _trainingHousing + "\\" + _playerSyncController.MaxUnitsCapacity.GetValue() * 2;
        }

        private void ClearScreenData()
        {
            foreach (var unit in _armyRoster)
            {
                Destroy(unit.gameObject); 
            }
            _armyRoster.Clear();
            _armyHousing = 0;
            
            foreach (var unit in _trainQueue)
            {
                Destroy(unit.gameObject); 
            }
            _trainQueue.Clear();
            _trainingHousing = 0;
            
            foreach (var unit in _armyUnits)
            {
                Destroy(unit.gameObject); 
            }
            _armyUnits.Clear();
        }

        private void OnCloseButton()
        {
            SoundSystem.Instance.PlaySound("click");
            HideScreen();
        }

        private void UpdateTroopsCountInfo()
        {
            var totalUnits = _playerSyncController.UnitsCapacity.GetValue();
            var maxUnits = _playerSyncController.MaxUnitsCapacity.GetValue();

            _troopsSlider.value = 1f * totalUnits / maxUnits;
            _troopsText.text = totalUnits + "\\" + maxUnits;
        }

        private void TryTrainUnit(UnitID unitId)
        {
            SoundSystem.Instance.PlaySound("click");
            var unitData = _gameController.GetServerUnit(unitId);
            var maxCapacity = _playerSyncController.MaxUnitsCapacity.GetValue();
            var capacity = _playerSyncController.UnitsCapacity.GetValue();
            
            if (_playerSyncController.Power.GetValue() < unitData.requiredPower)
            {
                UIManager.Instanse.ShowMessage("No Resources");
            }
            else if (maxCapacity - capacity < unitData.housing)
            {
                UIManager.Instanse.ShowMessage("No capacity");
            }
            else
            {
                _networkPacket.SendTrainUnitRequest(unitId);
            }
        }

        private void RemoveFromTrainQuery(long databaseId)
        {
            SoundSystem.Instance.PlaySound("click");
            _networkPacket.SendCancelTrainUnitRequest(databaseId);
        }

        private void DeleteTrainedUnit(long databaseId)
        {
            SoundSystem.Instance.PlaySound("click");
            _networkPacket.SendRemoveUnitRequest(databaseId);
        }

        private void RequestSyncData()
        {
            _networkPacket.SyncRequest();
        }
        
        // -----

        private ArmyAvailableUnitCardData GetAvailableUnitCardData(UnitID unitId)
        {
            var unitData = _gameController.GetServerUnit(unitId);
            var cardData = new ArmyAvailableUnitCardData
            {
                unitId = unitId,
                trainPrice = unitData.requiredPower,
                level = 1
            };
            
            return cardData;
        }
    }
}