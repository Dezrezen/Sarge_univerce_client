using System;
using SargeUniverse.Scripts.Config;
using SargeUniverse.Scripts.Enums;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SargeUniverse.Scripts.UI.Shop
{
    public class ShopItemCard : MonoBehaviour
    {
        [SerializeField] private Image _cardImage = null;
        [SerializeField] private TMP_Text _cardName = null;

        [Header("Description Group")]
        [SerializeField] private GameObject _descriptionGroup = null;
        [SerializeField] private TMP_Text _descriptionText = null;

        [Header("Button Group")]
        [SerializeField] private Button _button = null;
        [SerializeField] private TMP_Text _priceText;
        [SerializeField] private GameObject _icon = null;

        private string _id = string.Empty;
        private float _price = 0f;

        private void Awake()
        {
            _descriptionGroup.SetActive(false);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveAllListeners();
        }

        public void Init(ShopItemConfig cardConfig, Action<string, float> callback)
        {
            _id = cardConfig.id;
            _price = cardConfig.price;

            _cardImage.sprite = cardConfig.icon;
            _cardName.text = cardConfig.name;

            if (!string.IsNullOrEmpty(cardConfig.description))
            {
                _descriptionGroup.SetActive(true);
                _descriptionText.text = cardConfig.description;
            }

            if (cardConfig.type is 
                ShopItemType.Booster or ShopItemType.ResourcesPackage or ShopItemType.UAC or ShopItemType.UBC)
            {
                _priceText.text = $"{cardConfig.price:F0}";
                _icon.SetActive(true);
            }
            else
            {
                _priceText.text = $"{cardConfig.price:F2}" + "$";
                _icon.SetActive(false);
            }
            
            _button.onClick.AddListener(() => callback.Invoke(_id, _price));
        }
    }
}