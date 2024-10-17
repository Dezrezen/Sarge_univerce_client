using System.Collections.Generic;
using SargeUniverse.Scripts.Config;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts.UI.Shop
{
    public class UIShopShieldTabPanel : UIShopTabPanel
    {
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
            GenerateCards(_shopItemsConfig.GetDataOfType(ShopItemType.Shield));
        }

        private void GenerateCards(List<ShopItemConfig> cardsConfig)
        {
            foreach (var config in cardsConfig)
            {
                var card = Instantiate(_cardPrefab, _content);
                card.Init(config, BuyItem);
                
                _cards.Add(card);
            }
        }

        private void BuyItem(string id, float price)
        {
            _uiShop.BuyItemCallback(id, price);
        }
    }
}