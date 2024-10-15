using System;
using System.Linq;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Model;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class ArmyUnitCard : UnitCard
    {
        [SerializeField] private Button _removeButton = null;
        [SerializeField] private TMP_Text _amountText = null;
        [SerializeField] private Slider _trainSlider = null;
        [SerializeField] private GameObject _lockImage = null;

        private UnitsGroup _group = null;
        private bool _trainCard = false;
        
        private bool _showProgress = false;
        private DateTime _trainStartTime;
        private DateTime _trainEndTime;
        
        private void Start()
        {
            _trainSlider.gameObject.SetActive(_showProgress);
        }

        private void OnDestroy()
        {
            _removeButton.onClick.RemoveAllListeners();
        }

        public void UpdateTrainProgress(PlayerSyncController playerSyncController, Action callback)
        {
            if (_showProgress)
            {
                var span = TimeSpan.Zero;
                if (PlayerSyncController.ServerTime < _trainEndTime)
                {
                    span = _trainEndTime - PlayerSyncController.ServerTime;
                }

                if (span.TotalSeconds <= 0)
                {
                    callback.Invoke();
                    GetUnitTrainTime(playerSyncController);
                    return;
                }

                var trainTime = (_trainEndTime - _trainStartTime).TotalSeconds;;
                _trainSlider.value = Mathf.Abs(1f - (float)span.TotalSeconds / (float)trainTime);
            }
        }

        public void InitCard(UnitsGroup group, Action<long> callback, bool trainCard = false)
        {
            _group = group;
            _trainCard = trainCard;
            _amountText.text = "x" + _group.GroupSize();
            SetLevel("1");
            _removeButton.onClick.AddListener(() => 
                callback.Invoke(_trainCard ? _group.DatabseIds.Last() : _group.GetUnit())
            );
        }

        public void ShowTrainProgress(PlayerSyncController playerSyncController)
        {
            _showProgress = true;
            GetUnitTrainTime(playerSyncController);
        }

        private void GetUnitTrainTime(PlayerSyncController playerSyncController)
        {
            var unitData = playerSyncController.GetUnit(_group.GetUnit());
            _trainStartTime = unitData.trainStartTime;
            _trainEndTime = unitData.trainEndTime;
        }
    }
}