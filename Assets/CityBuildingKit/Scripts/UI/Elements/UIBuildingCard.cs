using System;
using Controller;
using SargeUniverse.Scripts;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Model.UI;
using SargeUniverse.Scripts.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace UI.Elements
{
    public class UIBuildingCard : MonoBehaviour
    {
        private const string LockLabel = "Level {0} Hq Required";
        
        [SerializeField] private TMP_Text _buildingName = null;
        [SerializeField] private Image _buildingImage = null;
        [SerializeField] private Button _infoButton = null;
        [SerializeField] private TMP_Text _buildTimeText = null;
        [SerializeField] private TMP_Text _amountText = null;
        [SerializeField] private Button _buildButton = null;
        
        [SerializeField] private TMP_Text _priceText = null;
        [SerializeField] private Image _suppliesIcon = null;
        [SerializeField] private Image _gemsIcon = null;

        [Header("Lock")]
        [SerializeField] private GameObject _lockImage = null;
        [SerializeField] private TMP_Text _lockText = null;

        private int _buildingCost = 0;
        private int _buildingHave = 0;
        private int _buildingCount = 0;

        private BuildingID _buildingId;
        
        private void Start()
        {
            var rect = GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero;
        }

        private void OnDestroy()
        {
            _buildButton.onClick.RemoveAllListeners();
        }

        public BuildingID GetId()
        {
            return _buildingId;
        }

        public void Init(ShopCardData cardData, Action<BuildingID, int> callback)
        {
            _buildingId = cardData.buildingId;
            _buildingCost = cardData.buildCost;
            
            _buildingName.text = cardData.name;
            _buildingImage.sprite = cardData.image;
            _buildTimeText.text = cardData.buildTime;
            _priceText.text = TextUtils.NumberToTextWithSeparator(cardData.buildCost);
            
            _suppliesIcon.gameObject.SetActive(_buildingId != BuildingID.builderstation);
            _gemsIcon.gameObject.SetActive(_buildingId == BuildingID.builderstation);
            
            UpdateCardInfo(cardData.limit, cardData.hqUnlockLevel);
            
            _buildButton.onClick.AddListener(() => OnBuildButton(callback));
        }

        public void UpdateCardInfo(BuildingCount limit, int hqUnlockLevel)
        {
            _lockImage.SetActive(limit == null);
            _amountText.gameObject.SetActive(limit != null);
            if (limit == null)
            {
                _lockText.text = string.Format(LockLabel, hqUnlockLevel);
            }
            else
            {
                _buildingHave = limit.have;
                _buildingCount = limit.count;
                _amountText.text = _buildingHave + "/" + _buildingCount;

                if (_buildingId == BuildingID.builderstation)
                {
                    _buildingCost = Constants.GetBuilderStationBuildCost(_buildingHave);
                    _priceText.text = _buildingCost.ToString();
                }
            }
        }

        private void OnBuildButton(Action<BuildingID, int> callback)
        {
            if (_buildingCount <= 0)
            {
                UIManager.Instanse.ShowMessage("Building locked");
            }
            else if (_buildingHave == _buildingCount)
            {
                UIManager.Instanse.ShowMessage("Max Buildings reached");
            }
            else 
            {
                callback.Invoke(_buildingId, _buildingCost);
            }
        }
    }
}