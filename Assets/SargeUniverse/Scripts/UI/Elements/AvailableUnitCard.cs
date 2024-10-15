using System;
using Controller;
using SargeUniverse.Scripts.Controller;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Model.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI
{
    public class AvailableUnitCard : UnitCard
    {
        [SerializeField] private Button _infoButton = null;
        [SerializeField] private Button _cardButton = null;
        [SerializeField] private TMP_Text _priceText = null;

        private UnitID _unitId = UnitID.rifleman;
        
        private void OnDestroy()
        {
            _cardButton.onClick.RemoveAllListeners();
        }

        public void InitCard(ArmyAvailableUnitCardData cardData, Action<UnitID> callback)
        {
            _unitId = cardData.unitId;
            _priceText.text = cardData.trainPrice.ToString();
            SetLevel(cardData.level.ToString());
            
            _cardButton.onClick.AddListener(() => callback.Invoke(_unitId));
        }
    }
}