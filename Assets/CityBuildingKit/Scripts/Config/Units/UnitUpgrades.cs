using UnityEngine;

namespace Config.Units
{
    [System.Serializable]
    public class UnitUpgrades
    {
        [HideInInspector]
        public string name;
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