using System.Collections.Generic;
using SargeUniverse.Scripts.Enums;
using UnityEngine;
using Zenject;

namespace SargeUniverse.Scripts.Config
{
    [CreateAssetMenu(fileName = "ShopItemsConfig", menuName = "Data/ShopItemsConfig")]
    public class ShopItemsConfig : ScriptableObjectInstaller
    {
        public List<ShopItemConfig> shopItems = new();

        public List<ShopItemConfig> GetDataOfType(ShopItemType type)
        {
            return shopItems.FindAll(data => data.type == type);
        }
        
        public override void InstallBindings()
        {
            Container.Bind<ShopItemsConfig>()
                .FromInstance(this)
                .AsSingle();
        }
    }
}