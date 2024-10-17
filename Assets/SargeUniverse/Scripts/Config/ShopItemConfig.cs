using System;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Config
{
    [Serializable]
    public class ShopItemConfig
    {
        public string name;
        public string id;
        public string description;
        public Sprite icon;
        public float price;
        public ShopItemType type;
    }
}