using SargeUniverse.Scripts.Data;
using SargeUniverse.Scripts.Enums;
using UnityEngine;

namespace SargeUniverse.Scripts.Model.UI
{
    public class ShopCardData
    {
        public BuildingID buildingId;
        public string name = string.Empty;
        public Sprite image;
        public string buildTime = "0";
        public int buildCost = 0;

        public BuildingCount limit;
        public int hqUnlockLevel = -1;
    }
}