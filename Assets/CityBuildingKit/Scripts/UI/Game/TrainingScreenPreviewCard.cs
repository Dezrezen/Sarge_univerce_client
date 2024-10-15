using System;
using Data;
using UI.Base;
using CityBuildingKit.Scripts.Utils;
using Controller;
using Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class TrainingScreenPreviewCard : UnitCard
    {
        [SerializeField] private Button infoButton;
        [SerializeField] private TMP_Text levelLabel;
        [SerializeField] private TMP_Text priceLabel;

        private OnImageClick _imageClickAction;

        private UnitData _unitData;
        
        private void Awake()
        {
            _imageClickAction = cardImage.GetComponent<OnImageClick>();
        }

        private void OnDestroy()
        {
            
        }

        public void SetCardInfo(UnitData unitData, Action<UnitID> callback)
        {
            _unitData = unitData;
            levelLabel.text = unitData.level.ToString();
            priceLabel.text = unitData.price.ToString();
            _imageClickAction.onClick += () => callback(unitData.id);
        }
    }
}