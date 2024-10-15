using UnityEngine;

namespace CityBuildingKit.Scripts.Data.Units
{
    [System.Serializable]
    public class TroopUpgradeInfo
    {
        public int damage;
        public int dps;
        public int hp;
        public int trainingCost;
        public int researchCost;
        public int researchTime;
        public int armoryLevel;
        public int hqLevel;
    }
}