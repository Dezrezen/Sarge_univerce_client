/**
 * This source file is part of CityBuildingKit.
 * Copyright (c) 2018.
 * All rights reserved.
 */

using System;
using Assets.Scripts;
using Assets.Scripts.UIControllersAndData.Images;
using Assets.Scripts.UIControllersAndData.Store;
using JetBrains.Annotations;
using TMPro;
using UIControllersAndData.Models;
using UnityEngine;
using UnityEngine.UI;

namespace UIControllersAndData.Store.ShopItems
{
    public class BaseShopItem : MonoBehaviour, IBaseShopItem
    {
        [SerializeField] private Image _backgroundImage;

        [SerializeField] protected Image BigImage;

        [SerializeField] private Image _smallIcon;
        [SerializeField] protected TextMeshProUGUI TitleText;
        [SerializeField] protected TextMeshProUGUI TimeLabel;

        [SerializeField] private Image _iconOnBuyButton;
        [SerializeField] protected TextMeshProUGUI BuyButtonLabel;

        [SerializeField] protected TextMeshProUGUI QuantityOfItem;

        [SerializeField] private GameObject _notAvailableMask;

        public Action OnClick;

        public RectTransform RectTransform { get; private set; }

        public TextMeshProUGUI BuyButtonLabel1
        {
            get => BuyButtonLabel;
            set => BuyButtonLabel = value;
        }

        public TextMeshProUGUI QuantityOfItem1
        {
            get => QuantityOfItem;
            set => QuantityOfItem = value;
        }

        public BaseStoreItemData ItemData { get; set; }

        public int Id { get; set; }

        private void Awake()
        {
            RectTransform = GetComponent<RectTransform>();
        }

        public virtual void Initialize(DrawCategoryData data, ShopCategoryType shopCategoryType)
        {
            ItemData = data.BaseItemData;
            Id = data.Id.GetId();


            var requiredHqLevel = ((IRequiredHQLevel)data.BaseItemData).GetRequiredHQLevel();
            NotAvailable(requiredHqLevel);

            if (ItemData == null) throw new Exception("Item data is null");

            if (TitleText) TitleText.text = data.Name.GetName();

            if (BuyButtonLabel) BuyButtonLabel.text = ItemData.Price.ToString();

            if (BigImage)
            {
                BigImage.enabled = !string.IsNullOrEmpty(ItemData.IdOfBigIcon);
                BigImage.sprite = ImageControler.GetImage(ItemData.IdOfBigIcon);
            }

            if (_smallIcon)
            {
                _smallIcon.enabled = !string.IsNullOrEmpty(ItemData.IdOfSmallIcon);
                _smallIcon.sprite = ImageControler.GetImage(ItemData.IdOfSmallIcon);
            }

            if (_iconOnBuyButton)
            {
                _iconOnBuyButton.enabled = !string.IsNullOrEmpty(ItemData.IdOfIconOnBuyButton);
                _iconOnBuyButton.sprite = ImageControler.GetImage(ItemData.IdOfIconOnBuyButton);
            }
        }

        [UsedImplicitly]
        public void OnItemClick()
        {
            if (OnClick != null) OnClick();
        }

        [UsedImplicitly]
        public void OnDescriptionClick()
        {
            if (ItemData is INamed namedItem)
                DescriptionPanel.Instance.SetDescriptionInfo(namedItem.GetName(), ItemData.Description);
            else
                throw new Exception("named item is not null");
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity == ItemData.MaxCountOfThisItem)
            {
                _backgroundImage.sprite = ImageControler.GetImage(Constants.ID_ITEM_BACKGROUND);
                BigImage.sprite = ImageControler.GetImage(ItemData.IdOfBlackBigIcon);
                transform.GetComponent<Button>().interactable = false;
                TitleText.color = Color.black;
                TimeLabel.color = Color.black;
                QuantityOfItem.color = Color.black;
            }


            if (QuantityOfItem) QuantityOfItem.text = quantity + "/" + ItemData.MaxCountOfThisItem;
        }

        public void NotAvailable(int requiredHQLevelForUpgrade)
        {
            var isInteractable = Stats.Instance.CurrentHqLevel >= requiredHQLevelForUpgrade;
            gameObject.GetComponent<Button>().interactable = isInteractable;
            _notAvailableMask.SetActive(!isInteractable);
        }
    }
}