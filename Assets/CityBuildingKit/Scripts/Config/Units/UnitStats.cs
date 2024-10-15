using CityBuildingKit.Scripts.Enums;

namespace Config.Units
{
    [System.Serializable]
    public class UnitStats
    {
        public PreferredTarget preferredTarget;
        public float attackRange;
        public AttackOptions attackOption;
        public AttackType attackType;
        public float attackSpeed;
        public int housingSpace;
        public int movementSpeed;
        public int barrackLevel;
    }
}