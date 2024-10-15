using System;
using CityBuildingKit.Scripts.Utils;
using SargeUniverse.Scripts.Enums;
using SargeUniverse.Scripts.Model.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI.Elements
{
    public class ArmouryUnitCard : MonoBehaviour
    {
        [Header("Image Settings")]
        [SerializeField] private Image _unitImage = null;
        [SerializeField] private OnImageClick _onImageClick = null;
        
        [Header("Footer Settings")]
        [SerializeField] private GameObject _footer = null;
        [SerializeField] private Image _levelRankImage = null;
        [SerializeField] private TMP_Text _levelRankText = null;
        [SerializeField] private TMP_Text _upgradePriceText = null;

        private Material _unitImageMaterial = null;
        private UnitID _id; 
        private int _upgradePrice = 0;

        private void Awake()
        {
            _unitImageMaterial = _unitImage?.material;
        }

        public void InitCard(ArmouryUnitCardData cardData, Action<UnitID, int> callback)
        {
            _id = cardData.id;
            _unitImage.sprite = cardData.locked ? cardData.unitImageLocked : cardData.unitImage;
            _levelRankImage.sprite = cardData.levelRankImage;
            _levelRankText.text = cardData.level.ToString();
            _upgradePriceText.text = cardData.upgradePrice.ToString();
            _upgradePrice = cardData.upgradePrice;

            _onImageClick.onClick += () => callback.Invoke(_id, _upgradePrice);
            _onImageClick.SetInteractable(!cardData.locked);
            _footer.SetActive(!cardData.locked);
        }
    }
}