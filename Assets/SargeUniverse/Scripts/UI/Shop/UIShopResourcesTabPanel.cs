using System.Collections.Generic;
using SargeUniverse.Scripts.Config;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts.UI.Shop
{
    public class UIShopResourcesTabPanel : UIShopTabPanel
    {
        [SerializeField] private ShopTabsScroll _shopTabsScroll = null;
        [SerializeField] private ShopItemCard _cardPrefab = null;
        
        private List<ShopItemCard> _cards = new();

        private ShopItemsConfig _shopItemsConfig;
        
        [Inject]
        private void Construct(ShopItemsConfig shopItemsConfig)
        {
            _shopItemsConfig = shopItemsConfig;
        }
        
        protected override void InitCards()
        {
            GenerateCards(_shopItemsConfig.GetDataOfType(ShopItemType.Credits));
            GenerateCards(_shopItemsConfig.GetDataOfType(ShopItemType.Supplies));
            GenerateCards(_shopItemsConfig.GetDataOfType(ShopItemType.Power));
            GenerateCards(_shopItemsConfig.GetDataOfType(ShopItemType.Tachyon));

        }

        private void GenerateCards(List<ShopItemConfig> cardsConfig)
        {
            RectTransform rect = null;
            foreach (var config in cardsConfig)
            {
                var card = Instantiate(_cardPrefab, _content);
                card.Init(config, BuyItem);

                if (rect == null)
                {
                    rect = card.GetComponent<RectTransform>();
                    _shopTabsScroll.AddScrollPosition(rect);
                }
                _cards.Add(card);
            }
        }

        private void BuyItem(string id, float price)
        {
            _uiShop.BuyItemCallback(id, price);
        }
    }
}